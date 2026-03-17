using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsView : MonoBehaviour
{
    [SerializeField] private UnitBarView _barViewPrefab;
    [SerializeField] private float _offset;

    private UnitBarView _unitBarView;

    public void ChangeHealthBarValue(float maxValue, float value)
    {
        if (_unitBarView == null) CreateViewPrefab();

        if (_unitBarView.HealthBar.enabled == false) _unitBarView.HealthBar.enabled = true;

        _unitBarView.HealthBar.fillAmount = value / maxValue;
    }

    public void ChangeArmorBarValue(float maxValue, float value)
    {
        if (_unitBarView == null) CreateViewPrefab();

        if (_unitBarView.ArmorBar.enabled == false) _unitBarView.ArmorBar.enabled = true;

        _unitBarView.ArmorBar.fillAmount = value / maxValue;
    }

    private void CreateViewPrefab()
    {
        Vector3 topPosition = Vector3.zero;
        topPosition.y = _offset;

        _unitBarView = Instantiate(_barViewPrefab, topPosition, Quaternion.identity);
        _unitBarView.transform.SetParent(transform, false);
    }
}
