using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Pack : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    public bool IsCompleted;


    public void DoFill(Transform from, GameColor color, Action complte)
    {
        renderer.gameObject.SetActive(true);
        UpdateVisual(color);
        transform.position = from.position;

        transform.DOLocalRotateQuaternion(Quaternion.identity, 0.25f);
        transform.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() =>
        {
            complte?.Invoke();
        });
    }

    [Button]
    private void UpdateVisual(GameColor color)
    {
        renderer.sharedMaterial = DataConfigs.instance.GetColorConfig(color).material;
    }

}
