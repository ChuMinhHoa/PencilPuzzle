using UnityEngine;
using UnityEngine.EventSystems;
namespace Core.UI.Other
{
    public class ScrollRectDragCheck : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [field: SerializeField] public bool IsDragging { get; private set; }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
        }
    }
}