using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Managers
{
    public enum SoundType
    {
        Sfx,
        Music,
        UI
    }

    public interface ISoundPlayer
    {
        public void PlaySound(AudioClip clip, Vector2 position, SoundType soundType = SoundType.Sfx,
            float pitchRandomness = 0f);

        public void PlaySoundGlobal(AudioClip clip, SoundType soundType = SoundType.Sfx);
        AudioSource CreateSound(AudioClip clip, SoundType soundType, bool destroy = true);
        void AddAudioSource(AudioSource audioSource, SoundType soundType);
        void RemoveAudioSource(AudioSource audioSource, SoundType soundType);
    }

    public class SoundPlayer : ISoundPlayer
    {
        private const float DelayToDestroyNonPlayingAudioSource = 0.5f;

        // TODO - use pool instead of dictionary because it's faster
        private readonly Dictionary<SoundType, IList<AudioSource>> _audioSources = new();
        private readonly Dictionary<SoundType, float> _volumes = new();

        // [Inject] private IGameSettingsAccessor _settingsAccessor;

        [Inject]
        private void Construct()
        {
            _audioSources[SoundType.Music] = new List<AudioSource>();
            _audioSources[SoundType.Sfx] = new List<AudioSource>();
            _audioSources[SoundType.UI] = new List<AudioSource>();

            // TODO: Volumes shoudl be loaded from settings
            _volumes[SoundType.Music] = 0.15f;
            _volumes[SoundType.Sfx] = 1f;
            _volumes[SoundType.UI] = 0.7f;
            // _volumes[SoundType.Music] = _settingsAccessor.Settings.muiscVolume;
            // _volumes[SoundType.Sfx] = _settingsAccessor.Settings.sfxVolume;
            // _volumes[SoundType.UI] = _settingsAccessor.Settings.uiVolume;

            // _settingsAccessor.Changed += ApplyVolumeChange;
        }

        // private void ApplyVolumeChange(GameSettings gameSettings)
        // {
        //     ChangeVolume(SoundType.Music, gameSettings.muiscVolume);
        //     ChangeVolume(SoundType.Sfx, gameSettings.sfxVolume);
        //     ChangeVolume(SoundType.UI, gameSettings.uiVolume);
        // }

        public void AddAudioSource(AudioSource audioSource, SoundType soundType)
        {
            _audioSources[soundType].Add(audioSource);
            audioSource.volume = _volumes[soundType];
        }

        public void RemoveAudioSource(AudioSource audioSource, SoundType soundType)
        {
            _audioSources[soundType].Remove(audioSource);
        }

        public void PlaySound(AudioClip clip, Vector2 position, SoundType soundType = SoundType.Sfx, float pitchRandomness = 0f)
        {
            if (clip is null)
                Debug.LogWarning($"Missing sound!");

            var audioSource = CreateSound(clip, soundType);
            audioSource.transform.position = position;

            audioSource.pitch = 1 + Random.Range(-pitchRandomness, pitchRandomness);
            audioSource.spatialBlend = 1;
        }

        public void PlaySoundGlobal(AudioClip clip, SoundType soundType = SoundType.Sfx)
        {
            if (clip is null)
                Debug.LogWarning($"Missing sound!");

            var audioSource = CreateSound(clip, soundType);

            audioSource.maxDistance = int.MaxValue;
            audioSource.spatialBlend = 0;
        }

        public void ChangeVolume(SoundType soundType, float targetVolume)
        {
            _volumes[soundType] = targetVolume;

            // TODO: this is a hack, fix it
            _audioSources[soundType] = _audioSources[soundType]
                .Where(x => x) // this checks if an object was destroyed
                .ToList();

            foreach (var audioSource in _audioSources[soundType])
            {
                audioSource.volume = targetVolume;
            }
        }

        public AudioSource CreateSound(AudioClip clip, SoundType soundType, bool destroy = true)
        {
            GameObject audioObject = new GameObject("AudioPlayer");
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();

            var volume = _volumes[soundType];

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            _audioSources[soundType].Add(audioSource);

            if (destroy)
                Object.Destroy(audioObject, clip.length + DelayToDestroyNonPlayingAudioSource);

            return audioSource;
        }
    }
}