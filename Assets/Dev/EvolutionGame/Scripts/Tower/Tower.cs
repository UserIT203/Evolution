using System;
using UnityEngine;
using Zenject;

public class Tower : MonoBehaviour, IDamagaeble
{
    [SerializeField] private Stat MaxHealth;
    [SerializeField] private TowerType _type;

    private LootBag _lootBag;
    private float _currentHealth;

    private ItemManager _itemManager;
    

    public Transform Transform { get => transform; set => Transform = value; }
    public GameManager GameManager { get; set; }

    public event Action<float> onTakeDamage;
    public event Action<float> onReduceArmor;

    public event Action<float, float> onChangeHealth;
    public event Action<float, float> onChangeArmor;

    public event Action onDie;
    public event Action<string> onSetTarget;

    [Inject]
    public void Construct(ItemManager itemManager)
    {
        _itemManager = itemManager;
    }

    private void Start()
    {
        Initialized();

        if(_type == TowerType.EnemyTower)
        {
            _lootBag = GetComponent<LootBag>();
            _lootBag.Initialized(_itemManager.ItemContext);
        } 
    }

    private void Initialized()
    {
        _currentHealth = MaxHealth.GetValue();
        onChangeHealth?.Invoke(MaxHealth.GetValue(), _currentHealth);
    }

    public void TakeDamage(float damage)
    {
        float predictHealth = _currentHealth - damage;

        if(predictHealth > 0)
        {
            _currentHealth = predictHealth;
        }
        else
        {
            _currentHealth = 0;
            Die();
        }

        onChangeHealth?.Invoke(MaxHealth.GetValue(), _currentHealth);
        onTakeDamage?.Invoke(damage);
    }

    public void Die()
    {
        Debug.Log($"Tower defend {transform.name}");
        GameManager.EndGame(_type);
        onDie?.Invoke();
    }
}

public enum TowerType
{
    EnemyTower,
    PlayerTower
}