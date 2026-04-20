using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetProvider
{
    private Dictionary<string, AsyncOperationHandle> _assetHandles = new();
    private Dictionary<string, int> _referenceCount = new();

    public async UniTask<T> Load<T>(AssetReferenceT<T> assetReference) where T : UnityEngine.Object
    {
        string key = assetReference.RuntimeKey.ToString();

        if(_assetHandles.TryGetValue(key, out AsyncOperationHandle operation))
        {
            _referenceCount[key]++;
            return operation.Result as T;
        }

        _referenceCount[key] = 1;
        var newHandle = Addressables.LoadAssetAsync<T>(assetReference);
        _assetHandles[key] = newHandle;

        return await newHandle.ToUniTask();
    }

    public void Unload(string reference)
    {
        if (_assetHandles.TryGetValue(reference, out var handle) == false)
            return;

        Addressables.ReleaseInstance(handle);
        Addressables.Release(handle);

        _assetHandles.Remove(reference);
        _referenceCount.Remove(reference);
    }

    public async UniTask UnloadAllAssets()
    {
        Debug.Log($"<color=black>Asset count {_assetHandles.Count}</color>");

        foreach (var asset in _assetHandles)
        {
            Unload(asset.Key);
        }

        await UniTask.CompletedTask;
    }
}
