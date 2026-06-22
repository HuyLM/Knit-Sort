using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public GameColor Color;
    public Transform Out;
    public bool Clickable;

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
public void ConsumeLevel()
    {
        if (_remainingLevels <= 0) return;

        int usedIndex = pastaLevels.Length - _remainingLevels; // pastaLevels[0] là lớp trên cùng, ẩn trước
        if (usedIndex >= 0 && usedIndex < pastaLevels.Length)
        {
            foreach (var renderer in pastaLevels[usedIndex].pastaRenderers)
            {
                renderer.gameObject.SetActive(false);
            }
        }

        _remainingLevels--;

        if (_remainingLevels <= 0)
        {
            Clickable = false;
            // KHÔNG tự SetActive(false) ở đây — báo ra ngoài qua event để PlateController
            // có cơ hội chạy animation (scale nhỏ) trước khi thực sự tắt GameObject.
            OnLevelEmpty?.Invoke(this);
        }
    }

/// <summary>
    /// Kiểm tra Plate có đang Clickable không. Trả về true nếu có thể tiến hành chọn.
    /// KHÔNG tự chạy coroutine (để tránh bị hủy nửa chừng nếu Plate bị SetActive(false)
    /// trong lúc đang chờ) — phần chờ/thực thi callback do nơi gọi (GameManager) tự quản lý.
    /// </summary>
    public bool Selected()
    {
        return Clickable;
    }


}
