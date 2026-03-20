using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitBase))]
public class UnitEffect : MonoBehaviour
{
    [System.Serializable]
    private struct Effect
    {
        public string Name;
        public Vector3 Offset;
        public ParticleSystem Particle;
    }

    [SerializeField] private List<Effect> _effects;

    private UnitBase _unit;
    private Dictionary<string, ParticleSystem> _particleDictianory = new();

    private void OnEnable()
    {
        _unit = GetComponent<UnitBase>();

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

    public void PlayEffect(string name)
    {
        _particleDictianory[name].Play();
    }
}
