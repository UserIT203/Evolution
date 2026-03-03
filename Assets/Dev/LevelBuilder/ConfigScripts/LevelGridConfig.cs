using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGridConfig : ScriptableObject
{
    public int Row = 4; 
    public int Col = 6;

    public int CellSize;

    public List<int> CellColorIndex;
    public List<TypeColor> TypeColors;

    private void OnEnable()
    {
        if(TypeColors == null) TypeColors = new List<TypeColor>();
        if(CellColorIndex == null) CellColorIndex = new List<int>();

        if(TypeColors.Count <= 0)
        {
            TypeColor typeColor = new TypeColor { 
                ColorIndex = 0,
                ColorCell = Color.green, 
                ObjectType = LevelTypeObject.PlayerTower,
            };
            TypeColors.Add(typeColor);

            typeColor = new TypeColor {
                ColorIndex = 1,
                ColorCell = Color.red,
                ObjectType = LevelTypeObject.EnemyTower,
            };
            TypeColors.Add(typeColor);

            typeColor = new TypeColor
            {
                ColorIndex = 2,
                ColorCell = Color.yellow,
                ObjectType = LevelTypeObject.Props,
            };
            TypeColors.Add(typeColor);

            typeColor = new TypeColor
            {
                ColorIndex = 3,
                ColorCell = Color.grey,
                ObjectType = LevelTypeObject.Road,
            };
            TypeColors.Add(typeColor);

            typeColor = new TypeColor
            {
                ColorIndex = 4,
                ColorCell = Color.white,
                ObjectType = LevelTypeObject.Empty,
            };
            TypeColors.Add(typeColor);
        }

        if (CellColorIndex.Count <= 0)
        {
            for (int i = 0; i < Row * Col; i++)
            {
                CellColorIndex.Add(0);
                SetColor(i, Color.white);
            }
                
        }
    }

    public void ReziseGrid()
    {
        CellColorIndex.Clear();

        for (int i = 0; i < Row * Col; i++)
        {
            CellColorIndex.Add(0);
            SetColor(i, Color.white);
        }  
    }

    public List<LevelTypeObject> GetCellObjects()
    {
        List<LevelTypeObject> cellObject = new List<LevelTypeObject>();

        foreach (var color in CellColorIndex)
        {
            LevelTypeObject obj = TypeColors.Find(o => o.ColorIndex == color).ObjectType;
            cellObject.Add(obj);
        }

        return cellObject;
    }

    public void SetColor(int index, Color color)
    {
        CellColorIndex[index] = TypeColors.Find(item => item.ColorCell == color).ColorIndex;
    }

    public Color GetColor(int index)
    {
        int colorIndex = CellColorIndex[index];
        return TypeColors.Find(item => item.ColorIndex == colorIndex).ColorCell;
    }
}

[System.Serializable]
public struct TypeColor
{
    public int ColorIndex;
    public Color ColorCell;
    public LevelTypeObject ObjectType;
}