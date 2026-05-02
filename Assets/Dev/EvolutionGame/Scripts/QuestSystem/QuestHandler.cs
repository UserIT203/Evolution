using UnityEngine;

[RequireComponent(typeof(UnitBase))]
public class QuestHandler : MonoBehaviour
{
    private UnitBase _enemyBase;

    private void Awake()
    {
        _enemyBase = GetComponent<UnitBase>();

        _enemyBase.onDie += Handler;
    }

    private void OnDestroy()
    {
        _enemyBase.onDie -= Handler;
    }

    private void Handler()
    {
        try
        {
            if (_enemyBase.UnitConfig.UnitType == UnitType.Melee)
                QuestBus.GetInstance()?.onUpdateCounter(QuestType.KillMelleEnemy, 1);

            if (_enemyBase.UnitConfig.UnitType == UnitType.Ranged)
                QuestBus.GetInstance()?.onUpdateCounter(QuestType.KillRangedEnemy, 1);

            if (_enemyBase.UnitConfig.UnitType == UnitType.Heavy)
                QuestBus.GetInstance()?.onUpdateCounter(QuestType.KillHeavyEnemy, 1);

            QuestBus.GetInstance()?.onUpdateCounter(QuestType.KillEnemy, 1);
        }
        catch 
        {
            Debug.LogError("Quest Handler Error");
        }
    }
}
