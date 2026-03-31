using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(UnitEffect))]
[RequireComponent(typeof(IDamagaeble))]
public class LootBag : MonoBehaviour
{
    [SerializeField] private bool _hasCreateLootOnTakeDamage;
    [SerializeField] private List<ItemInLootBag> _items;

    private Dictionary<ItemBase, int> _droppedItem;
    private ItemUseContext _context;

    private UnitEffect _unitEffect;
    private IDamagaeble _unit;

    private void Awake()
    {
        _unitEffect = GetComponent<UnitEffect>();
        _unit = GetComponent<IDamagaeble>();
        _unit.onDie += CreateItem;

        if (_hasCreateLootOnTakeDamage)
            _unit.onTakeDamage += CreateItem;

        _droppedItem = new Dictionary<ItemBase, int>();
        int randomValue = Random.Range(0, 100);

        _items.OrderBy(i => i.Probability);

        foreach (ItemInLootBag item in _items)
        {
            if(item.Probability >= randomValue)
            {
                _droppedItem.Add(item.Item, item.ItemCount);
            }
        }
    }

    public void Initialized(ItemUseContext context)
    {
        _context = context;
    }

    public void CreateItem()
    {
        if (_droppedItem == null) return;

        foreach (var item in _droppedItem)
        {
            item.Key.Use(_context, item.Value);
            _unitEffect.CreateCoinView(item.Value);
        }
    }

    public void CreateItem(float value)
    {
        foreach (var item in _droppedItem)
        {
            item.Key.Use(_context, item.Value);
        }
    }
}

[System.Serializable]
public struct ItemInLootBag
{
    public ItemBase Item;
    [Range(0, 1000)]
    public int ItemCount;
    [Range(1, 100)]
    public int Probability;
}