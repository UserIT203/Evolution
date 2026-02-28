using UnityEngine;

[RequireComponent(typeof(StatsView))]
public class StatsController : MonoBehaviour
{
    private StatsView _statsView;
    private IDamagaeble _statsModel;

    private void OnEnable()
    {
        _statsView = GetComponent<StatsView>();
        _statsModel = GetComponent<IDamagaeble>();

        _statsModel.onChangeHealth += _statsView.ChangeHealthBarValue;
        _statsModel.onChangeArmor += _statsView.ChangeArmorBarValue;
        _statsModel.onSetTarget += _statsView.SetVictimText;
    }

    private void OnDestroy()
    {
        _statsModel.onChangeHealth -= _statsView.ChangeHealthBarValue;
        _statsModel.onChangeArmor -= _statsView.ChangeArmorBarValue;
        _statsModel.onSetTarget -= _statsView.SetVictimText;
    }
}
