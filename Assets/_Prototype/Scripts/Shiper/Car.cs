using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Renderer renderer;

    public GameColor Color;
    public List<Pack> Packs;

    private bool isMoving;

    public bool Ready;
    public Action OnCompleted;

    public bool IsCompleted
    {
        get
        {
           foreach(var p in Packs)
            {
                if (p.IsCompleted == false)
                    return false;
            }
            return true;
        }
    }

    [Button]
    private void UpdateVisual()
    {
        renderer.sharedMaterial = DataConfigs.instance.GetColorConfig(Color).carMat;
    }

    public Pack GetEmptySlot()
    {
        for (int i = 0; i < Packs.Count; ++i)
        {
            if (Packs[i].IsCompleted == false) return Packs[i];
        }
        return null;
    }

    public void CheckMove()
    {
        if(IsCompleted)
        {
            MoveOUt();
        }
    }

    private void MoveOUt()
    {
        if(isMoving)
        {
            return;
        }
        isMoving = true;
        transform.DOMove(transform.up * 1000, 100).SetSpeedBased(true).SetEase(Ease.InSine).OnComplete(() => {
        });
        OnCompleted?.Invoke();
    }
}
