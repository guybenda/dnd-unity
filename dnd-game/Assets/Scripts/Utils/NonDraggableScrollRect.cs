
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NonDraggableScrollRect : ScrollRect
{
    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
}