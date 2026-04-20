using System.Collections.Generic;
using UnityEngine;

public class CustomPool<T> where T : Poolable
{
    private T _prefab;
    private Queue<T> _objects;
    private Transform _poolContainer;

    public CustomPool(T prefab, int prewareObjects, Transform poolContainer)
    {
        _prefab = prefab;
        _objects = new Queue<T>();
        _poolContainer = poolContainer;

        for (int i = 0; i < prewareObjects; i++)
        {
            var obj = GameObject.Instantiate(_prefab);
            obj.transform.name = obj.transform.name + "_" + i.ToString();
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_poolContainer, false);

            obj.SetReleaseAction(() => Release(obj));
            _objects.Enqueue(obj);
        }
    }

    public T Get()
    {
        var obj = _objects.Dequeue();

        if (_objects.Count == 1)
            obj = Create();

        return obj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.position = Vector3.zero;

        _objects.Enqueue(obj);
    }

    public void ClearAll()
    {
        while (_objects.Count > 0)
        {
            var obj = _objects.Dequeue();
            Debug.Log($"Object in pool {obj.name}");

            if (obj != null)
                obj.Delete();
        }

        _prefab = null;
    }

    private T Create()
    {
        var obj = GameObject.Instantiate(_prefab);
        obj.transform.name = obj.transform.name + "_" + _objects.Count.ToString();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_poolContainer, false);

        obj.SetReleaseAction(() => Release(obj));
        _objects.Enqueue(obj);

        return _objects.Dequeue();
    }
}
