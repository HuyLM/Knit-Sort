using TMPro;
using UnityEngine;

[System.Serializable]
public class HoleData
{
    public int Number;
    public GameColor Color;
}

public class Hole : MonoBehaviour
{
    public Renderer _rend;
    public Transform inPos;
    public TextMeshPro text;
    public HoleData[] HoleDatas;

    public int curDataIndex = 0;

    private void Start()
    {
        UpdatVisual();
    }

    public HoleData CurData()
    {
        if(curDataIndex >= HoleDatas.Length)
        {
            return null;
        }
        return HoleDatas[curDataIndex];
    }

    public void Add()
    {
        var curData = CurData();
        if (curData == null)
        {
            return;
        }
        curData.Number--;
        if (curData.Number <= 0)
        {
            curDataIndex++;
        }
        if (curDataIndex >= HoleDatas.Length)
        {
            gameObject.SetActive(false);
            return;
        }
        UpdatVisual();
    }
        
    private void UpdatVisual()
    {
        var curData = CurData();
        if (curData == null)
        {
            return;
        }
        text.text = curData.Number .ToString();
        _rend.sharedMaterial = DataConfigs.instance.GetColorConfig(curData.Color).material;
    }
}
