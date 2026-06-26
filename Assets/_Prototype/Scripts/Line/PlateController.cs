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

    // Huong troi: +1 neu driftSpeed >= 0 (right -> left), -1 neu driftSpeed < 0 (left -> right).
    private int Dir => driftSpeed < 0f ? -1 : 1;

    // Diem Plate duoc spawn ra (noi "dau hang" dung, khong bi ep spacing).
    private Transform SpawnPoint => Dir > 0 ? right : left;

    // Diem Plate bi an/loop khi cham toi.
    private Transform EndPoint => Dir > 0 ? left : right;


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
                plate.transform.position = new Vector3(SpawnPoint.position.x, SpawnPoint.position.y, SpawnPoint.position.z);
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
        int dir = Dir;
        float xEnd = EndPoint.position.x;
        float y = right.position.y;
        float z = right.position.z;

        // 1) Troi deu theo huong dir (dir=+1: right->left, dir=-1: left->right)
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

        // 2) Sap xep theo thu tu tu SpawnPoint -> EndPoint (Plate dau hang = gan SpawnPoint nhat),
        //    KHONG dua vao thu tu trong _activeRow (thu tu do bi xao sau khi an/spawn nhieu lan).
        var ordered = new List<Plate>(_activeRow);
        if (dir > 0)
        {
            // right -> left: gan right nhat (X lon nhat) la dau hang
            ordered.Sort((a, b) => b.transform.position.x.CompareTo(a.transform.position.x));
        }
        else
        {
            // left -> right: gan left nhat (X nho nhat) la dau hang
            ordered.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        }

        // 3) Moi Plate (tru Plate dau hang) Lerp khoang cach voi Plate lien truoc no (theo thu tu sort)
        //    ve dung spacing. Viec nay khien hang tu ep lai sau khi 1 Plate o giua bi an.
        for (int i = 1; i < ordered.Count; i++)
        {
            Plate plate = ordered[i];
            Plate prev = ordered[i - 1];
            if (plate == null || prev == null) continue;

            float desiredX = prev.transform.position.x - spacing * dir;
            float currentX = plate.transform.position.x;

            // Plate dang "tut lai sau" so voi vi tri mong muon (xa hon EndPoint so voi desiredX)
            bool behind = dir > 0 ? currentX > desiredX : currentX < desiredX;
            if (behind)
            {
                float newX = Mathf.MoveTowards(currentX, desiredX, closeGapSpeed * dt);
                Vector3 pos = plate.transform.position;
                pos.x = newX;
                plate.transform.position = pos;
            }
        }

        // 4) BAT KY Plate nao da cham EndPoint deu bi an ngay va dua vao hang cho.
        for (int i = ordered.Count - 1; i >= 0; i--)
        {
            Plate plate = ordered[i];
            if (plate == null) continue;

            bool reachedEnd = dir > 0 ? plate.transform.position.x > xEnd : plate.transform.position.x < xEnd;
            if (reachedEnd) continue; // chua cham EndPoint

            HidePlate(plate);
            ordered.RemoveAt(i); // tranh xet lai Plate da an trong cung frame
        }

        // 5) Neu co Plate dang cho, kiem tra dieu kien de spawn lai o SpawnPoint.
        TrySpawnFromQueue();
    }

private void TrySpawnFromQueue()
    {
        if (_waitingQueue.Count == 0) return;
        if (right == null || left == null) return;

        Transform spawnPoint = SpawnPoint;
        int dir = Dir;

        bool canSpawn;
        if (_activeRow.Count == 0)
        {
            canSpawn = true;
        }
        else
        {
            // Plate gan SpawnPoint nhat trong thoi diem hien tai
            float bestX = dir > 0 ? float.NegativeInfinity : float.PositiveInfinity;
            for (int i = 0; i < _activeRow.Count; i++)
            {
                if (_activeRow[i] == null) continue;
                float x = _activeRow[i].transform.position.x;
                if (dir > 0 ? x > bestX : x < bestX) bestX = x;
            }

            float distFromSpawn = dir > 0 ? spawnPoint.position.x - bestX : bestX - spawnPoint.position.x;
            canSpawn = distFromSpawn >= spacing - 0.001f;
        }

        if (canSpawn)
        {
            Plate newPlate = _waitingQueue.Dequeue();
            SpawnAtStart(newPlate);
        }
    }

private void SpawnAtStart(Plate plate)
    {
        Transform spawnPoint = SpawnPoint;
        Vector3 pos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
        plate.transform.position = pos;
        //plate.transform.localScale = Vector3.one;
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
        RemovePlate(plate);
    }

    private void RemovePlate(Plate plate)
    {
        plate.OnLevelEmpty -= HandlePlateLevelEmpty;

        // Xóa khỏi hàng active (nếu đang active) — tạo chỗ trống ngay để các Plate sau ép lại.
        _activeRow.Remove(plate);

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



