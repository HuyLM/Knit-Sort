using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public SplineComputer splineComputer;
    public MovingSlot MovingSlotPrefab;
    public ConveyorSegment[] Segments;
    private double[] _slotPercents;
    public List<MovingSlot> _slots = new();

    public float desiredSpacing = 1;
    public bool includeEndpoints = true;
    public bool isLooped = true;

    [Header("Thread Guide")]
    public Transform Guide;
    public YarnMoveAnimation yarnMoveLine;

    [Header("Slot -> Hole Yarn Pool")]
    public YarnMoveAnimation yarnMoveLinePrefab; // prefab để Instantiate cho pool khi hết hàng có sẵn
    public float slotToHoleDuration = 0.1f;      // thời gian Play trước khi gọi Stop cho hiệu ứng slot -> hole
    private readonly List<YarnMoveAnimation> _yarnPool = new List<YarnMoveAnimation>();

    [Header("Add Yarn")]
    public GameObject PiecePrefab; // Prefab Piece (lử) để Instantiate khi thêm len mới

    private int _guideSegmentIndex = -1;     // Index segment có IsGuide = true (vị trí 1)
    public Plate _pendingPlate;              // Plate đang chờ được add vào conveyor


    private void Start()
    {
        StartCoroutine(SpawnMovingSlots());
    }

    public IEnumerator SpawnMovingSlots()
    {
        LoadLine();
        yield return null;
        float length = splineComputer.CalculateLength();
        if (length <= 0f) yield break;

        int segmentCount = Mathf.Max(1, Mathf.RoundToInt(length / desiredSpacing));
                float actualSpacing = length / segmentCount;

        int dotCount = isLooped ? segmentCount : (includeEndpoints ? segmentCount + 1 : segmentCount);

       

        int count = Segments.Length;
        // Tính percent của từng Slot trên spline
        _slotPercents = new double[count];
        for (int i = 0; i < count; i++)
        {
            SplineSample sampleP = splineComputer.Project(Segments[i].transform.position);
            _slotPercents[i] = sampleP.percent;
                }

        // Xác định segment là Guide (vị trí 1 - nơi add len mới)
        _guideSegmentIndex = -1;
        for (int i = 0; i < count; i++)
        {
            if (Segments[i].IsGuide)
            {
                _guideSegmentIndex = i;
                break;
            }
        }


        List<double> percents = new();

        SplineSample sample = new SplineSample();
        for (int i = 0; i < dotCount; i++)
        {
            float distance = i * actualSpacing;
            double percent = splineComputer.Travel(0.0, distance, Spline.Direction.Forward);
            splineComputer.Evaluate(percent, ref sample);


            MovingSlot movingSlot = Instantiate(MovingSlotPrefab, sample.position, sample.rotation);
            movingSlot.Follower.spline = splineComputer;

            movingSlot.RegisterSlotTriggers(_slotPercents);
            movingSlot.OnPassSlot.AddListener(OnPassSegmentTrigger);
            var taper = movingSlot.GetComponent<ConveyorTaperSampler>();
            taper.spline = splineComputer;
            taper.follower = movingSlot.Follower;

            _slots.Add(movingSlot);
            percents.Add(percent);
        }
        yield return null;
        for (int i = 0; i < dotCount; i++)
        {
            InitFollower(_slots[i].Follower, percents[i]);
        }
    }

    private void LoadLine()
    {
        List<SplineComputer> SourceSplines = new List<SplineComputer>();

        for (int i = 0; i < Segments.Length; i++)
        {
            SourceSplines.Add(Segments[i].spline);
        }
        List<SplinePoint> allPoints = new List<SplinePoint>();

        for (int s = 0; s < SourceSplines.Count; s++)
        {
            SplinePoint[] points = SourceSplines[s].GetPoints();

            // Bỏ điểm cuối của mỗi spline (trừ spline cuối)
            // để tránh trùng điểm tại chỗ nối
            int count = points.Length;

            for (int i = 0; i < count; i++)
                allPoints.Add(points[i]);
        }


        splineComputer.SetPoints(allPoints.ToArray());

        // Đóng loop
        splineComputer.Close();
    }

    void InitFollower(SplineFollower follower, double percent)
    {
        follower.SetPercent(percent);
    }

private void OnPassSegmentTrigger(MovingSlot movingSlot, int index)
    {
        var segment = Segments[index];

        if (movingSlot.IsEmpty())
        {
            if (segment.Unloader != null)
            {
                segment.Unloader.RemoveBlock(movingSlot);
            }

            // Vị trí 1: segment này là Guide và đang có yêu cầu add len mới.
            // Nếu slot đang đầy (không trống) thì bỏ qua, chờ slot rỗng tiếp theo.
            if (segment.IsGuide && _pendingPlate != null && _pendingPlate._remainingLevels > 0)
            {
                SpawnPieceIntoSlot(movingSlot, _pendingPlate);
            }
        }
        else
        {
            var hole = segment.GetContainer(movingSlot.Color);
            if (hole == null)
            {
                return;
            }
            hole.Add();
            movingSlot.MakeEmpty();
            // spawn a yarnMoveLine để play phase 2: từ MovingSlot (A) -> hole.inPos (B)
            PlaySlotToHoleYarn(movingSlot.OutPos, hole.inPos, movingSlot.Color);
        }
    }

    public int FilledMovingSlotCount()
    {
        return _slots.Count(slot => slot.IsEmpty() == false);
    }

public void PlayMoveLine(Plate plate)
    {
        yarnMoveLine.PlayYarnMove(plate.Out, Guide, 0.25f, plate.Color);
    }

    public void StopMoveLine()
    {
        yarnMoveLine.StopYarnMove(0.25f);
    }
    /// <summary>
    /// Gọi khi người chơi chọn Plate để thêm len mới vào băng chuyền.
    /// Len sẽ được gắn vào slot rỗng đầu tiên đi qua vị trí Guide (vị trí 1).
    /// </summary>
public void RequestAddYarn(Plate plate)
    {
        _pendingPlate = plate;
    }

public void EndAddYarn()
    {
        _pendingPlate = null;
    }

    /// <summary>
    /// Tạo mới 1 Piece (lử) từ prefab, lấy màu từ Plate, rồi gắn vào MovingSlot.
    /// </summary>
private void SpawnPieceIntoSlot(MovingSlot slot, Plate plate)
    {
        slot.Add(plate.Color);
        plate.ConsumeLevel();
    }

    /// <summary>
    /// Lấy 1 YarnMoveAnimation rảnh từ pool (IsPlaying == false). Nếu không có con nào rảnh,
    /// Instantiate thêm 1 con mới từ yarnMoveLinePrefab và thêm vào pool.
    /// </summary>
    private YarnMoveAnimation GetPooledYarnLine()
    {
        for (int i = 0; i < _yarnPool.Count; i++)
        {
            if (_yarnPool[i] != null && !_yarnPool[i].IsPlaying)
            {
                return _yarnPool[i];
            }
        }

        if (yarnMoveLinePrefab == null) return null;

        YarnMoveAnimation newLine = Instantiate(yarnMoveLinePrefab, transform);
        _yarnPool.Add(newLine);
        return newLine;
    }

    /// <summary>
    /// Phát hiệu ứng sợi len bay từ "from" (vị trí MovingSlot) tới "to" (Hole.inPos),
    /// dùng 1 instance lấy từ pool, tự Stop sau slotToHoleDuration giây.
    /// </summary>
    private void PlaySlotToHoleYarn(Transform from, Transform to, GameColor color)
    {
        YarnMoveAnimation line = GetPooledYarnLine();
        if (line == null) return;

        line.PlayYarnMove(from, to, slotToHoleDuration, color);
        StartCoroutine(StopYarnAfterDelay(line, slotToHoleDuration));
    }

    private IEnumerator StopYarnAfterDelay(YarnMoveAnimation line, float delay)
    {
        yield return new WaitForSeconds(delay);
        line.StopYarnMove(slotToHoleDuration);
    }


}
