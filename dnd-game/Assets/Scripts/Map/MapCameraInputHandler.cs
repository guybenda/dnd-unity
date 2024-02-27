using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCameraInputHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    List<RaycastResult> raycastResults = new();
    PointerEventData pointerData = new(EventSystem.current);
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var mousePos = Input.mousePosition;
        pointerData.position = mousePos;
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            if (raycastResults[0].gameObject == gameObject)
            {
                HandleMovementKeys();
                HandleScroll();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        MapCameraManager.Instance.OnDrag(eventData);
    }

    void HandleMovementKeys()
    {
        Vector3 moveVector = new(Input.GetAxis("Horizontal"), Input.GetAxis("Lateral"), Input.GetAxis("Vertical"));

        if (moveVector == Vector3.zero) return;

        MapCameraManager.Instance.OnMove(moveVector);
    }

    void HandleScroll()
    {
        // var scroll = Input.GetAxis("Mouse ScrollWheel");
        var scroll = Input.mouseScrollDelta.y;

        if (scroll == 0f) return;

        MapCameraManager.Instance.OnScroll(scroll);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            MapCameraManager.Instance.OnClick(eventData);
        }
    }
}
