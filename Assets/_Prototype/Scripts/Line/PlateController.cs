using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý hàng Plate hiển thị cho người chơi chọn.
/// Tối đa <see cref="max"/> Plate được hiển thị cùng lúc, xếp ngang theo trục X
/// bắt đầu từ <see cref="startPoint"/>, cách nhau <see cref="spacing"/>.
/// Khi 1 Plate trong hàng hết level (OnLevelEmpty), nó sẽ:
///   1) Chạy animation scale nhỏ (shrinkScale) rối ẩn đi.
///   2) Các Plate bên phải dịch trái lấp chỗ trống.
///   3) Lấy 1 Plate mới từ hàng chờ (queue) trượt vào vị trí cuối cùng.
/// </summary>
public class PlateController : MonoBehaviour
{
    [Header("Dữ liệu")]
    public List<Plate> plates;

    [Header("Vị trí hàng ngang (trục X)")]
    public int max = 4;
    [SerializeField] private Transform startPoint;
    [SerializeField] private float spacing = 1.5f;

    [Header("Animation")]
    [SerializeField] private float shrinkScale = 0.25f;
    [SerializeField] private float shrinkDuration = 0.2f;
    [SerializeField] private float slideDuration = 0.2f;
    [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Các Plate đang hiện diện trong hàng (tối đa `max` phần tử), theo đúng thứ tự vị trí 0..max-1
    private readonly List<Plate> _activeRow = new List<Plate>();

    // Các Plate chưa được đưa vào hàng, chờ theo đúng thứ tự trong `plates`
    private readonly Queue<Plate> _waitingQueue = new Queue<Plate>();

    private bool _isShifting; // true trong lúc đang chạy animation xóa + đẩy hàng, tránh chồng chéo

    private void Awake()
    {
        _activeRow.Clear();
        _waitingQueue.Clear();

        for (int i = 0; i < plates.Count; i++)
        {
            Plate plate = plates[i];
            if (plate == null) continue;

            if (_activeRow.Count < max)
            {
                _activeRow.Add(plate);
            }
            else
            {
                _waitingQueue.Enqueue(plate);
            }
        }

        // Đặt đúng vị trí cho `max` Plate đầu tiên, kích hoạt và cho phép click
        for (int i = 0; i < _activeRow.Count; i++)
        {
            SetupActivePlate(_activeRow[i], i);
        }

        // Các Plate còn lại trong queue: giợ nguyên vị trí hiện có trong scene, chỉ tắt đi
        foreach (var plate in _waitingQueue)
        {
            plate.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Đặt 1 Plate vào đúng vị trí index trong hàng (0..max-1), kích hoạt,
    /// cho phép click, và subscribe event OnLevelEmpty của nó.
    /// </summary>
    private void SetupActivePlate(Plate plate, int index)
    {
        plate.gameObject.SetActive(true);
        plate.transform.position = GetSlotPosition(index);
        plate.Clickable = true;

        plate.OnLevelEmpty -= HandlePlateLevelEmpty; // tránh subscribe trùng lặp
        plate.OnLevelEmpty += HandlePlateLevelEmpty;
    }

private Vector3 GetSlotPosition(int index)
    {
        float y = startPoint != null ? startPoint.position.y : 0f;
        float z = startPoint != null ? startPoint.position.z : 0f;

        // Đối xừng qua X = 0 (gốc world): slot giởa khoảng (max-1)/2.0
        float x = (index - (max - 1) / 2f) * spacing;

        return new Vector3(x, y, z);
    }

    private void HandlePlateLevelEmpty(Plate plate)
    {
        if (_isShifting) return; // an toàn: tránh trigger trùng trong cùng 1 frame
        StartCoroutine(RemoveAndShift(plate));
    }

    private IEnumerator RemoveAndShift(Plate plate)
    {
        _isShifting = true;

        plate.OnLevelEmpty -= HandlePlateLevelEmpty;

        int removedIndex = _activeRow.IndexOf(plate);

        // --- Pha 1: scale nhỏ Plate vừa hết level ---
        yield return ScalePlate(plate.transform, Vector3.one, Vector3.one * shrinkScale, shrinkDuration);

        plate.gameObject.SetActive(false);
        plate.transform.localScale = Vector3.one; // trả về scale gốc để lần sau dùng lại không bị nhỏ

        if (GameManager.instance != null)
        {
            GameManager.instance.EndConveyor();
        }

        if (removedIndex >= 0)
        {
            _activeRow.RemoveAt(removedIndex);
        }

        // --- Pha 2: lấy 1 Plate mới từ queue (nếu còn), đặt ở vị trí ngay sau cùng hiện tại ---
        Plate newPlate = null;
        if (_waitingQueue.Count > 0)
        {
            newPlate = _waitingQueue.Dequeue();
            newPlate.gameObject.SetActive(true);
            newPlate.Clickable = false; // chỉ cho click sau khi trượt vào đúng vị trí
            // Đặt Plate mới ở vị trí ngay phía phải của hàng hiện tại (slot thừa, tạm thời)
            newPlate.transform.position = GetSlotPosition(_activeRow.Count + 1);
            _activeRow.Add(newPlate);
        }

        // --- Pha 3: đẩy toàn bộ Plate còn lại (và Plate mới, nếu có) trượt về đúng vị trí 0..n-1 ---
        var moveTargets = new Vector3[_activeRow.Count];
        var moveStarts = new Vector3[_activeRow.Count];
        for (int i = 0; i < _activeRow.Count; i++)
        {
            moveStarts[i] = _activeRow[i].transform.position;
            moveTargets[i] = GetSlotPosition(i);
        }

        yield return SlideAll(_activeRow, moveStarts, moveTargets, slideDuration);

        if (newPlate != null)
        {
            newPlate.Clickable = true;
            newPlate.OnLevelEmpty -= HandlePlateLevelEmpty;
            newPlate.OnLevelEmpty += HandlePlateLevelEmpty;
        }

        _isShifting = false;
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

    private IEnumerator SlideAll(List<Plate> targets, Vector3[] starts, Vector3[] ends, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float rawT = Mathf.Clamp01(elapsed / duration);
            float t = slideEase.Evaluate(rawT);

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null) continue;
                targets[i].transform.position = Vector3.Lerp(starts[i], ends[i], t);
            }
            yield return null;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null) continue;
            targets[i].transform.position = ends[i];
        }
    }
}
