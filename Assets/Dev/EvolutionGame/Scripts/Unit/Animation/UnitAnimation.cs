using UnityEngine;
using UnityEngine.AI;

public class UnitAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitBase _unitBase;
    [SerializeField] private NavMeshAgent _agent;

    private void Awake()
    { 
        if(_unitBase == null) _unitBase = GetComponent<UnitBase>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if(_agent == null) _agent = GetComponent<NavMeshAgent>();

        _unitBase.onAttack += PlayAttackAnimation;
        _unitBase.onDie += PlayDieAnimation;
        _unitBase.onChangeHealth += PlayHitAnimation;
    }

    private void FixedUpdate()
    {
        float speedPercent = _agent.velocity.magnitude / _agent.speed;
        
        _animator.SetFloat("speed", speedPercent);
    }

    private void PlayAttackAnimation() => _animator.SetTrigger("onAttack");
    
    private void PlayDieAnimation() => _animator.SetTrigger("onDie");

    private void PlayHitAnimation(float d, float a) => _animator.SetTrigger("onHit"); 
}
