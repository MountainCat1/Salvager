using System;
using UnityEngine;

namespace Managers
{
    public interface ISignalManager
    {
        event Action<Signals> Signaled;
        void Signal(Signals signal);
    }


    public class SignalManager : ISignalManager
    {
        public event Action<Signals> Signaled;

        public void Signal(Signals signal)
        {
            Debug.Log($"Signal {signal} called");
            Signaled?.Invoke(signal);
        }
    }
}