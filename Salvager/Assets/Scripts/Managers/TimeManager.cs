using System;
using ScriptableObjects;
using UnityEngine;
using Zenject;

public interface ITimeManager
{
    public event Action TimeRunOut;
    public event Action<int> NewSecond;
    
    public float GameTime { get; }
    public float TimeTillEnd { get; }
}

public class TimeManager : MonoBehaviour, ITimeManager
{
    [Inject] private IGameConfiguration _gameConfiguration;
    [Inject] private IInputManager _inputManager;

    public event Action TimeRunOut;
    public event Action<int> NewSecond;

    public float GameTime { get; private set; }
    public float TimeTillEnd => _gameConfiguration.GameTime - GameTime;
    
    private bool _timeRunOut;

    private int _lastSecond = -1;
    private float _lastTimeScale = 0;

    private void Start()
    {
        // _inputManager.SpeedDown += () => Time.timeScale -= 0.1f;
        // _inputManager.SpeedUp += () => Time.timeScale += 0.1f;
        // _inputManager.Pause += () =>
        // {
        //     if(Time.timeScale <= 0.01f)
        //     {
        //         Time.timeScale = _lastTimeScale;
        //     }
        //     else
        //     {
        //         _lastTimeScale = Time.timeScale;
        //         Time.timeScale = 0.01f;
        //     }
        // };
    }

    private void Update()
    {
        GameTime += Time.deltaTime;
        
        var currentSecond = Mathf.FloorToInt(GameTime);
        if (currentSecond != _lastSecond)
        {
            _lastSecond = currentSecond;
            NewSecond?.Invoke(currentSecond);
        }
        
        if (!_timeRunOut && GameTime >= _gameConfiguration.GameTime)
        {
            _timeRunOut = true;
            TimeRunOut?.Invoke();
        }
    }
}
