using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public struct Sound
{
    public string Name;
    
    [Space(10)]

    public AssetReferenceT<AudioClip> ClipReference;
    public float StartVolume;
    public float StartPitch;
    public SoundType Type;
}

public enum SoundType { SFX, Ambient, MenuMusic}
