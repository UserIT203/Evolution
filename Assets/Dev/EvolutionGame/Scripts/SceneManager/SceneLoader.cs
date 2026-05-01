using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader
{
    [Inject] private YandexSDK _yandex;

    public Action<float> onLoadSceneProgress;
    public Action onStartLoadScene;
    public Action onEndLoadScene;

    private Dictionary<string, SceneLoadData> _sceneLoadOperation = new();
    private bool _isSwitch = false;

    public async UniTask SwitchScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, ISceneArgs args = null)
    {
        if (_isSwitch == true) return;

        Debug.Log($"<color=black>Load Scene With Index {sceneName}</color>");

        _isSwitch = true;

        onStartLoadScene?.Invoke();

        if (loadSceneMode == LoadSceneMode.Single)
            await UnloadedSceneWithout(sceneName);

        await LoadScene(sceneName, loadSceneMode);

        _isSwitch = false;

        Scene targetScene = SceneManager.GetSceneByName(sceneName);

        Bootstrap[] bootstraps = GameObject.FindObjectsByType<Bootstrap>(FindObjectsSortMode.None);
        
        if(bootstraps.FirstOrDefault(i => i.gameObject.scene == targetScene) != null)
            await bootstraps.FirstOrDefault(i => i.gameObject.scene == targetScene).Initialized(args);

        _yandex.ShowInterstitialADV();

        Debug.Log($"<color=black>End Load Scene With Index {sceneName}</color>");

        onEndLoadScene?.Invoke();
    }

    public async UniTask UnloadScene(string sceneName)
    {
        if (_isSwitch == true) return;

        _isSwitch = true;

        onStartLoadScene?.Invoke();

        await Addressables.UnloadSceneAsync(_sceneLoadOperation[sceneName].Handle).ToUniTask();

        _sceneLoadOperation.Remove(sceneName);

        _isSwitch = false;

        onEndLoadScene?.Invoke();
    }

    private UniTask LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if(_sceneLoadOperation.TryGetValue(sceneName, out var operation) == true)
        {
            operation.RequestCount++;
            return UniTask.CompletedTask;
        }

        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName, loadSceneMode);

        _sceneLoadOperation.Add(sceneName, new SceneLoadData(handle, 1));

        return handle.ToUniTask();
    }

    private async UniTask UnloadedSceneWithout(string sceneName)
    {
        List<UniTask> unloadOperation = new List<UniTask>(_sceneLoadOperation.Count);

        foreach (KeyValuePair<string, SceneLoadData> loadOperation in _sceneLoadOperation.ToList())
        {
            if(loadOperation.Key != sceneName)
            {
                unloadOperation.Add(Addressables.UnloadSceneAsync(loadOperation.Value.Handle).ToUniTask());
                _sceneLoadOperation.Remove(loadOperation.Key);
            }
        }

        await UniTask.WhenAll(unloadOperation);
    }

    private class SceneLoadData
    {
        public AsyncOperationHandle<SceneInstance> Handle;
        public int RequestCount;

        public SceneLoadData(AsyncOperationHandle<SceneInstance> handle, int requestCount)
        {
            Handle = handle;
            RequestCount = requestCount;
        }
    }
}
