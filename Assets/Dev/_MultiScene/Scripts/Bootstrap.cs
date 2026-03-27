using UnityEngine;
using System.Collections;

public class Bootstrap : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private LoaderUI _loaderUI;

    private IEnumerator Start()
    {
        BindingObject();

        yield return CreateObject();
        yield return Initialized();

        PrepareGame();
    }

    private void BindingObject()
    {

    }

    private IEnumerator Initialized()
    {
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
