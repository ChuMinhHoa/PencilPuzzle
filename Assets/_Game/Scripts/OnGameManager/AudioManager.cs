using System;
using System.Collections.Generic;
using _Game.Scripts.GameEnum;
using _Game.Scripts.ScriptAbleObject;
using BaseGame.Scripts.Data;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts.OnGameManager
{
    public enum AudioKey
    {
        BgMainMenu = 0,
        BgGamePlay = 1,

        SfxPickObj = 2,
        SfxPutObj = 3,
        SfxPeopleDrop = 4,
        SfxHelicopter = 5,
        SfxMagnet = 6,
        SfxFreezeTime = 7,

        SfxUIClickBtn = 31,
        SfxUIWinGame = 32,
        SfxUILoseGame = 33,
        SfxUIRewardClaim = 34,
        SfxUINewObject = 35,
        SfxUICoinClaim = 36,
    }
    
    public class AudioManager : Singleton<AudioManager>
    {
        [field: SerializeField] public AudioConfig[] SoundFxArray { get; private set; }
        [field: SerializeField] public AudioConfig[] MusicArray { get; private set; }

        [field: SerializeField] public AudioSource SoundFxAudioSource { get; private set; }
        [field: SerializeField] public AudioSource MusicAudioSource { get; private set; }
        public UnityAction<bool> SoundFxChangeCallback { get; set; }
        private AudioClip CurrentMusicAudioClip { get; set; }
        private Dictionary<AudioKey, (int, DateTime)> LastPlaySoundFx { get; set; } = new();
        private MotionHandle ScaleVolumeHandle { get; set; }

        private void Start()
        {
            SettingData.Instance.GetSettingSubData(SettingType.Sound).Value.Subscribe(SetSoundFx).AddTo(this);
            SettingData.Instance.GetSettingSubData(SettingType.Music).Value.Subscribe(SetMusic).AddTo(this);
        }

        private AudioConfig GetFxAudioConfig(AudioKey audioKey)
        {
            AudioConfig audioConfig = null;
            for (int index = 0; index < SoundFxArray.Length; index++)
            {
                AudioConfig config = SoundFxArray[index];
                if (config.AudioKey != audioKey) continue;
                audioConfig = config;
                break;
            }

            return audioConfig;
        }

        private AudioConfig GetMusicAudioConfig(AudioKey audioKey)
        {
            AudioConfig audioConfig = null;
            for (int index = 0; index < MusicArray.Length; index++)
            {
                AudioConfig config = MusicArray[index];
                if (config.AudioKey != audioKey) continue;
                audioConfig = config;
                break;
            }

            return audioConfig;
        }

        public void PlaySoundFx(AudioKey audioKey, float delay = 0)
        {
            AudioConfig audioConfig = GetFxAudioConfig(audioKey);
            if (audioConfig == null) return;
            PlaySoundDelay(audioConfig.AudioClip.GetRandomElement(), delay).Forget();
        }

        public void PlaySoundFxStack(AudioKey audioKey, float stackTime, float delay = 0)
        {
            AudioConfig audioConfig = GetFxAudioConfig(audioKey);
            if (audioConfig == null) return;
            int stack = 0;
            if (LastPlaySoundFx.TryGetValue(audioKey, out (int, DateTime) value))
            {
                (int stackCount, DateTime lastPlayTime) = value;
                if (lastPlayTime.AddSeconds(stackTime) > DateTime.Now)
                {
                    stack = Mathf.Clamp(stackCount + 1, 0, audioConfig.AudioClip.Length - 1);
                }
            }

            LastPlaySoundFx[audioKey] = (stack, DateTime.Now);
            PlaySoundDelay(audioConfig.AudioClip[stack], delay).Forget();
        }

        public void PlaySoundFx(AudioClip audioClip, float delay = 0)
        {
            if (audioClip == null) return;
            PlaySoundDelay(audioClip, delay).Forget();
        }

        public void PlaySoundFxIndex(AudioKey audioKey, int index, float delay = 0)
        {
            AudioConfig audioConfig = GetFxAudioConfig(audioKey);
            if (audioConfig == null) return;
            PlaySoundDelay(audioConfig.AudioClip[index], delay).Forget();
        }

        public void PlayMusic(AudioKey audioKey)
        {
            AudioConfig audioConfig = GetMusicAudioConfig(audioKey);
            if (audioConfig == null) return;
            AudioClip newMusicAudioClip = audioConfig.AudioClip.GetRandomElement();
            if (CurrentMusicAudioClip == newMusicAudioClip) return;
            CurrentMusicAudioClip = newMusicAudioClip;
            MusicAudioSource.clip = CurrentMusicAudioClip;
            MusicAudioSource.Play();
        }

        public void ChangeMusic(AudioKey audioKey, float time)
        {
            ScaleVolumeHandle.TryCancel();
            AudioConfig audioConfig = GetMusicAudioConfig(audioKey);
            if (audioConfig == null) return;
            AudioClip newMusicAudioClip = audioConfig.AudioClip.GetRandomElement();
            if (CurrentMusicAudioClip == newMusicAudioClip) return;
            CurrentMusicAudioClip = newMusicAudioClip;
            ScaleVolumeHandle = LMotion.Create(MusicAudioSource.volume, 0f, time / 2f)
                .WithOnComplete(OnMusicScaleVolumeToAZero)
                .Bind(SetMusicAudioSourceVolume);
        }

        private void OnMusicScaleVolumeToAZero()
        {
            ScaleVolumeHandle.TryCancel();
            MusicAudioSource.clip = CurrentMusicAudioClip;
            MusicAudioSource.Play();
            ScaleVolumeHandle = LMotion.Create(0f, 1f, 0.2f)
                .Bind(SetMusicAudioSourceVolume);
        }

        private void SetMusicAudioSourceVolume(float x)
        {
            MusicAudioSource.volume = x;
        }

        private float GetMusicAudioSourceVolume()
        {
            return MusicAudioSource.volume;
        }

        private async UniTask PlaySoundDelay(AudioClip audioClip, float delay = 0)
        {
            if (delay < 0.01f)
            {
                SoundFxAudioSource.PlayOneShot(audioClip);
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy());
            SoundFxAudioSource.PlayOneShot(audioClip);
        }

        public void SetSoundFx(bool value)
        {
            SoundFxAudioSource.mute = !value;
            SoundFxChangeCallback?.Invoke(value);
        }

        public void SetMusic(bool value)
        {
            MusicAudioSource.mute = !value;
        }
    }
}