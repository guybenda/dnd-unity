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
    public GameObject ModeButtonPrefab;
    public TMP_Text StatusText;

    const float maxRaycastDistance = 160f;

    List<RaycastResult> raycastResults = new();
    PointerEventData pointerData = new(EventSystem.current);

    Mode _currentMode;
    public Mode CurrentMode
    {
        get => _currentMode;
        set
        {
            _currentMode = value;

            foreach (var modeButton in ModeButton.Buttons)
            {
                if (modeButton.Mode == value)
                {
                    modeButton.Button.colors = ModeButton.SelectedColor;
                }
                else
                {
                    modeButton.Button.colors = ModeButton.NormalColors;
                }
            }

            var tilesSelector = GetComponentInChildren<MapTileSelectorScript>(true);
            tilesSelector.gameObject.SetActive(value == Mode.Draw);
            tilesSelector.Expanded = value == Mode.Draw;

            if (value == Mode.Erase)
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
        CurrentMode = Mode.Drag;
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

        foreach (var modeButton in ModeButton.Buttons)
        {
            var button = Instantiate(ModeButtonPrefab, ModeButtonsContainer.transform);
            button.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Icons/{modeButton.IconName}");

            var buttonComp = button.GetComponent<Button>();
            buttonComp.colors = ModeButton.NormalColors;
            buttonComp.name = modeButton.Mode.ToString();

            var mode = modeButton.Mode;
            buttonComp.onClick.AddListener(() => OnClickMode(mode));

            modeButton.Button = buttonComp;
        }

        cam = Camera.main;
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

    public void OnClickMode(Mode mode)
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




public enum Mode
{
    Drag,
    Draw,
    Erase,
}

class ModeButton
{
    public Mode Mode;
    public string IconName;
    public Button Button;


    public static readonly List<ModeButton> Buttons = new()
    {
        new() {
            Mode = Mode.Drag,
            IconName = "hand",
        },
        new() {
            Mode = Mode.Draw,
            IconName = "brush-01",
        },
        new() {
            Mode = Mode.Erase,
            IconName = "eraser",
        }
    };


    public static readonly ColorBlock NormalColors = new()
    {
        normalColor = new(0.7f, 0.7f, 0.7f, 0.5f),
        highlightedColor = new(1f, 1f, 1f, 0.8f),
        pressedColor = new Color32(200, 200, 200, byte.MaxValue),
        selectedColor = new Color32(245, 245, 245, byte.MaxValue),
        disabledColor = new Color32(200, 200, 200, 128),
        colorMultiplier = 1f,
        fadeDuration = 0.1f
    };

    public static readonly ColorBlock SelectedColor = new()
    {
        normalColor = new(1, 1, 1, 1),
        highlightedColor = new(1f, 1f, 1f, 0.8f),
        pressedColor = new Color32(200, 200, 200, byte.MaxValue),
        selectedColor = new Color32(245, 245, 245, byte.MaxValue),
        disabledColor = new Color32(200, 200, 200, 128),
        colorMultiplier = 1f,
        fadeDuration = 0.1f
    };
}