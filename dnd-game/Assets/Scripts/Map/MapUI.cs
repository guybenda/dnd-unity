using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance { get; private set; }

    public GameObject ModeButtonsContainer;
    public GameObject ActionButtonsContainer;
    public GameObject ButtonPrefab;
    public TMP_Text StatusText;

    const float maxRaycastDistance = 160f;

    List<RaycastResult> raycastResults = new();
    PointerEventData pointerData;

    MapMode _currentMode;
    public MapMode CurrentMode
    {
        get => _currentMode;
        set
        {
            _currentMode = value;

            foreach (var modeButton in MapUIButton.ModeButtons)
            {
                if (modeButton.Mode == value)
                {
                    modeButton.Button.colors = MapUIButton.SelectedColor;
                }
                else
                {
                    modeButton.Button.colors = MapUIButton.NormalColor;
                }
            }

            var tilesSelector = GetComponentInChildren<MapTileSelectorScript>(true);
            tilesSelector.gameObject.SetActive(value == MapMode.Draw);
            tilesSelector.Expanded = value == MapMode.Draw;

            if (value == MapMode.Erase)
            {
                CurrentTileType = TileType.Empty;
            }
        }
    }

    TileType _currentTileType = TileType.CobblestoneA;
    public TileType CurrentTileType
    {
        get => _currentTileType;
        set
        {
            _currentTileType = value;
        }
    }

    Camera cam;

    void Start()
    {
        CurrentMode = MapMode.Drag;
    }

    void Update()
    {
        HandleStatusText();
    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        foreach (var modeButton in MapUIButton.ModeButtons)
        {
            var button = modeButton.Create(ButtonPrefab, ModeButtonsContainer.transform);

            var mode = modeButton.Mode;
            button.onClick.AddListener(() => OnClickMode(mode));
        }

        foreach (var modeButton in MapUIButton.ActionButtons)
        {
            var button = modeButton.Create(ButtonPrefab, ActionButtonsContainer.transform, true);

            var action = modeButton.Mode;
            button.onClick.AddListener(() => OnClickAction(action));
        }

        cam = Camera.main;
        pointerData = new(EventSystem.current);
    }

    void HandleStatusText()
    {
        var mousePos = Input.mousePosition;
        pointerData.position = mousePos;
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            var target = raycastResults[0];

            if (target.gameObject.TryGetComponent<Button>(out var button))
            {
                StatusText.text = button.name;
                return;
            }
        }

        var ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out var hit, maxRaycastDistance))
        {
            if (hit.collider.gameObject == MapRenderer.Instance.gameObject)
            {
                var position = MapRenderer.Instance.Tilemap.WorldToCell(hit.point);
                StatusText.text = $"{position.x}, {position.y}";
                return;
            }
        }

        StatusText.text = "";
    }

    void OnClickMode(MapMode mode)
    {
        CurrentMode = mode;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnClick(eventData);
    }

    public void OnClick(PointerEventData eventData)
    {
        var ray = cam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out var hit, maxRaycastDistance))
        {
            if (hit.collider.gameObject == MapRenderer.Instance.gameObject)
            {
                MapManager.Instance.DrawTile(hit.point, CurrentTileType);
            }
        }
    }

    async void OnClickAction(MapAction action)
    {
        switch (action)
        {
            case MapAction.RealignCamera:
                MapCameraManager.Instance.OrientCamera();
                break;
            case MapAction.CenterCamera:
                MapCameraManager.Instance.CenterCamera();
                break;
            case MapAction.Save:
                Debug.Log("Save");
                await MapManager.Instance.Map.SaveData();
                Debug.Log("Save done");
                break;
            case MapAction.Discard:
                break;
        }
    }

    public void OnClickOrient()
    {
        MapCameraManager.Instance.OrientCamera();
    }

    public void OnClickCenter()
    {
        MapCameraManager.Instance.CenterCamera();
    }

    public void OnClickSave()
    {
        Debug.Log("Save");
    }
}




public enum MapMode
{
    Drag,
    Draw,
    Erase,
}

enum MapAction
{
    RealignCamera,
    CenterCamera,
    Save,
    Discard,
}
