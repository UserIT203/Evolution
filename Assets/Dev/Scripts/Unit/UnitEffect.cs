using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDamagaeble))]
public class UnitEffect : MonoBehaviour
{
    [System.Serializable]
    private struct Effect
    {
        public string Name;
        public Vector3 Offset;
        public ParticleSystem Particle;
    }

    [Header("<color=yellow>Coin Effect</color>")]
    [SerializeField] private CoinViewUI _coinViewUI;
    [SerializeField] private Vector3 _spawnOffset;
    
    [Header("<color=red>Particle Effects</color>")]
    [SerializeField] private List<Effect> _effects;

    private IDamagaeble _unit;
    private Dictionary<string, ParticleSystem> _particleDictianory = new();

    private void OnEnable()
    {
        _unit = GetComponent<IDamagaeble>();

        _unit.onTakeDamage += PlayHitEffect;
    }

    private void OnDisable()
    {
        _unit.onTakeDamage -= PlayHitEffect;
    }

    private void Awake()
    {
        InitializedEffects();
    }

    public void PlayEffect(string name)
    {
        _particleDictianory[name].Play();
    }

    public void CreateCoinView(int amount)
    {
        if (_coinViewUI == null) return;

        CoinViewUI view = Instantiate(_coinViewUI);

        view.transform.position = transform.position + _spawnOffset;

        view.PlayAnimation(amount);
    }

    private void InitializedEffects()
    {
        foreach (Effect effect in _effects)
        {
            ParticleSystem particle = Instantiate(
                effect.Particle, 
                effect.Offset, 
                Quaternion.identity) 
                as ParticleSystem;

            particle.transform.SetParent(transform, false);

            _particleDictianory.Add(effect.Name, particle);
        }
    }

    private void PlayHitEffect(float value)
    {
        PlayEffect("hit");
    }
}
