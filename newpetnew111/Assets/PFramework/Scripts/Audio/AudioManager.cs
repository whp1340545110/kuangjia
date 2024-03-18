using UnityEngine;
using System.Collections.Generic;
using UniRx.Async;

namespace PFramework.Audio
{
    public class AudioManager : ManagerBase<AudioManager>
    {
        internal class AudioInfo
        {
            public string path;
            public AudioClip clip;
        }

        private Dictionary<string, AudioInfo> _audioCaches;      // 存放音频文件路径
        private AudioSource _musicAudioSource;                   // 背景音乐
        private AudioSource _ambienceAudioSource;                // 环境音乐
        private List<AudioSource> _unusedSoundAudioSourceList;   // 存放可以使用的音频组件
        private List<AudioSource> _usedSoundAudioSourceList;     // 存放正在使用的音频组件
        private readonly Dictionary<string, int> _playingSoundsCount = new Dictionary<string, int>();

        private float _mainVolume = 0.5f;
        public float MainVolume
        {
            get
            {
                return _mainVolume;
            }
            set
            {
                if (value >= 1)
                {
                    value = 1;
                }
                else if (value <= 0)
                {
                    value = 0;
                }
                ChangeMainVolume(value);
                _mainVolume = value;
            }
        }

        private float _musicVolume = 1f;
        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                if (value >= 1)
                {
                    value = 1;
                }
                else if (value <= 0)
                {
                    value = 0;
                }
                ChangeMusicVolume(value);
                _musicVolume = value;
            }
        }

        private float _soundVolume = 1;
        public float SoundVolume
        {
            get
            {
                return _soundVolume;
            }
            set
            {
                if (value >= 1)
                {
                    value = 1;
                }
                else if (value <= 0)
                {
                    value = 0;
                }
                ChangeSoundVolume(value);
                _soundVolume = value;
            }
        }

        private bool _isMute;
        public bool IsMute
        {
            get
            {
                return _isMute;
            }
            set
            {
                ChangeIsMute(value);
                _isMute = value;
            }
        }

        private int poolCount = 3;        // 对象池数量 主音乐 环境音乐 音效

        private string playingMusic = null;

        private GameObject _gameObject;
        public GameObject gameObject
        {
            get
            {
                if (!_gameObject)
                {
                    _gameObject = new GameObject("Peach.AudioManager");
                }
                return _gameObject;
            }
        }

        private AudioManager(){ }

        public override void Initialize()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            _audioCaches = new Dictionary<string, AudioInfo>();
            _musicAudioSource = gameObject.AddComponent<AudioSource>();
            _ambienceAudioSource = gameObject.AddComponent<AudioSource>();
            _unusedSoundAudioSourceList = new List<AudioSource>();
            _usedSoundAudioSourceList = new List<AudioSource>();
        }

        public void PlayMusic(string path, bool loop = true)
        {
            PlayMusicAsync(path, loop).Forget();
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loop"></param>
        private async UniTask PlayMusicAsync(string path, bool loop = true)
        {
            if (playingMusic == path)
            { //如果是正在播放，继续让音乐播放
                _musicAudioSource.Play();
                _musicAudioSource.volume = MainVolume * MusicVolume;
                return;
            }
            // 通过Tween将声音淡入淡出
            _musicAudioSource.volume = 0;
            _musicAudioSource.clip = await GetAudioClip(path);
            if (_musicAudioSource.clip != null)
            {
                _musicAudioSource.clip.LoadAudioData();
                _musicAudioSource.loop = loop;
                _musicAudioSource.volume = MainVolume * MusicVolume;
                _musicAudioSource.mute = IsMute;
                _musicAudioSource.Play();
                playingMusic = path;
            }
            _musicAudioSource.volume = MainVolume* MusicVolume;
        }

        public void StopMusic()
        {
            _musicAudioSource.Stop();
            _musicAudioSource.volume = MainVolume * MusicVolume;
        }

        public void PlayAmbience(string path)
        {
            PlayAmbienceAsync(path).Forget();
        }

        /// <summary>
        /// 播放环境音乐
        /// </summary>
        /// <param name="path"></param>
        private async UniTask PlayAmbienceAsync(string path)
        {
            _ambienceAudioSource.clip = await GetAudioClip(path);
            if (_ambienceAudioSource.clip != null)
            {
                _ambienceAudioSource.clip.LoadAudioData();
                _ambienceAudioSource.loop = false;
                _ambienceAudioSource.volume = MainVolume * MusicVolume;
                _ambienceAudioSource.mute = IsMute;
                _ambienceAudioSource.Play();
            }
        }

        public void PlaySound(string path)
        {
            PlaySoundAsync(path).Forget();
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="path"></param>
        private async UniTask PlaySoundAsync(string path)
        {
            if (SoundVolume == 0f)
            {//静音
                return;
            }

            if (!_playingSoundsCount.ContainsKey(path))
            {
                _playingSoundsCount.Add(path, 0);
            }

            var audioClip = await GetAudioClip(path);
            if (audioClip != null)
            {
                AudioSource audioSource = null;
                if (_unusedSoundAudioSourceList.Count != 0)
                {
                    audioSource = UnusedToUsed();
                }
                else
                {
                    AddAudioSource();
                    audioSource = UnusedToUsed();
                    audioSource.volume = MainVolume * SoundVolume;
                    audioSource.loop = false;
                    audioSource.mute = IsMute;
                }
                audioSource.clip = audioClip;
                audioSource.clip.LoadAudioData();
                var pitch = 1.0f;

                audioSource.pitch = pitch;
                audioSource.Play();
                _playingSoundsCount[path]++;
                WaitPlayEnd(audioSource, path).Forget();
            }
        }

        /// <summary>
        /// 当播放音效结束后，将其移至未使用集合
        /// </summary>
        /// <param name="audioSource"></param>
        /// <returns></returns>
        private async UniTask WaitPlayEnd(AudioSource audioSource, string id)
        {
            await UniTask.WaitUntil(() => { return !audioSource.isPlaying; });
            UsedToUnused(audioSource);
            _playingSoundsCount[id]--;
        }

        /// <summary>
        /// 获取音频文件，获取后会缓存一份
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async UniTask<AudioClip> GetAudioClip(string path)
        {
            if (!_audioCaches.ContainsKey(path))
            {
                var audioInfo = new AudioInfo()
                {
                    path = path,
                };
                audioInfo.clip = await Peach.LoadAsync<AudioClip>(path);
                if (!_audioCaches.ContainsKey(path))
                {
                    _audioCaches.Add(path, audioInfo);
                }
            }

            return _audioCaches[path].clip;
        }

        /// <summary>
        /// 添加音频组件
        /// </summary>
        /// <returns></returns>
        private AudioSource AddAudioSource()
        {
            if (_unusedSoundAudioSourceList.Count != 0)
            {
                return UnusedToUsed();
            }
            else
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                _unusedSoundAudioSourceList.Add(audioSource);
                return audioSource;
            }
        }

        /// <summary>
        /// 将未使用的音频组件移至已使用集合里
        /// </summary>
        /// <returns></returns>
        private AudioSource UnusedToUsed()
        {
            AudioSource audioSource = _unusedSoundAudioSourceList[0];
            _unusedSoundAudioSourceList.RemoveAt(0);
            _usedSoundAudioSourceList.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 将使用完的音频组件移至未使用集合里
        /// </summary>
        /// <param name="audioSource"></param>
        private void UsedToUnused(AudioSource audioSource)
        {
            _usedSoundAudioSourceList.Remove(audioSource);
            if (_unusedSoundAudioSourceList.Count >= poolCount)
            {
                GameObject.Destroy(audioSource);
            }
            else
            {
                _unusedSoundAudioSourceList.Add(audioSource);
            }
        }

        /// <summary>
        /// 修改主音乐音量
        /// </summary>
        /// <param name="volume"></param>
        private void ChangeMainVolume(float volume)
        {
            _musicAudioSource.volume = MusicVolume * volume;
            _ambienceAudioSource.volume = MusicVolume * volume;
            for (int i = 0; i < _unusedSoundAudioSourceList.Count; i++)
            {
                _unusedSoundAudioSourceList[i].volume = SoundVolume * volume;
            }
            for (int i = 0; i < _usedSoundAudioSourceList.Count; i++)
            {
                _usedSoundAudioSourceList[i].volume = SoundVolume * volume;
            }
        }

        /// <summary>
        /// 修改背景音乐音量
        /// </summary>
        /// <param name="volume"></param>
        private void ChangeMusicVolume(float volume)
        {
            _musicAudioSource.volume = MainVolume * volume;
            _ambienceAudioSource.volume = MainVolume * volume;
        }

        /// <summary>
        /// 修改音效音量
        /// </summary>
        /// <param name="volume"></param>
        private void ChangeSoundVolume(float volume)
        {
            for (int i = 0; i < _unusedSoundAudioSourceList.Count; i++)
            {
                _unusedSoundAudioSourceList[i].volume = MainVolume * volume;
            }
            for (int i = 0; i < _usedSoundAudioSourceList.Count; i++)
            {
                _usedSoundAudioSourceList[i].volume = MainVolume * volume;
            }
        }

        private void ChangeIsMute(bool isMute)
        {
            _musicAudioSource.mute = isMute;
            _ambienceAudioSource.mute = isMute;
            for (int i = 0; i < _unusedSoundAudioSourceList.Count; i++)
            {
                _unusedSoundAudioSourceList[i].mute = isMute;
            }
            for (int i = 0; i < _usedSoundAudioSourceList.Count; i++)
            {
                _usedSoundAudioSourceList[i].mute = isMute;
            }
        }
    }
}