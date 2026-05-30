using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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

    [Space(5f)]
    [Header("<color=green>Tower Defeat Effect</color>")]
    [Header("Параметры падения")]
    [SerializeField] private float _fallDuration = 1.0f;
    [SerializeField] private float _tiltAngle = 80f;          
    [SerializeField] private Vector3 _tiltAxis = Vector3.forward; 
    [SerializeField] private float _dropHeight = 4f;         
    [SerializeField] private string _impactSound;

    [Header("Камера и звук")] 
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.5f;

    private Camera _mainCamera;
    private Sequence _collapseSeq;
    private Vector3 _startPos;
    private Quaternion _startRot;

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
        InitiliazedTowerEffect();
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

    public void PlayTowerDefeat(Action completedAction)
    {
        if (_collapseSeq != null && _collapseSeq.IsPlaying()) return;

        _collapseSeq = DOTween.Sequence();

        _collapseSeq.Append
            (
            transform.DORotate(_startRot.eulerAngles + _tiltAxis * _tiltAngle * 0.3f, 0.2f)
            .SetEase(Ease.OutSine)
            );

        Vector3 targetPos = _startPos + Vector3.down * _dropHeight;
        Vector3 targetEuler = _startRot.eulerAngles + _tiltAxis * _tiltAngle;

        _collapseSeq.Append
            (
            transform.DOMove(targetPos, _fallDuration)
            .SetEase(Ease.InQuad)
            );
        _collapseSeq.Join
            (
            transform.DORotate(targetEuler, _fallDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InQuad)
            );

        _collapseSeq.OnComplete(() => completedAction());
        _collapseSeq.Play();

        if (_mainCamera != null)
            _mainCamera.DOShakePosition(_shakeDuration, shakeStrength, 10, 90, false);
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

    private void InitiliazedTowerEffect()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
        _mainCamera = Camera.main;
    }
}
