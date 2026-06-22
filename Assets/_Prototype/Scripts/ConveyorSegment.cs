using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSegment : MonoBehaviour
{
    public Hole[] Holes;
    public bool IsGuide; // Vị trí thêm len mới vào băng chuyền (vị trí 1)
    public Unloader Unloader; 
    public SplineComputer spline;


    public Hole GetContainer(GameColor color)
    {
        for (int i = 0; i < Holes.Length; i++)
        {
            var holeData = Holes[i].CurData();
            if(holeData == null)
            {
                continue;
            }
            if (holeData.Color == color && holeData.Number > 0)
            {
                return Holes[i];
            }
        }
        return null;
    }

   
}
