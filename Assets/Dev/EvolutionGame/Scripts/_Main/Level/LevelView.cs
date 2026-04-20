using TMPro;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    [Header("Level Upgrades UI Links")]
    [SerializeField] private TMP_Text _coinsCountText;

    public void ChangeCointText(int coinsCount)
    {
        _coinsCountText.text = coinsCount.ToString();
    }
}
