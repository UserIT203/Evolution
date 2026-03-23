using UnityEngine;
using System.Collections;

public class Bootstrap : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private LoaderUI _loaderUI;

    [Header("Manager")]
    [SerializeField] private SceneLoader _loader;


    private IEnumerator Start()
    {
        BindingObject();

        yield return CreateObject();
        yield return Initialized();

        PrepareGame();
    }

    private void BindingObject()
    {
        _loader = Instantiate(_loader);
    }

    private IEnumerator Initialized()
    {
        _loader.onEndLoading += _loaderUI.CloseLoaderUI;
        _loader.onSceneProgress += _loaderUI.OpenLoaderUI;

        yield return null;
    }

    private IEnumerator CreateObject()
    {
        _loaderUI = Instantiate(_loaderUI);

        yield return null;
    }

    private void PrepareGame()
    {

    }
}
