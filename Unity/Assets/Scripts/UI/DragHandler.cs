using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DragHandler : MonoBehaviour, IDragHandler
{
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.Translate(eventData.delta.x, eventData.delta.y, 0);
        rectTransform.ClampToScreen();
    }
}
