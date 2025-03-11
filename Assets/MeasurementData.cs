using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeasurementData
{
    public Vector2 pointA;
    public Vector2 pointB;

    public int outlierCount;
    public int inlierCount;
}
