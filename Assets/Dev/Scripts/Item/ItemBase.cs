using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items")]
public class ItemBase : ScriptableObject
{
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private ItemHandlerType _handlerType;
    [SerializeField] private Vector3 _spawnOffset;

    public virtual void Use(ItemUseContext context, int count)
    {
        context.GetHandler(_handlerType)?.AddCoin(count);
    }  
}
