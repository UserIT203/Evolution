using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader : ITickable
{
    public Action<float> onLoadSceneProgress;
    public Action onStartLoadScene;
    public Action onEndLoadScene;

    private AsyncOperation _loadScene;
    private bool _isSwitch = false;

    public void SwitchScene(int sceneIndex)
    {
        _isSwitch = true;

        _loadScene = SceneManager.LoadSceneAsync(sceneIndex);

        onStartLoadScene?.Invoke();
    }

    public void Tick()
    {
        if(_isSwitch == true && _loadScene != null)
        {
            if (_loadScene.isDone == false)
                onLoadSceneProgress?.Invoke(_loadScene.progress);
            else
            {
                _isSwitch = false;
                onEndLoadScene?.Invoke();
            }    
        }
    }
}
