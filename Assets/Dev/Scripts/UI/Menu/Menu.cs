using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class Menu : MonoBehaviour 
{
    protected CanvasGroup _canvasGroup;

    public bool IsClosed { get; protected set; }

    [HideInInspector] public MenuManager MenuManager;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Initialized();
    }

    public abstract void OpenMenu();
    public abstract void CloseMenu();

    protected virtual void Initialized()
    {

    }
}
