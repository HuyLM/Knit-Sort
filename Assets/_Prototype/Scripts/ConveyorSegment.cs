using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSegment : MonoBehaviour
{
    public Car[] Cars;
    public bool IsGuide; // Vị trí thêm len mới vào băng chuyền (vị trí 1)
    public Unloader Unloader; 
    public SplineComputer spline;


    public Car GetContainer(GameColor color)
    {
        for (int i = 0; i < Cars.Length; i++)
        {
            if (Cars[i] == null)
            {
                continue;
            }
            if (Cars[i].Color == color && Cars[i].Ready)
            {
                return Cars[i];
            }
        }
        return null;
    }

   
}
