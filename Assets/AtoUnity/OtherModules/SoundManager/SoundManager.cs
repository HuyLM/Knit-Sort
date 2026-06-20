using AtoGame.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AtoGame.OtherModules.SoundManager
{
    public class SoundManager<T> : SingletonBindAlive<T> where T : SoundManager<T>
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;


        private SoundMangerSaver saver;

        public void Setup(SoundMangerSaver saver)
        {
            this.saver = saver;
            Load();
        }

        public void Load()
        {
            if(MasterEnable)
            {
                mixer.SetFloat("MasterVolume", SoundHelper.ConvertToDecibel(MasterVolume));
            }
            else
            {
                mixer.SetFloat("MasterVolume", SoundHelper.ConvertToDecibel(0));
            }

            if(MusicEnable)
            {
                mixer.SetFloat("MusicVolume", SoundHelper.ConvertToDecibel(MusicVolume));
            }
            else
            {
                mixer.SetFloat("MusicVolume", SoundHelper.ConvertToDecibel(0));
            }

            if(SFXEnable)
            {
                mixer.SetFloat("SFXVolume", SoundHelper.ConvertToDecibel(SFXVolume));
            }
            else
            {
                mixer.SetFloat("SFXVolume", SoundHelper.ConvertToDecibel(0));
            }
        }

        #region Master
        public bool MasterEnable
        {
            get => saver.GetMasterEnable();
            set 
            {
                saver.SetMasterEnable(value);
            }
        }
        public float MasterVolume
        {
            get => saver.GetMasterVolume();
            set
            {
                saver.SetMasterVolume(value);
                mixer.SetFloat("MasterVolume", SoundHelper.ConvertToDecibel(MasterVolume));
				return;
                if(value == 0)
                {
                    MasterEnable = false;
                }
                else
                {
                    MasterEnable = true;
                }
            }
        }
        #endregion

        #region Music
        public bool MusicEnable
        {
            get => saver.GetMusicEnable() && MasterEnable;
            set
            {
                saver.SetMusicEnable(value);
            }
        }
        public float MusicVolume
        {
            get => saver.GetMusicVolume();
            set {
                saver.SetMusicVolume(value);
                mixer.SetFloat("MusicVolume", SoundHelper.ConvertToDecibel(MusicVolume));
				return;
                if(value == 0)
                {
                    MusicEnable = false;
                }
                else
                {
                    MusicEnable = true;
                }
            }
        }

        public void PlayMusic(AudioClip clip, bool fadein = false, float fadeDuration = 1f, bool loop = true, float volume = 1f)
        {
            if(musicAudioSource == null || MusicEnable == false || musicAudioSource.isPlaying)
            {
                return;
            }
            musicAudioSource.clip = clip;
            musicAudioSource.loop = loop;
            musicAudioSource.Play();
            if(fadein)
            {
                FadeIn(musicAudioSource, volume, fadeDuration);
            }
            else
            {
                musicAudioSource.volume = volume;
            }
        }

        public void StopMusic(bool fadeout = false, float fadeDuration = 1f, Action onComplete = null)
        {
            if(musicAudioSource == null || musicAudioSource.isPlaying == false)
            {
                onComplete?.Invoke();
                return;
            }

            if(fadeout)
            {
                FadeOut(musicAudioSource, fadeDuration, ()=> {
                    musicAudioSource.Stop();
                    onComplete?.Invoke();
                });
            }
            else
            {
                musicAudioSource.Stop();
                onComplete?.Invoke();
            }
        }

        public void PauseMusic(bool fadeout = false, float fadeDuration = 1f, Action onComplete = null)
        {
            if(musicAudioSource == null || musicAudioSource.isPlaying == false)
            {
                onComplete?.Invoke();
                return;
            }

            if(fadeout)
            {
                FadeOut(musicAudioSource, fadeDuration, () => {
                    musicAudioSource.Pause();
                    onComplete?.Invoke();
                });
            }
            else
            {
                musicAudioSource.Pause();
                onComplete?.Invoke();
            }
        }

        public void UnpauseMusic(bool fadein = false, float fadeDuration = 1f, float volume = 1f)
        {
            if(musicAudioSource == null || MusicEnable == false || musicAudioSource.isPlaying == false)
            {
                return;
            }

            if(fadein)
            {
                FadeIn(musicAudioSource, volume, fadeDuration);
            }
            else
            {
                musicAudioSource.volume = volume;
            }
            musicAudioSource.UnPause();
        }
        #endregion

        #region SFX
        public bool SFXEnable
        {
            get => saver.GetSFXEnable() && MasterEnable;
            set
            {
                saver.SetSFXEnable(value);
            }
        }
        public float SFXVolume
        {
            get => saver.GetSFXVolume();
            set
            {
                saver.SetSFXVolume(value);
                mixer.SetFloat("SFXVolume", SoundHelper.ConvertToDecibel(SFXVolume));
				return;
                if(value == 0)
                {
                    SFXEnable = false;
                }
                else
                {
                    SFXEnable = true;
                }
            }
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if(sfxAudioSource == null || SFXEnable == false)
            {
                return;
            }
            sfxAudioSource.PlayOneShot(clip, volume);
        }

        public void PlayLoopSFX(AudioClip clip, float volume = 1)
        {
            if (sfxAudioSource == null || SFXEnable == false)
            {
                return;
            }
            sfxAudioSource.clip = clip;
            sfxAudioSource.loop = true;
            sfxAudioSource.volume = volume;
            sfxAudioSource.Play();
        }

        public void StopLoopSFX()
        {
            if (sfxAudioSource == null || sfxAudioSource.isPlaying == false)
            {
                return;
            }
            sfxAudioSource.loop = false;
            sfxAudioSource.Stop();
        }
        #endregion


        private void FadeIn(AudioSource audio, float toVolume, float duration = 1)
        {
            StartCoroutine(IEFadeSound(audio, 0, toVolume, duration));
        }

        private void FadeOut(AudioSource audio, float duration = 1, Action onCompleted = null)
        {
            StartCoroutine(IEFadeSound(audio, audio.volume, 0, duration, onCompleted));
        }

        IEnumerator IEFadeSound(AudioSource audio, float froVolume, float toVolume, float duration = 1, Action onCompleted = null)
        {
            if(audio == null)
            {
                Debug.LogWarning(string.Format("[SoundManager] Fade Sound Fall! Null audio {0}", audio.name));
                yield break;
            }
            float t = 0;
            while(t < duration)
            {
                t += Time.deltaTime;
                audio.volume = Mathf.Lerp(froVolume, toVolume, t / duration);
                yield return null;
            }
            audio.volume = toVolume;
            onCompleted?.Invoke();
        }
    }
}
