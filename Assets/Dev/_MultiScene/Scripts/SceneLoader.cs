using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.SceneManager;
using System;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private List<SceneData> _scenesData;

    private int _currentLevelSceneIndex;
    private bool _isLoaded;

    private Coroutine _loadSceneCoroutine;
    private Coroutine _unloadSceneCoroutine;

    public Action<float> onSceneProgress;

    private void OnDisable()
    {
        StopCoroutine(_loadSceneCoroutine);
        StopCoroutine(_unloadSceneCoroutine);
    }

    public void LoadScene(SceneType sceneType, Action onComplete = null)
    {
        string sceneName = GetSceneByType(sceneType);
        _loadSceneCoroutine = StartCoroutine(LoadSceneAsync(sceneName, onComplete));
    }

    public void UnloadScene(SceneType sceneType)
    {
        string sceneName = GetSceneByType(sceneType);
        _unloadSceneCoroutine = StartCoroutine(UnloadingScene(sceneName));
    }

    public void SetLevelScene(SceneType sceneType, string levelSceneName)
    {
        SceneData sceneData = _scenesData.Find(s => s.SceneType == sceneType);
        sceneData.SceneName = levelSceneName;
    }

    private string GetSceneByType(SceneType type)
    {
        return _scenesData.First(s => s.SceneType == type).SceneName;
    }

    private IEnumerator LoadSceneAsync(string sceneName, Action onComplete = null)
    {
        if (_isLoaded == transform) yield break;

        _isLoaded = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    
        while (operation.progress < 0.9f)
        {
            onSceneProgress?.Invoke(operation.progress);
            yield return null;
        }

        operation.allowSceneActivation = true;

        onComplete?.Invoke();

        _isLoaded = false;
    
        _loadSceneCoroutine = null;

        Debug.Log($"Scene <color=red>{sceneName}</color> load");
    }

    private IEnumerator UnloadingScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded == false) yield break;

        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

        while (operation.isDone == false)
        {
            yield return null;
        }

        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        _unloadSceneCoroutine = null;

        Debug.Log($"Scene <color=red>{sceneName}</color> unload");
    }
}


