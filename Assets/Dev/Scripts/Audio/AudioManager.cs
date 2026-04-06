using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class AudioManager : MonoBehaviour, ISaveSystemService
{
    [Inject] private SettingData _settingData;

    private const string MUSIC_GROUP = "Music";
    private const string SFX_GROUP = "SFX";
    private const string MENU_MUSIC_GROUP = "MenuMusic";

    public static AudioManager Instance;

    [Header("<color=green><b>Main Settings</b></color>")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _startAmbientName;
    [SerializeField] private List<Sound> _sounds;
    [Space(20)]

    private AudioSource _currentAmbient;

    private Dictionary<string, AudioSource> _audioSources = new();

    private void Awake()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;

        Initialized();
        SetAmbient(_startAmbientName);
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Debug.Log($"<color=red>Load Data</color> {_settingData.SFXVolume}");
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

    public void SetVolume(float volume, string parametr)
    {
        _audioMixer.SetFloat(parametr, Mathf.Log10(volume) * 20);
    }

    private void Initialized()
    {
        foreach (var sound in _sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            
            source.clip = sound.Clip;
            source.volume = sound.StartVolume;
            source.pitch = sound.StartPitch;
            
            source.playOnAwake = false;

            switch (sound.Type)
            {
                case SoundType.Ambient:
                    source.loop = true;
                    source.outputAudioMixerGroup =
                        _audioMixer.FindMatchingGroups(MUSIC_GROUP)[0];
                    break;

                case SoundType.SFX:
                    source.outputAudioMixerGroup = 
                        _audioMixer.FindMatchingGroups(SFX_GROUP)[0];
                    break;

                case SoundType.MenuMusic:
                    source.outputAudioMixerGroup = 
                        _audioMixer.FindMatchingGroups(MENU_MUSIC_GROUP)[0];
                    break;
            }
            
            source.Stop();

            _audioSources.Add(sound.Name, source);
        }
    }

    public void LoadData()
    {
        _audioMixer.SetFloat("sfxVolume", _settingData.SFXVolume);
        _audioMixer.SetFloat("musicVolume", _settingData.MusicVolume);
    }

    public void SaveData(SaveSystem saveSystem)
    {
        _audioMixer.GetFloat("sfxVolume", out _settingData.SFXVolume);
        _audioMixer.GetFloat("musicVolume", out _settingData.MusicVolume);

        saveSystem.SaveDate(_settingData, "SettingData");

        Debug.Log($"<color=yellow>Save Setting Data</color>\nSave Data {_settingData.SFXVolume}");
    }

    public float GetVolumeValue(string parametr)
    {
        _audioMixer.GetFloat(parametr, out float value);

        return Mathf.Pow(10f, value / 20f);
    }
}
