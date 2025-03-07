using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers
{
    public interface ISoundManager
    {
    }

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Animator))]
    public class SoundtrackPlayer : MonoBehaviour, ISoundManager
    {
        [SerializeField] private List<AudioClip> soundtracks;

        private AudioClip _lastSoundtrack;
        private AudioSource _soundtrackAudioSource;
        private Animator _soundtrackAnimator;


        private void Awake()
        {
            _soundtrackAudioSource = GetComponent<AudioSource>();
            _soundtrackAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            PlaySoundtrack(soundtracks.RandomElement());
        }

        private void Update()
        {
            // check if the soundtrack ended by comparing the length of the audio clip and the time passed
            if (_soundtrackAudioSource.clip != null &&
                _soundtrackAudioSource.clip.length - _soundtrackAudioSource.time < 0.1f)
                PlayNextSoundtrack();
        }

        private void PlayNextSoundtrack()
        {
            if (soundtracks.Count == 0)
                return;
            if (soundtracks.Count == 1)
            {
                PlaySoundtrack(soundtracks.First());
                return;
            }

            var nextSoundtrack = soundtracks.Except(new[] { _lastSoundtrack }).RandomElement();
            PlaySoundtrack(nextSoundtrack);
        }

        private void PlaySoundtrack(AudioClip audioClip)
        {
            _soundtrackAudioSource.clip = audioClip;
            _soundtrackAudioSource.Play();
            _lastSoundtrack = audioClip;
        }
    }
}