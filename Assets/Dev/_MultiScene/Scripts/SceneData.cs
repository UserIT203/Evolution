using UnityEngine;

namespace System.SceneManager
{
    [Serializable]
    public class SceneData
    {
        [field: SerializeField] public string SceneName { get; set; }
        [field: SerializeField] public SceneType SceneType { get; private set; }
    }

    public enum SceneType
    {
        ActiveScene,
        MainMenu,
        GamePlay,
        HUD
    }
}
