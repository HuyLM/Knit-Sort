using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý băng trôi các Plate hiển thị cho người chơi chọn.
///
/// - Các Plate đang ACTIVE trôi liên tục từ right sang left (driftSpeed).
/// - Mỗi Plate (trừ Plate đầu hàng) liên tục Lerp khoảng cách hiện tại với Plate đỬng trước
///   nó về đúng "spacing" (cố định) — khi có chỗ trống (do Plate khác bị ẩn) các Plate
///   phía sau sẽ từ từ ép lại gần cho đủ spacing.
/// - Khi 1 Plate active chạm tới left: nếu khoảng cách với Plate đỬng trước nó CHƯ A đạt
///   đủ spacing thì bị ẩn đi (không Destroy), chuyển vào hàng chờ (waiting queue, FIFO).
/// - Plate đầu hàng chờ được spawn lại tại right ngay khi Plate active gần right nhất
///   hiện tại đã cách right đủ spacing (tức đã có chỗ trống).
/// </summary>
public class PlateController : MonoBehaviour
{
    [Header("Dữ liệu")]
    public List<Plate> plates;

    [Header("Biên trôi (cố định)")]
    [SerializeField] private Transform right;
    [SerializeField] private Transform left;

    [Header("Khoảng cách & tốc độ")]
    [SerializeField] private float spacing = 1.5f;     // khoảng cách yêu cầu giởa 2 Plate liền kề (cố định)
    [SerializeField] private float driftSpeed = 1f;    // units/giây, trôi từ right sang left
    [SerializeField] private float closeGapSpeed = 4f; // tốc độ Lerp ép khoảng cách về đúng spacing

    [Header("Animation khi hết level")]
    [SerializeField] private float shrinkScale = 0.25f;
    [SerializeField] private float shrinkDuration = 0.2f;

    // Hàng Plate đang ACTIVE (trôi trên màn hình), theo đúng thứ tự 0 = gần right nhất
    private readonly List<Plate> _activeRow = new List<Plate>();

    // Hàng chờ (FIFO) các Plate đang bị ẩn, chờ đủ chỗ để spawn lại ở right
    private readonly Queue<Plate> _waitingQueue = new Queue<Plate>();

private void Awake()
    {
        _activeRow.Clear();
        _waitingQueue.Clear();

        for (int i = 0; i < plates.Count; i++)
        {
            Plate plate = plates[i];
            if (plate == null) continue;

            plate.OnLevelEmpty -= HandlePlateLevelEmpty;
            plate.OnLevelEmpty += HandlePlateLevelEmpty;

            if (i == 0)
            {
                // Chỉ Plate đầu tiên active ngay lúc bắt đầu, tại đúng vị trí right.
                plate.gameObject.SetActive(true);
                plate.Clickable = true;
                plate.transform.position = new Vector3(right.position.x, right.position.y, right.position.z);
                _activeRow.Add(plate);
            }
            else
            {
                // Các Plate còn lại đều vào hàng chờ, sẽ tữ spawn dần theo điều kiện khoảng cách.
                plate.gameObject.SetActive(false);
                plate.Clickable = false;
                _waitingQueue.Enqueue(plate);
            }
        }
    }

private void Update()
    {
        if (right == null || left == null) return;
        if (_activeRow.Count == 0)
        {
            TrySpawnFromQueue();
            return;
        }

        float dt = Time.deltaTime;
        float xLeft = left.position.x;
        float y = right.position.y;
        float z = right.position.z;

        // 1) Trôi đều sang trái
        for (int i = 0; i < _activeRow.Count; i++)
        {
            Plate plate = _activeRow[i];
            if (plate == null) continue;

            Vector3 pos = plate.transform.position;
            pos.x -= driftSpeed * dt;
            pos.y = y;
            pos.z = z;
            plate.transform.position = pos;
        }

        // 2) Sắp xếp theo vị trí X THỌC TẠ (giảm dần: gần right -> gần left), KHÔNG dửa vào
        //    thứ tự trong _activeRow (thứ tự đó bị xáo sau khi ẩn/spawn nhiều lần, không đáng tin).
        var ordered = new List<Plate>(_activeRow);
        ordered.Sort((a, b) => b.transform.position.x.CompareTo(a.transform.position.x));

        // 3) Mỗi Plate (trừ Plate đầu hàng — gần right nhất trong thứ tự đã sort) Lerp khoảng
        //    cách với Plate liền trước nó (theo thứ tự sort) về đúng spacing. Việc này khiến
        //    hàng tự ép lại sau khi 1 Plate ở giởa bị ẩn, lấp khoảng trống mềm mại.
        for (int i = 1; i < ordered.Count; i++)
        {
            Plate plate = ordered[i];
            Plate prev = ordered[i - 1];
            if (plate == null || prev == null) continue;

            float desiredX = prev.transform.position.x - spacing;
            float currentX = plate.transform.position.x;

            if (currentX > desiredX)
            {
                float newX = Mathf.MoveTowards(currentX, desiredX, closeGapSpeed * dt);
                Vector3 pos = plate.transform.position;
                pos.x = newX;
                plate.transform.position = pos;
            }
        }

        // 4) BẤT KỲ Plate nào đã chạm left đều bị ẩn ngay và đưa vào hàng chờ — không cần
        //    kiểm tra khoảng cách gì cả (điều kiện khoảng cách chỉ áp dụng cho lúc SPAWN ở right).
        for (int i = ordered.Count - 1; i >= 0; i--)
        {
            Plate plate = ordered[i];
            if (plate == null) continue;
            if (plate.transform.position.x > xLeft) continue; // chưa chạm left

            HidePlate(plate);
            ordered.RemoveAt(i); // tránh xét lại Plate đã ẩn trong cùng frame
        }

        // 5) Nếu có Plate đang chờ, kiểm tra điều kiện để spawn lại ở right.
        TrySpawnFromQueue();
    }

    private void TrySpawnFromQueue()
    {
        if (_waitingQueue.Count == 0) return;
        if (right == null) return;

        bool canSpawn;
        if (_activeRow.Count == 0)
        {
            canSpawn = true;
        }
        else
        {
            // Plate gần right nhất trong thời điểm hiện tại (X lớn nhất)
            float maxX = float.NegativeInfinity;
            for (int i = 0; i < _activeRow.Count; i++)
            {
                if (_activeRow[i] == null) continue;
                float x = _activeRow[i].transform.position.x;
                if (x > maxX) maxX = x;
            }

            float distFromRight = right.position.x - maxX;
            canSpawn = distFromRight >= spacing - 0.001f;
        }

        if (canSpawn)
        {
            Plate newPlate = _waitingQueue.Dequeue();
            SpawnAtRight(newPlate);
        }
    }

    private void SpawnAtRight(Plate plate)
    {
        Vector3 pos = new Vector3(right.position.x, right.position.y, right.position.z);
        plate.transform.position = pos;
        plate.transform.localScale = Vector3.one;
        plate.gameObject.SetActive(true);
        plate.Clickable = true;

        _activeRow.Add(plate);
    }

    private void HidePlate(Plate plate)
    {
        _activeRow.Remove(plate);
        plate.Clickable = false;
        plate.gameObject.SetActive(false);
        _waitingQueue.Enqueue(plate);
    }

    private void HandlePlateLevelEmpty(Plate plate)
    {
        StartCoroutine(RemovePlate(plate));
    }

    private IEnumerator RemovePlate(Plate plate)
    {
        plate.OnLevelEmpty -= HandlePlateLevelEmpty;

        // Xóa khỏi hàng active (nếu đang active) — tạo chỗ trống ngay để các Plate sau ép lại.
        _activeRow.Remove(plate);

        yield return ScalePlate(plate.transform, Vector3.one, Vector3.one * shrinkScale, shrinkDuration);

        plate.gameObject.SetActive(false);
        plate.transform.localScale = Vector3.one;

        if (GameManager.instance != null)
        {
            GameManager.instance.EndConveyor();
        }
    }

    private IEnumerator ScalePlate(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        target.localScale = to;
    }
}



