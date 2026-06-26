using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Tun : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private Transform ready;
    public List<Car> Cars;

    private int curIndex;

    private void Start()
    {
        foreach(var c in Cars)
        {
            c.OnCompleted = CarCompleted;
        }
        NextToReady();
    }

    private void CarCompleted()
    {
        curIndex++;
        NextToReady();
    }

    private void NextToReady()
    {
        if(curIndex >= Cars.Count)
        {
            return;
        }
        var car = Cars[curIndex];
        car.gameObject.SetActive(true);
        car.Ready = true;
        car.transform.DOMove(ready.position, 0.5f);
        int nextPrepare = curIndex + 1;
        if (nextPrepare >= Cars.Count)
        {
            return;
        }
        var prepare = Cars[nextPrepare];
        prepare.gameObject.SetActive(true);
    }
}
