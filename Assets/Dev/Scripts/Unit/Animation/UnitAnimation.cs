using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitBase))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class UnitAnimation : MonoBehaviour
{
    private UnitBase _unitBase;
    private Animator _animator;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _unitBase = GetComponent<UnitBase>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

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
