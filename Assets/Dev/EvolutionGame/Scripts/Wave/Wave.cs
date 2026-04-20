using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public string WaveName;
    [SerializeField] public List<WaveStage> StagesList = new List<WaveStage>();
    public float Delay;

    private Queue<WaveStage> _stagesQueue;

    public Queue<WaveStage> Stages => _stagesQueue;

    public void Initialized()
    {
        _stagesQueue = new Queue<WaveStage>();

        foreach (var stage in StagesList)
        {
            _stagesQueue.Enqueue(stage);
        }
    }

    public WaveStage GetStage()
    {
        return _stagesQueue.Dequeue();
    }
}
