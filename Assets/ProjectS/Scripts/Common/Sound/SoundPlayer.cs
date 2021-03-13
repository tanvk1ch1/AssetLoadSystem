using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    #region member
    
    private static SoundPlayer instance;
    
    public static SoundPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                var gameObject = new GameObject();
                gameObject.name = nameof(SoundPlayer);
                instance = gameObject.AddComponent<SoundPlayer>();
                DontDestroyOnLoad(gameObject);
            }
            
            return instance;
        }
    }
    
    private List<AudioSource> _audioSources = new List<AudioSource>();
    private List<int> _notPlayingSources = new List<int>();
    private List<int> _playingSources = new List<int>();
    private AudioSource _bgmAudioSource;
    private AudioSource _seOneShotSource;
    
    #endregion
    
    #region Method
    
    private void PlayBgm(AudioClip clip, bool loop = true)
    {
        
    }
    
    #endregion
    
    #region MonoBehavior
    
    private void Awake()
    {
        
    }
    
    private void Update()
    {
        
    }
    
    #endregion
}
