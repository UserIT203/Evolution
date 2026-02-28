using System;
using Unity.Burst;
using UnityEditor.Purchasing;
using UnityEngine;

public interface IDamagaeble
{
    public Transform Transform { get; set; }

    public event Action<float> onTakeDamage;
    public event Action<float> onReduceArmor;

    public event Action<float, float> onChangeHealth;
    public event Action<float, float> onChangeArmor;

    public event Action onDie;

    public event Action<string> onSetTarget;

    public void TakeDamage(float damage);
    public void Die();
}
