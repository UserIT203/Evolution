using UnityEngine;
using UnityEngine.UI;

public class UnitBarView : MonoBehaviour
{
    [field: SerializeField] public Image HealthBar { get; private set; }
    [field: SerializeField] public Image ArmorBar { get; private set; }

    private void Awake()
    {
        HealthBar.enabled = false; 
        ArmorBar.enabled = false;
    }
}
