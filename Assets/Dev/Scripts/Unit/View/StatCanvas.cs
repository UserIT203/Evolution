using UnityEngine;

public class StatCanvas : MonoBehaviour
{
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        LookOnCamera();
    }

    private void LookOnCamera()
    {
        transform.LookAt(_mainCamera.transform);
    }
}
