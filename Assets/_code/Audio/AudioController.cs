using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Vopere.Common;

namespace MegaGame
{
    [Serializable]
    public class MusicSample
    {
        public string name;
        public AudioClip clip;
    }

    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance;

        [Header("Music")]
        public AudioSource musicSource;
        public AudioMixerGroup musicMixer;
        public List<MusicSample> musicSamples = new List<MusicSample>();

        [Header("UI")]
        public AudioSource UISource;
        public AudioMixerGroup UIMixer;

        [Header("SFX")]
        public AudioSource SFXSource;
        public AudioMixerGroup SFXMixer;

        [Header("Music")]
        public AudioSource voiceSource;
        public AudioMixerGroup voiceMixer;

        int currentMusic = -1;

        float currentMusicTime;

        bool isRandomPlaying = true;
        bool isPaused = false;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create AudioController");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (musicSource.clip != null && !isPaused)
            {
                currentMusicTime -= Time.deltaTime;

                if (currentMusicTime <= 0)
                {
                    if (musicSource.loop)
                        return;
                
                    if (isRandomPlaying)
                        PlayRandomMusicClip();
                    else
                        PlayNextMusicClip();
                }
            }
        }

        public void Init()
        {
            if (musicSource.volume > 0)
                PlayRandomMusicClip();

            float musicVolume = DataSaveLoad.Instance.GetSavedFloat("MusicVolume");

            if (musicVolume != -1)
                ChangeVolume(0, musicVolume);

            float UIVolume = DataSaveLoad.Instance.GetSavedFloat("UIVolume");

            if (UIVolume != -1)
                ChangeVolume(1, UIVolume);
        }

        void PlayMusicClip()
        {
            if (currentMusic < 0)
                currentMusic = musicSamples.Count - 1;
            else if (currentMusic >= musicSamples.Count)
                currentMusic = 0;

            musicSource.Stop();
            musicSource.clip = musicSamples[currentMusic].clip;
            musicSource.Play();

            currentMusicTime = musicSource.clip.length;
            isPaused = false;
        }

        public void PlayNextMusicClip()
        {
            if (isRandomPlaying)
                PlayRandomMusicClip();
            else
            {
                currentMusic++;
                PlayMusicClip();
            }
        }

        public void PlayPrevMusicClip()
        {
            currentMusic--;
            PlayMusicClip();
        }

        void PlayRandomMusicClip()
        {
            int randomValue = UnityEngine.Random.Range(0, musicSamples.Count);

            if (randomValue == currentMusic)
                PlayNextMusicClip();
            else
            {
                currentMusic = randomValue;
                PlayMusicClip();
            }
        }

        public void SetMusicLoopPlaying(bool state)
        {
            musicSource.loop = state;
        }

        public void PlayCurrentMusic()
        {
            musicSource.UnPause();
            isPaused = false;
        }

        public void PauseCurrentMusic()
        {
            musicSource.Pause();
            isPaused = true;
        }

        public int GetCurrentMusicId()
        {
            return currentMusic;
        }

        public void SetRandomPlaying(bool state)
        {
            isRandomPlaying = state;
        }

        public void PlayUIAudioClip(AudioClip clip)
        {
            UISource.PlayOneShot(clip);
        }

        public void ChangeVolume(int group, float INvalue)
        {
            float value = (INvalue - 100) / 4.0f;

            if (INvalue <= 0)
                value = -80;

            if (group == 0)
                SetVolume(musicMixer, INvalue, value);
            else if (group == 1)
                SetVolume(UIMixer, INvalue, value);
            else if (group == 2)
                SetVolume(SFXMixer, INvalue, value);
            else if (group == 3)
                SetVolume(voiceMixer, INvalue, value);
        }

        void SetVolume(AudioMixerGroup mixerGroup, float INvalue, float value)
        {
            mixerGroup.audioMixer.SetFloat(mixerGroup.name + "Volume", value);
            DataSaveLoad.Instance.Save(mixerGroup.name + "Volume", INvalue);
        }
    }
 }
