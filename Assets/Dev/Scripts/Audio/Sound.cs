using UnityEngine;

[System.Serializable]
public struct Sound
{
    public string Name;
    
    [Space(10)]

    public AudioClip Clip;
    public float StartVolume;
    public float StartPitch;
    public SoundType Type;
}

public enum SoundType { SFX, Ambient, MenuMusic}
