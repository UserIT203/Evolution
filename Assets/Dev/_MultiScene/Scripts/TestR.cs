using UnityEngine;
using DG.Tweening;

public class TestR : MonoBehaviour
{
    // Update is called once per frame
    void Start()
    {
        transform.DORotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Yoyo);
    }
}
