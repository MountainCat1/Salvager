using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class GameTimeDisplayUI : MonoBehaviour
    {
        [Inject] private ITimeManager _timeManager;
        
        [SerializeField] private TextMeshProUGUI afterDotText;
        [SerializeField] private TextMeshProUGUI beforeDotText;
        
        private void Update()
        {
            var gameTime = _timeManager.TimeTillEnd;
            var gameTimeString = gameTime.ToString("F2");
            var split = gameTimeString.Split('.');
            beforeDotText.text = int.Parse(split[0]).ToString();
            afterDotText.text = split[1];
        }
    }
}