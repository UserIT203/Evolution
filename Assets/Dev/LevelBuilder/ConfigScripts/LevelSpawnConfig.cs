using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

public class LevelSpawnConfig : ScriptableObject
{
    [SerializeField] private List<GameObjectList> _typeObjects = new List<GameObjectList>();
    
    public Dictionary<LevelTypeObject, List<AssetReferenceGameObject>> TypeObjects
    {
        get
        {
            var dict = new Dictionary<LevelTypeObject, List<AssetReferenceGameObject>>();

            foreach (var obj in _typeObjects)
            {
                dict.Add(obj.Type, obj.Objects);
            }

            return dict;
        }
    }

    public void SetData(Dictionary<LevelTypeObject, List<AssetReferenceGameObject>> typeObjects)
    {
        _typeObjects.Clear();

        foreach (var kvp in typeObjects)
        {
            _typeObjects.Add(new GameObjectList
            {
                Type = kvp.Key,
                Objects = new List<AssetReferenceGameObject>(kvp.Value)
            });
        }
    }
}

[System.Serializable]
public class GameObjectList
{
    public LevelTypeObject Type;
    public List<AssetReferenceGameObject> Objects;
}
