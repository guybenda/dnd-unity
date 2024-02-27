using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCameraManager : MonoBehaviour
{
    public static MapCameraManager Instance { get; private set; }

    Vector3 targetPosition;
    Quaternion targetRotation;

    const float movementLerpSpeed = 15f;
    const float rotationLerpSpeed = 20f;
    const float dragMoveSpeed = 10f;
    const float dragRotateSpeed = 250f;
    const float keyboardMoveSpeed = 12f;
    const float scrollMoveSpeed = 1f;

    const float maxCameraHeight = 70f;
    const float minCameraHeight = 2f;

    const float minCameraPitch = 15f;
    const float maxCameraPitch = 90f;


    Camera cam;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.SetPositionAndRotation(
            Vector3.Lerp(cam.transform.position, targetPosition, 0.01f + movementLerpSpeed * Time.deltaTime),
            Quaternion.Lerp(cam.transform.rotation, targetRotation, 0.01f + rotationLerpSpeed * Time.deltaTime)
        );
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        cam = Camera.main;
        targetPosition = cam.transform.position;
        targetRotation = cam.transform.rotation;
    }

    Vector3 MovementVector(float x, float y, float z)
    {
        float scale = (cam.transform.position.y - minCameraHeight) / 10 + 1;
        return CamForward() * (z * scale) + CamRight() * (x * scale) + Vector3.up * (y * scale);
    }

    Vector3 CamForward()
    {
        var forward = cam.transform.forward + cam.transform.up;
        return new Vector3(forward.x, 0, forward.z).normalized;
    }

    Vector3 CamRight()
    {
        var forward = cam.transform.right;
        return new Vector3(forward.x, 0, forward.z).normalized;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float x = eventData.delta.x / cam.pixelWidth;
        float y = eventData.delta.y / cam.pixelHeight;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var euler = targetRotation.eulerAngles;
            euler.x = math.clamp(euler.x - (y * dragRotateSpeed), minCameraPitch, maxCameraPitch);
            euler.y += x * dragRotateSpeed;
            targetRotation.eulerAngles = euler;

        }
        else if (eventData.button == PointerEventData.InputButton.Middle || MapUI.Instance.CurrentMode == Mode.Drag)
        {
            targetPosition += MovementVector(x, 0, y) * -dragMoveSpeed;
        }
        else
        {
            MapUI.Instance.OnDrag(eventData);
        }
    }

    public void OnClick(PointerEventData eventData)
    {
        if (MapUI.Instance.CurrentMode == Mode.Drag)
        {
            return;
        }

        MapUI.Instance.OnClick(eventData);
    }

    public void OnMove(Vector3 movement)
    {
        Vector3 newPosition = targetPosition + keyboardMoveSpeed * Time.deltaTime * MovementVector(movement.x, movement.y, movement.z);
        newPosition.y = math.clamp(newPosition.y, minCameraHeight, maxCameraHeight);
        targetPosition = newPosition;
    }

    public void OnScroll(float scroll)
    {
        targetPosition.y = math.clamp(targetPosition.y - (scroll * scrollMoveSpeed), minCameraHeight, maxCameraHeight);
    }

    public void OrientCamera()
    {
        targetRotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
    }

    public void CenterCamera()
    {
        targetPosition.x = 0f;
        targetPosition.z = 0f;
    }
}
