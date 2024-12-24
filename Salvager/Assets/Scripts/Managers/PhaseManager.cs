using System;
using UnityEngine;
using Zenject;

namespace Managers
{
    public interface IPhaseManager
    {
        int CurrentPhase { get; }

        static int EndPhase => 6;

        event Action<int> PhaseChanged;
    }

    public class PhaseManager : MonoBehaviour, IPhaseManager
    {
        [Inject] private ITimeManager _timeManager;
        [Inject] private IWinManager _winManager;
        
        public event Action<int> PhaseChanged;

        public int CurrentPhase { get; private set; } = 0;
        
        [SerializeField] private int firstPhaseTime;
        [SerializeField] private int secondPhaseTime;
        [SerializeField] private int thirdPhaseTime;
        [SerializeField] private int fourthPhaseTime;
        [SerializeField] private int fifthPhaseTime;
        
        [Inject] 
        private void Initialize()
        {
            _timeManager.NewSecond += OnNewSecond;
            _timeManager.TimeRunOut += OnTimeRunOut;
            
            _winManager.Won += OnWin;
        }

        private void OnWin()
        {
            CurrentPhase = -1;
            PhaseChanged?.Invoke(CurrentPhase);
        }

        private void OnTimeRunOut()
        {
            if(CurrentPhase == -1)
                return;
            
            CurrentPhase = 6;
            PhaseChanged?.Invoke(CurrentPhase);
            Debug.Log("The end...");
        }

        private void OnNewSecond(int seconds)
        {
            if(CurrentPhase == -1)
                return;
            
            if (seconds == firstPhaseTime && CurrentPhase == 0)
            {
                CurrentPhase = 1;
                PhaseChanged?.Invoke(CurrentPhase);
                Debug.Log("First phase");
            }
            else if (seconds == secondPhaseTime && CurrentPhase == 1)
            {
                CurrentPhase = 2;
                PhaseChanged?.Invoke(CurrentPhase);
                Debug.Log("Second phase");
            }
            else if (seconds == thirdPhaseTime && CurrentPhase == 2)
            {
                CurrentPhase = 3;
                PhaseChanged?.Invoke(CurrentPhase);
                Debug.Log("Third phase");
            }
            else if (seconds == fourthPhaseTime && CurrentPhase == 3)
            {
                CurrentPhase = 4;
                PhaseChanged?.Invoke(CurrentPhase);
                Debug.Log("Fourth phase");
            }
            else if (seconds == fifthPhaseTime && CurrentPhase == 4)
            {
                CurrentPhase = 5;
                PhaseChanged?.Invoke(CurrentPhase);
                Debug.Log("Fifth phase");
            }
        }
    }
}