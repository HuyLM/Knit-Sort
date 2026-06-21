using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public GameColor Color;
    public Transform Out;

    [SerializeField] private Renderer[] pastaRenderers;
     private void UpdateVisual()
    {
        foreach (var renderer in pastaRenderers)
        {
            renderer.sharedMaterial = DataConfigs.instance.GetColorConfig(Color).material;
        }
    }

    [Button]
    private void UpdateColorr()
    {
        UpdateVisual();
    }

    public void Selected(float removeTime, Action onCompleted)
    {
        StartCoroutine(PlayRemovePasta(removeTime, onCompleted));
    }

    private IEnumerator PlayRemovePasta(float removeTime, Action onCompleted)
    {
        float deltaTime = removeTime / pastaRenderers.Length;
        for (int i = 0; i < pastaRenderers.Length; i++) {
            pastaRenderers[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(deltaTime);
        }
        onCompleted?.Invoke();
    }
}
