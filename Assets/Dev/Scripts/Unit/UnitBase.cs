using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class UnitBase : Poolable, IDamagaeble, IEffectAction
{
    [Header("<color=green><b>Main Settings</b></color>")]
    [SerializeField] protected UnitStatsConfig _unitConfig;
    [field: SerializeField] public string VictimTag;
    [field: SerializeField] public LayerMask VictimLayer;

    private Transform _targetBase;
    private EffectManager _effectManager;

    protected CustomPool<Bullet> _bulletPool;
    protected IDamagaeble _victim;
    protected NavMeshAgent _agent;
    protected FSM _fsm;
    protected GameManager _gameManager;

    protected UnitStats _unitStats;

    public float CurrentHealth { get; private set; }
    public float CurrentArmor { get; private set; }

    public IDamagaeble Victim => _victim;
    public UnitStats UnitStats => _unitStats;
    public UnitStatsConfig UnitConfig => _unitConfig;
    public Transform TargetBase => _targetBase;

    public Transform Transform { get => transform; set => Transform = value; }

    public event Action<float> onTakeDamage;
    public event Action<float> onReduceArmor;

    public event Action<float, float> onChangeHealth;
    public event Action<float, float> onChangeArmor;

    public event Action<string> onSetTarget;

    public event Action onDie;
    public event Action onAttack;

    private void OnDisable()
    {
        if(_gameManager != null)
            _gameManager.onEnd -= Restart;
        
        ClearVictim();
    }

    private void Awake()
    {
        _effectManager = new EffectManager(this);
        
        _unitStats = new UnitStats();
        _unitStats.SetValues(_unitConfig);
        Debug.Log(UnitStats.AttackRange.GetValue());

        Initialized();
    }

    private void LateUpdate()
    {
        _fsm?.LateUpdate();
    }

    private void Update()
    {
        _effectManager?.Update();
        _fsm?.Update();
    }

    private void Restart() => Release();

    private void ClearVictim()
    {
        if (_victim == null) return;

        onSetTarget?.Invoke("Empty");

        _victim.onDie -= ClearVictim;
        _victim = null;
     }

    protected abstract void InitializedFSM();

    protected virtual void Initialized()
    {
        _fsm = new FSM();
        _agent = GetComponent<NavMeshAgent>();
        
        InitializedFSM();
    }

    public void Spawn(SpawnContext context)
    {
        _targetBase = context.TowerTransform;
        _bulletPool = context.BulletPool;
        _gameManager = context.GameManager;

        _unitStats.ApplyMultiplier(context.GlobalManager);

        CurrentHealth = _unitStats.MaxHealth.GetValue();
        CurrentArmor = _unitStats.Armor.GetValue();

        _gameManager.onEnd += Restart;

        onChangeHealth?.Invoke(_unitStats.MaxHealth.GetValue(), CurrentHealth);
    }

    public void SetVictim(IDamagaeble target)
    {
        _victim = target;

        onSetTarget?.Invoke(target.ToString());

        if (_victim != null)
            _victim.onDie += ClearVictim;
    }

    public CustomPool<Bullet> GetBulletPool() => _bulletPool;

    public void AddEffect(Effect effect) => _effectManager.AddEffect(effect);

    public void Attack() => onAttack?.Invoke();

    #region FSM Transition
    protected virtual bool HasAttack()
    {
        if (_victim == null)
            return false;

        if (_agent.remainingDistance <= _agent.stoppingDistance)
            return true;

        return false;
    }

    protected virtual bool HasSearchTarget()
    {
        return _victim == null;
    }

    protected virtual bool HasFollowTarget()
    {
        if(_victim == null)
            return false;

        Vector3 victimDistance = _victim.Transform.position - transform.position;

        if (victimDistance.sqrMagnitude > _unitStats.AttackRange.GetValue())
            return true;

        return false;
    }

    #endregion

    #region Damagaeble Methods
    public void TakeDamage(float damage)
    {        
        float damageReduction = CurrentArmor / 100f;
        CurrentArmor = Mathf.Clamp(CurrentArmor - damage, 0, _unitStats.Armor.GetValue());

        float reduceDamage = damage * (1 - damageReduction);
        reduceDamage = Math.Clamp(reduceDamage, 0, reduceDamage);

        float predictHealth = CurrentHealth - reduceDamage;
        
        if(predictHealth > 0)
        {
            CurrentHealth = predictHealth;
        }
        else
        {
            CurrentHealth = 0;
            Die();
        }

        AudioManager.PlaySound("Hit");

        onChangeHealth?.Invoke(_unitStats.MaxHealth.GetValue(), CurrentHealth);
        onChangeArmor?.Invoke(_unitStats.Armor.GetValue(), CurrentArmor);

        onTakeDamage?.Invoke(reduceDamage);
        onReduceArmor?.Invoke(damage);   
    }

    public void Die()
    {
        onDie?.Invoke();
        Release();
    }

    #endregion

    #region Effect Methods
    public void FreezeAction(float speedValue, float damageValue)
    {
        Debug.Log($"Speed Value {speedValue} {this.name}");

        _unitStats.Speed.BaseValue = speedValue;
        _unitStats.Damage.BaseValue = damageValue;
    }

    public void ShieldAction(float shieldValue)
    {
        _unitStats.Armor.BaseValue = shieldValue;
        CurrentArmor = shieldValue;

        onChangeArmor?.Invoke(CurrentArmor, _unitStats.Armor.GetValue());
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_victim != null)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.yellow;

        if(_unitStats != null)
            Gizmos.DrawWireSphere(transform.position, _unitStats.AttackRange.GetValue());
    }
#endif
}
