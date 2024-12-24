using System;

namespace Managers
{
    public interface IWinManager
    {
        event Action Won;
        public void Win();
    }

    public class WinManager : IWinManager
    {
        public event Action Won;

        public void Win()
        {
            Won?.Invoke();
        }
    }
}