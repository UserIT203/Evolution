using System;
using UnityEngine;
using Zenject;

public class Tower : MonoBehaviour, IDamagaeble
{
    [Inject] private ItemManager _itemManager;
    [Inject] private GameManager _gameManager;

    [SerializeField] private Stat MaxHealth;
    [SerializeField] private TowerType _type;

    private LootBag _lootBag;
    private float _currentHealth;

    public Transform Transform { get => transform; set => Transform = value; }

    public event Action<float> onTakeDamage;
    public event Action<float> onReduceArmor;

    public event Action<float, float> onChangeHealth;
    public event Action<float, float> onChangeArmor;

    public event Action onDie;
    public event Action<string> onSetTarget;

    [Inject]
    public void Construct(ItemManager itemManager, GameManager gameManager)
    {
        _gameManager = gameManager;
        _itemManager = itemManager;

        _gameManager.onPlay += Initialized;
    }

    private void OnDestroy()
    {
        _gameManager.onPlay -= Initialized;
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
        _gameManager.EndGame(_type);
        onDie?.Invoke();
    }
}

public enum TowerType
{
    EnemyTower,
    PlayerTower
}