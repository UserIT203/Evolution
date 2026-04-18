using System;

public class QuestBus
{
    private static QuestBus _instance;

    public static QuestBus GetInstance()
    {
        if(_instance == null )
            _instance = new QuestBus();

        return _instance;
    }

    public Action<QuestType, int> onUpdateCounter;
    public Action onUpdateData;
}
