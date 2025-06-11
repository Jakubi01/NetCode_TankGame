using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum SoundType
{
    ThemeBegin,
    ThemeTank,
    ThemeEnd
}

[System.Serializable]
public struct SoundTypeAudioClipPair
{
    public SoundType SoundType;
    public AudioClip AudioClip;

    public SoundTypeAudioClipPair(SoundType soundType, AudioSource source)
    {
        SoundType = soundType;
        AudioClip = source.clip;
    }
}

public class SoundManager : NetworkBehaviour
{
    public SoundTypeAudioClipPair[] soundAssets;
    
    [Range(0, 1)]
    public float soundVolume = 0.5f;
    
    private Dictionary<SoundType, AudioSource> _soundCache = new();
    private AudioSource _source;
    
    public static SoundManager Instance;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        _source = GetComponent<AudioSource>();
        if (!_source)
        {
            _source = gameObject.AddComponent<AudioSource>();
        }
        
        _soundCache = new Dictionary<SoundType, AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(SoundType soundType, bool bLoop)
    {
        _PlaySound(soundType, bLoop);
    }

    public void StopPlaySound(SoundType soundType)
    {
        _StopPlaySound(soundType);
    }

    private void _PlaySound(SoundType soundType, bool bLoop)
    {
        var clip = GetClipByType(soundType);
        _source.clip = clip;
        _source.loop = bLoop;
        _source.volume = soundVolume;
        _soundCache.Add(soundType, _source);
        _source.Play();
    }

    private void _StopPlaySound(SoundType soundType)
    {
        if (_source.isPlaying && _soundCache.TryGetValue(soundType, out var audioSource))
        {
            if (audioSource == _source)
            {
                _source.Stop();
                _soundCache.Remove(soundType);
            }
        }
    }

    private AudioClip GetClipByType(SoundType soundType)
    {
        return soundAssets[(int)soundType].AudioClip;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (_source.isPlaying)
        {
            if (hasFocus)
            {
                _source.volume = soundVolume;
            }
            else
            {
                _source.volume = 0;
            }
        }
    }
}
