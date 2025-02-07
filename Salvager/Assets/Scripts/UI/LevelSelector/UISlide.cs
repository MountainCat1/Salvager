using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class UISlide : MonoBehaviour
    {
        public RectTransform panel;
        public float slideDuration = 0.5f;

        private Vector2 hiddenPos;
        private Vector2 visiblePos;

        [SerializeField] private Ease showEase = Ease.InExpo;
        [SerializeField] private Ease hideEase = Ease.InExpo;

        private bool _shown = false;

        private void Start()
        {
            hiddenPos = panel.anchoredPosition;
            visiblePos = new Vector2(0, panel.anchoredPosition.y); // Adjust X for left/right sliding
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
            panel.DOAnchorPos(visiblePos, slideDuration).SetEase(showEase);
        }

        public void HidePanel()
        {
            _shown = false;
            panel.DOAnchorPos(hiddenPos, slideDuration).SetEase(hideEase);
        }
    }
}