using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGridConfig : ScriptableObject
{
    public int Row; 
    public int Col;

    public int CellSize;

    public List<Color> CellColor;
    public List<TypeColor> TypeColors;

    private void OnEnable()
    {
        if(CellColor.Count <= 0)
        {
            for (int i = 0; i < Row * Col; i++)
                CellColor.Add(Color.white);
        }

        if(TypeColors.Count <= 0)
        {
            TypeColor typeColor = new TypeColor { 
                ColorCell = Color.green, 
                ObjectType = LevelTypeObject.PlayerTower,
            };
            TypeColors.Add(typeColor);

            typeColor = new TypeColor { 
                ColorCell = Color.red,
                ObjectType = LevelTypeObject.EnemyTower,
            };
            TypeColors.Add(typeColor);

            typeColor = new TypeColor
            {
                ColorCell = Color.yellow,
                ObjectType = LevelTypeObject.Props,
            };
            TypeColors.Add(typeColor);

            typeColor = new TypeColor
            {
                ColorCell = Color.grey,
                ObjectType = LevelTypeObject.Road,
            };
            TypeColors.Add(typeColor);
        }
    }

    public void ReziseGrid()
    {
        CellColor.Clear();

        for (int i = 0; i < Row * Col; i++)
            CellColor.Add(Color.white);
    }

    public List<LevelTypeObject> GetCellObjects()
    {
        List<LevelTypeObject> cellObject = new List<LevelTypeObject>();

        foreach (var color in CellColor)
        {
            LevelTypeObject obj = TypeColors.Find(o => o.ColorCell == color).ObjectType;
            cellObject.Add(obj);
        }

        return cellObject;
    }
}

[System.Serializable]
public struct TypeColor
{
    public Color ColorCell;
    public LevelTypeObject ObjectType;
}
