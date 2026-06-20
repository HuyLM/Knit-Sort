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
            if(segment.Unloader != null)
            {
                segment.Unloader.RemoveBlock(movingSlot);
            }
        }
        else
        {
            Piece block = movingSlot.Block;

            var container = segment.GetContainer(block.Color);
            if (container == null)
            {
                return;
            }
            movingSlot.MakeEmpty();
            block.JumpToContainer(container);
        }
    }

    public int FilledMovingSlotCount()
    {
        return _slots.Count(slot => slot.IsEmpty() == false);
    }
}
