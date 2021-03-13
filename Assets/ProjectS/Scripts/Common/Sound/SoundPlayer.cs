using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectS
{
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
        
        // 仮の値
        private const int SOURCE = 10;
        private List<AudioSource> _audioSources = new List<AudioSource>();
        private List<int> _notPlayingSources = new List<int>();
        private List<int> _playingSources = new List<int>();
        private AudioSource _bgmAudioSource;
        private AudioSource _seOneShotSource;
        private Dictionary<int, Action> _callbacks = new Dictionary<int, Action>();
        
        #endregion
        
        #region Method
        
        private void PlayBgm(AudioClip clip, bool loop = true)
        {
            if (_bgmAudioSource.isPlaying) _bgmAudioSource.Pause();
            _bgmAudioSource.clip = clip;
            _bgmAudioSource.loop = loop;
            _bgmAudioSource.Play();
        }
        
        public void PlayBGM(Bgm id, bool loop = true)
        {
            var clip = SoundStore.Instance.GetBgm(id);
            PlayBgm(clip, loop);
        }
        
        public void PlaySeOneShot(AudioClip clip)
        {
            _seOneShotSource.PlayOneShot(clip);
        }
        
        public void PlaySeOneShot(Se id)
        {
            var clip = SoundStore.Instance.GetSe(id);
            PlaySeOneShot(clip);
        }
        
        private int PlaySe(AudioClip clip, bool loop = false, Action callback = null)
        {
            if (_notPlayingSources.Count == 0) return -1;
            var index = _notPlayingSources[0];
            var source = _audioSources[index];
            source.clip = clip;
            source.loop = loop;
            source.Play();
            _notPlayingSources.Remove(index);
            _playingSources.Add(index);
            if (callback != null) _callbacks.Add(index, callback);
            return index;
        }
        
        public int PlaySe(Se id, bool loop = false, Action callback = null)
        {
            var clip = SoundStore.Instance.GetSe(id);
            return PlaySe(clip, loop, callback);
        }
        
        public void StopBgm()
        {
            _bgmAudioSource.Stop();
        }
        
        #endregion
        
        #region MonoBehavior
        
        private void Awake()
        {
            for (int i = 0; i < SOURCE; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _audioSources.Add(source);
                _notPlayingSources.Add(i);
            }
            _bgmAudioSource = gameObject.AddComponent<AudioSource>();
            _bgmAudioSource.playOnAwake = false;
            _seOneShotSource = gameObject.AddComponent<AudioSource>();
            _seOneShotSource.playOnAwake = false;
        }
        
        private void Update()
        {
            var finishIndexes = new List<int>();
            foreach (var index in _playingSources)
            {
                var source = _audioSources[index];
                if (source.loop) continue;
                if (source.isPlaying) continue;
                finishIndexes.Add(index);
                if (_callbacks.ContainsKey(index))
                {
                    _callbacks[index]();
                    _callbacks.Remove(index);
                }
            }
            while (finishIndexes.Count > 0)
            {
                var index = finishIndexes[0];
                finishIndexes.RemoveAt(0);
                _playingSources.Remove(index);
                _notPlayingSources.Add(index);
            }
        }
        
        #endregion
    }
}