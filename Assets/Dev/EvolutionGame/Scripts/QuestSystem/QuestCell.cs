using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class QuestCell : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private Image _progressSlider;
    [SerializeField] private Image _finishQuest;

    private LocalizationSelector _localizationSelector;

    public QuestData Data { get; set; }

    [Inject]
    public void Construct(LocalizationSelector localizationSelector)
    {
        _localizationSelector = localizationSelector;
        localizationSelector.onChangeLocale += UpdateInfo;
    }

    private void OnEnable()
    {
        QuestBus.GetInstance().onUpdateData += UpdateInfo;
    }

    private void OnDisable()
    {
        QuestBus.GetInstance().onUpdateData -= UpdateInfo;
    }

    public void Init(QuestData data)
    {
        Data = data;

        _nameText.text = string.Format
            (
            Data.QuestName.GetText(_localizationSelector.CurrentLanguage),
            Data.Goal
            );

        _rewardText.text = Data.Reward.ToString();
        _progressText.text = $"{Data.Progress} | {Data.Goal}";
        _progressSlider.fillAmount = (float)Data.Progress / (float)Data.Goal;
        _finishQuest.enabled = false;
    }

    public void UpdateInfo()
    {
        _nameText.text = string.Format
            (
            Data.QuestName.GetText(_localizationSelector.CurrentLanguage), 
            Data.Goal
            );
        _progressText.text = $"{Data.Progress} | {Data.Goal}";
        _progressSlider.fillAmount = (float)Data.Progress / (float)Data.Goal;            
    }

    public void FinishQuest()
    {
        _finishQuest.enabled = true;
    }
}
