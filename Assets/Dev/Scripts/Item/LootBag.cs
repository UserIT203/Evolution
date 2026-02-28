using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(IDamagaeble))]
public class LootBag : MonoBehaviour
{
    [SerializeField] private bool _hasCreateLootOnTakeDamage;
    [SerializeField] private List<ItemInLootBag> _items;

    private ItemBase[] _droppedItem;
    private ItemUseContext _context;

    private IDamagaeble _unit;

    private void Awake()
    {
        _unit = GetComponent<IDamagaeble>();
        _unit.onDie += CreateItem;

        if (_hasCreateLootOnTakeDamage)
            _unit.onTakeDamage += CreateItem;

        _droppedItem = new ItemBase[_items.Count];
        int randomValue = Random.Range(0, 100);
        int currentItem = 0;

        _items.OrderBy(i => i.Probability);

        foreach (ItemInLootBag item in _items)
        {
            if(item.Probability >= randomValue)
            {
                _droppedItem[currentItem] = item.Item;
                currentItem++;
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
            item.Use(_context, transform.localPosition);
        }
    }

    public void CreateItem(float value)
    {
        foreach (var item in _droppedItem)
        {
            item.Use(_context, transform.localPosition);
        }
    }
}

[System.Serializable]
public struct ItemInLootBag
{
    public ItemBase Item;
    [Range(1, 100)]
    public int Probability;
}