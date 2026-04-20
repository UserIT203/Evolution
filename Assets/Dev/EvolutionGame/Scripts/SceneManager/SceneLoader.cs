using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public Action<float> onLoadSceneProgress;
    public Action onStartLoadScene;
    public Action onEndLoadScene;

    private AsyncOperation _loadScene;
    private bool _isSwitch = false;

    public async UniTask SwitchScene(int sceneIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single, ISceneArgs args = null)
    {
        if (_isSwitch == true) return;

        Debug.Log($"<color=black>Load Scene With Index {sceneIndex}</color>");

        _isSwitch = true;

        _loadScene = SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);

        onStartLoadScene?.Invoke();

        await _loadScene;

        _isSwitch = false;

        Scene targetScene = SceneManager.GetSceneByBuildIndex(sceneIndex);

        Bootstrap[] bootstraps = GameObject.FindObjectsByType<Bootstrap>(FindObjectsSortMode.None);
        await bootstraps.FirstOrDefault(i => i.gameObject.scene == targetScene).Initialized(args);

        Debug.Log($"<color=black>End Load Scene With Index {sceneIndex}</color>");

        onEndLoadScene?.Invoke();
    }

    public async UniTask UnloadScene(int sceneIndex)
    {
        if (_isSwitch == true) return;

        _isSwitch = true;

        onStartLoadScene?.Invoke();

        await SceneManager.UnloadSceneAsync(sceneIndex);

        _isSwitch = false;

        onEndLoadScene?.Invoke();
    }
}
