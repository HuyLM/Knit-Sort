using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public GameColor Color;
    public Transform Out;
    public bool Clickable;
    public bool Clicked;

    /// <summary>
    /// Bắn ra khi Plate đã dùng hết toàn bộ level (_remainingLevels <= 0).
    /// PlateController sẽ subscribe để chạy animation và đẩy hàng.
    /// </summary>
    public event Action<Plate> OnLevelEmpty;

    [SerializeField] private Level[] pastaLevels;

    [System.Serializable]
    public class Level
    {
        [SerializeField] public Renderer[] pastaRenderers;
    }

    // Số level (lớp pasta) còn lại. Mỗi lần add thành công 1 len vào băng chuyền
    // (MovingSlot.Add được gọi thờc sự) thì giảm đi 1 level, ẩn lớp trên cùng (index nhỏ nhất còn lại).
    [SerializeField] public int _remainingLevels = -1;

    private void Awake()
    {
        _remainingLevels = pastaLevels != null ? pastaLevels.Length : 0;
    }

    private void UpdateVisual()
    {
        foreach (var level in pastaLevels)
            foreach (var renderer in level.pastaRenderers)
            {
                renderer.sharedMaterial = DataConfigs.instance.GetColorConfig(Color).material;
            }
    }

    [Button]
    private void UpdateColorr()
    {
        UpdateVisual();
    }

    /// <summary>
    /// Gọi khi 1 len đã được add THÀNH CÔNG vào băng chuyền (MovingSlot.Add được gọi thực sự).
    /// Ẩn lớp pasta trên cùng còn lại; nếu hết level thì ẩn luôn toàn bộ Plate.
    /// </summary>

/// <summary>
    /// Kiểm tra Plate có đang Clickable không. Trả về true nếu có thể tiến hành chọn.
    /// KHÔNG tự chạy coroutine (để tránh bị hủy nửa chừng nếu Plate bị SetActive(false)
    /// trong lúc đang chờ) — phần chờ/thực thi callback do nơi gọi (GameManager) tự quản lý.
    /// </summary>
    public void Selected()
    {
        OnLevelEmpty?.Invoke(this);
    }

    Tween moving;
    public void MoveToPos(Vector3 target)
    {
        moving?.Kill();
        moving = transform.DOLocalMove(target, 0.25f).SetEase(Ease.OutQuad);
    }

    public bool JumpToContainer(Car container)
    {
        var containerSlot = container.GetEmptySlot();
        if (containerSlot == null) return false;

        containerSlot.IsCompleted = true;

        containerSlot.DoFill(this.transform, Color, () => {
            container.CheckMove();
        });
        gameObject.SetActive(false);
        return true;

        //transform.parent = containerSlot.transform;
        //transform.DOLocalRotateQuaternion(Quaternion.identity, 0.25f);
        //transform.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() => {
        //    container.CheckFull();
        //});
    }
}
