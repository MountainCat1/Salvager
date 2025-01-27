
using UnityEngine;
using UnityEngine.EventSystems;
namespace UI
{
   

    public class DraggablePanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector2 _originalPosition;

        // Called when dragging starts
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Save the current position when the drag starts
            _originalPosition = transform.position;
        }

        // Called during dragging
        public void OnDrag(PointerEventData eventData)
        {
            // Update the position of the panel based on the drag delta
            transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        }

        // Optional: Called when dragging ends
        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }

}