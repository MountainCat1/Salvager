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
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [Inject] private ISoundPlayer _soundPlayer;
        [Inject] private IPlayerCharacterProvider _playerProvider;
        [Inject] private IPhaseManager _phaseManager;

        [SerializeField] private List<AudioClip> soundtracks;
        
        private AudioClip _lastSoundtrack;
        private AudioSource _soundtrackAudioSource;
        private Animator _soundtrackAnimator;
        
        private static readonly int PlayerDeath = Animator.StringToHash("PlayerDeath");
        private static readonly int Phase = Animator.StringToHash("Phase");

        private void Awake()
        {
            _soundtrackAudioSource = GetComponent<AudioSource>();
            _soundtrackAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            _soundPlayer.AddAudioSource(_soundtrackAudioSource, SoundType.Music);
            PlaySoundtrack(soundtracks.First());
            
            var player = _playerProvider.Get();
            player.Death += OnPlayerDeath;
            
            _phaseManager.PhaseChanged += OnPhaseChanged;
        }

        private void OnPhaseChanged(int phase)
        {
            _soundtrackAnimator.SetInteger(Phase, phase);
        }

        private void OnPlayerDeath(DeathContext ctx)
        {
            _soundtrackAnimator.SetTrigger(PlayerDeath);
        }

        private void OnDestroy()
        {
            if (_soundPlayer != null)
            {
                _soundPlayer.RemoveAudioSource(_soundtrackAudioSource, SoundType.Music);
            }
        }

        private void Update()
        {
            if (!_soundtrackAudioSource.isPlaying)
                PlayNextSoundtrack();
        }

        private void PlayNextSoundtrack()
        {
            if(soundtracks.Count == 0)
                return;
            if (soundtracks.Count == 1)
            {
                PlaySoundtrack(soundtracks.First());
                return;
            }
            var nextSoundtrack = soundtracks.Except(new []{_lastSoundtrack}).RandomElement();
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