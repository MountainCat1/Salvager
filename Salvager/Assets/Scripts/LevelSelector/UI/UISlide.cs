using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UISlide : MonoBehaviour
    {
        public event Action Showed;
        
        public RectTransform panel;
        public float slideDuration = 0.5f;

        [SerializeField]
        private SlideDirection slideDirection = SlideDirection.Horizontal;

        [SerializeField]
        private Ease showEase = Ease.InExpo;

        [SerializeField]
        private Ease hideEase = Ease.InExpo;

        private Vector2 _hiddenPos;
        private Vector2 _visiblePos;
        private bool _shown = false;

        private void Start()
        {
            CachePositions();
        }

        public void TogglePanel()
        {
            if (_shown)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }

        public void ShowPanel()
        {
            _shown = true;
            panel.DOAnchorPos(_visiblePos, slideDuration).SetEase(showEase);
            Showed?.Invoke();
        }

        public void HidePanel()
        {
            _shown = false;
            panel.DOAnchorPos(_hiddenPos, slideDuration).SetEase(hideEase);
        }

        private void CachePositions()
        {
            _hiddenPos = panel.anchoredPosition;

            switch (slideDirection)
            {
                case SlideDirection.Horizontal:
                    _visiblePos = new Vector2(
                        0,
                        panel.anchoredPosition.y
                    ); // Adjust X for left/right sliding
                    break;
                case SlideDirection.Vertical:
                    _visiblePos = new Vector2(
                        panel.anchoredPosition.x,
                        0
                    ); // Adjust Y for up/down sliding
                    break;
                default:
                    _visiblePos = panel.anchoredPosition;
                    break;
            }
        }

        public enum SlideDirection
        {
            Horizontal,
            Vertical,
        }
    }
}
