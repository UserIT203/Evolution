using UnityEngine;
using System.Collections.Generic;

public class LevelSpawnConfig : ScriptableObject
{
    [SerializeField] private List<GameObjectList> _typeObjects = new List<GameObjectList>();
    
    public Dictionary<LevelTypeObject, List<GameObject>> TypeObjects
    {
        get
        {
            var dict = new Dictionary<LevelTypeObject, List<GameObject>>();

            foreach (var obj in _typeObjects)
            {
                dict.Add(obj.Type, obj.Objects);
            }

            return dict;
        }
    }

    public void SetData( Dictionary<LevelTypeObject, List<GameObject>> typeObjects)
    {
        _typeObjects.Clear();

        foreach (var kvp in typeObjects)
        {
            _typeObjects.Add(new GameObjectList
            {
                Type = kvp.Key,
                Objects = new List<GameObject>(kvp.Value)
            });
        }
    }
}

[System.Serializable]
public class GameObjectList
{
    public LevelTypeObject Type;
    public List<GameObject> Objects;
}
