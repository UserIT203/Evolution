using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Poolable: MonoBehaviour
{
    public Action onRelease;

    public void SetReleaseAction(Action releaseAction)
    {
        onRelease = releaseAction;
    }

    public void Release()
    {
        if(onRelease != null) onRelease.Invoke();
    }

    public void Delete() => Destroy(gameObject);
}
