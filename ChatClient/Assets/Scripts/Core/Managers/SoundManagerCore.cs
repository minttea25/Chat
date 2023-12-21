using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UObject = UnityEngine.Object;

namespace Core
{
    public class SoundManagerCore : IManager
    {
        /// <summary>
        /// These audio sources play with each settings. (NOTE: Audiosource of BGM has default value[loop=true].)
        /// </summary>
        readonly AudioSource[] _audios = new AudioSource[Const.DefaultAudioSourceCount];

        /// <summary>
        /// exactly same as Resource.LoadAudioClipAsync()
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        public void LoadAudioClipAsync(string key, Action<AudioClip> callback = null)
        {
            ManagerCore.Resource.LoadAudioClipAsync(key, callback);
        }

        /// <summary>
        /// Play a audioclip of the adrressable key at a typed audiosource. (It would be processed asynchrounsly)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="pitch"></param>
        public void LoadAudioClipAsyncAndPlay(string key, AudioType type, Action<AudioClip> callback = null, float pitch = 1.0f)
        {
            ManagerCore.Resource.LoadAudioClipAsync(key, (audioclip) => { callback?.Invoke(audioclip); PlaySound(audioclip, type, pitch); });
        }

        /// <summary>
        /// You can add 'switch' of 'if' with your added AudioTypes after the first line: base.PlaySound(~).
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="type"></param>
        /// <param name="pitch"></param>
        public void PlaySound(AudioClip audioClip, AudioType type, float pitch = 1.0f)
        {
            // assert crash
            if (audioClip == null) return;

            if (type == AudioType.BGM)
            {
                var audio = _audios[(int)AudioType.BGM];
                if (audio.isPlaying)
                {
                    audio.Stop();
                }
                audio.clip = audioClip;
                audio.pitch = pitch;
                audio.Play();

                return;
            }
            else if (type == AudioType.Effect)
            {
                var audio = _audios[(int)AudioType.Effect];
                audio.pitch = pitch;
                audio.PlayOneShot(audioClip);
                return;
            }
            else if (type == AudioType.Voice)
            {
                var audio = _audios[(int)AudioType.Voice];
                audio.pitch = pitch;
                audio.PlayOneShot(audioClip);
                return;
            }
        }

        public void Stop(AudioType type)
        {
            _audios[(int)type].Stop();
        }

        public AudioSource GetAudioSource(AudioType audioType)
        {
            return _audios[(int)audioType];
        }

        public bool IsPlaying(AudioType type)
        {
            return _audios[(int)type].isPlaying;
        }

        void IManager.ClearManager()
        {
            for (int i = 0; i < _audios.Length; i++)
            {
                _audios[i].Stop();
                _audios[i].clip = null;
            }
        }

        void IManager.InitManager()
        {
            GameObject rootObject = GameObject.Find($"{Const.AudioName}");
            if (rootObject == null)
            {
                rootObject = new GameObject { name = $"{Const.AudioSourceName}" };
                UObject.DontDestroyOnLoad(rootObject);

                string[] audioSourceNames = Enum.GetNames(typeof(AudioType));
                for (int i = 0; i < Const.DefaultAudioSourceCount; i++)
                {
                    GameObject go = new() { name = $"{Const.AudioSourceName}_{audioSourceNames[i]}" };
                    _audios[i] = go.GetOrAddComponent<AudioSource>();
                    go.transform.parent = rootObject.transform;
                }

                _audios[(int)AudioType.BGM].loop = true;
            }
        }
    }
}
