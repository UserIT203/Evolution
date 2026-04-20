using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveStage
{
    [SerializeField] public List<UnitStage> StageUnits;
    [SerializeField] public float Delay;
}

[System.Serializable]
public class UnitStage
{
    public UnitType UnitType;
    public int Count;
}
