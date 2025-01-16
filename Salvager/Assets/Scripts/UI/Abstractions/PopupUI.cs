using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Abstractions
{
    public class PopupUI : MonoBehaviour
    {
        [Inject] private IInputManager _inputManager;

        private RectTransform _rectTransform;
        private Canvas _canvas;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }


        private void Start()
        {
            _inputManager.OnCancel += OnCancel;


            OnShow();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Optional: Bring the panel to the front when clicked
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_canvas == null) return;

            // Move the panel based on drag delta
            Vector2 moveDelta = eventData.delta / _canvas.scaleFactor;
            _rectTransform.anchoredPosition += moveDelta;
        }

        private void OnDestroy()
        {
            _inputManager.OnCancel -= OnCancel;

            OnHide();
        }

        public void Show(bool show)
        {
            if (show)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OnHide();
        }

        private void OnCancel()
        {
            gameObject.SetActive(false);
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnShow()
        {
        }
    }
}