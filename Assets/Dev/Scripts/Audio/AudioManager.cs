using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private string _startAmbientName;
    [SerializeField] private List<Sound> _sounds;

    private AudioSource _currentAmbient;

    private Dictionary<string, AudioSource> _audioSources = new();

    public void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;

        Initialized();
        SetAmbient(_startAmbientName);
        DontDestroyOnLoad(this);
    }

    public static void PlaySound(string name)
    {
        if(Instance._audioSources.ContainsKey(name) == false)
        {
            Debug.Log("<color=red>Sound not found</color>");
        }

        Instance._audioSources[name].Play();
    }

    public static void StopSound(string name)
    {
        if (Instance._audioSources.ContainsKey(name) == false)
        {
            Debug.Log("<color=red>Sound not found</color>");
        }

        Instance._audioSources[name].Stop();
    }

    public void SetAmbient(string name)
    {
        AudioSource newAmbient = _audioSources[name];

        if (_currentAmbient != null)
            _currentAmbient.Stop();

        _currentAmbient = newAmbient;
        _currentAmbient.Play();

        Debug.Log($"<color=green>Play Ambient</color> {name}");
    }

    private void Initialized()
    {
        foreach (var sound in _sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            
            source.clip = sound.Clip;
            source.volume = sound.StartVolume;
            source.pitch = sound.StartPitch;
            source.loop = sound.IsAmbient;
            source.playOnAwake = false;
            
            source.Stop();

            _audioSources.Add(sound.Name, source);
        }
    }

    
}
