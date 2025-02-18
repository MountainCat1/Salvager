namespace UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PreventAutoSelect  : MonoBehaviour
    {
        void Update()
        {
            // Clear selection every frame to REALLY prevent auto-selection
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        
    }

}