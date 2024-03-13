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
        HandleMovementKeys();

        var mousePos = Input.mousePosition;
        pointerData.position = mousePos;
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            if (raycastResults[0].gameObject == gameObject)
            {
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
        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        moveVector.y += Input.GetAxis("Lateral");

        if (moveVector == Vector3.zero) return;

        MapCameraManager.Instance.OnMove(moveVector);
    }

    void HandleScroll()
    {
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
