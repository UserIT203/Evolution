using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Bootstrap : MonoBehaviour
{
    public abstract UniTask Initialized(ISceneArgs args);
}
