using UnityEngine;
using UnityEngine.AI;

public class UnitInstaller : MonoBehaviour
{ 
    [SerializeField] private bool _isEnemy;
    [SerializeField] private UnitType _type;

    public void InitializedUnit()
    {
        gameObject.AddComponent<BoxCollider>();
        gameObject.AddComponent<Animator>();
        
        switch (_type)
        {
            case UnitType.Melee:
                gameObject.AddComponent<UnitMelee>();
                break;

            case UnitType.Ranged:
                gameObject.AddComponent<UnitRanged>();
                break;
        }

        if (_isEnemy == true)
            gameObject.AddComponent<LootBag>();

        gameObject.AddComponent<UnitAnimation>();
        gameObject.AddComponent<StatsController>();

        DestroyImmediate(GetComponent<UnitInstaller>());
    }
}


