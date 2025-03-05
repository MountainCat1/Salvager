using LevelSelector.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class GameEventPanelUI : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private IGameEventManager _gameEventManager;
        
        [SerializeField] private GameObject eventPanel;
        [SerializeField] private Button eventOptionPrefab;
        [SerializeField] private Transform eventOptionsContainer;
        
        [SerializeField] private TextMeshProUGUI eventNameText;
        [SerializeField] private TextMeshProUGUI eventDescriptionText;
        
        // Unity Methods
        private void Start()
        {
            //
            eventPanel.SetActive(false);
            
            _gameEventManager.EventTriggered += DisplayEvent;
        }

        private void Update()
        {
            //
        }
        
        public void DisplayEvent(GameEvent gameEvent)
        {
            eventPanel.SetActive(true);
            eventNameText.text = gameEvent.Name;
            eventDescriptionText.text = gameEvent.Description;
            
            foreach (Transform child in eventOptionsContainer)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var option in gameEvent.Options)
            {
                var buttonGo = _diContainer.InstantiatePrefab(eventOptionPrefab, eventOptionsContainer);
                var buttonText = buttonGo.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = option.Name;
                buttonGo.GetComponent<Button>().onClick.AddListener(() => OnOptionSelected(gameEvent, option));
            }
        }

        private void OnOptionSelected(GameEvent gameEvent, GameEventOption option)
        {
            eventPanel.SetActive(false);
         
            option.OnClick?.Invoke();
        }
    }
}