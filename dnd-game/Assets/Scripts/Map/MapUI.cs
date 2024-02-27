using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance { get; private set; }

    public GameObject ModeButtonsContainer;
    public GameObject ModeButtonPrefab;


    Mode _currentMode;
    public Mode CurrentMode
    {
        get => _currentMode;
        set
        {
            _currentMode = value;

            foreach (var modeButton in modeButtons)
            {
                if (modeButton.Mode == value)
                {
                    modeButton.Button.colors = selectedColor;
                }
                else
                {
                    modeButton.Button.colors = normalColors;
                }
            }
        }
    }

    readonly List<ModeButton> modeButtons = new()
    {
        new() {
            Mode = Mode.Drag,
            IconName = "hand",
        },
        new() {
            Mode = Mode.Draw,
            IconName = "brush-01",
        }
    };

    readonly ColorBlock normalColors = new()
    {
        normalColor = new(0.7f, 0.7f, 0.7f, 0.5f),
        highlightedColor = new(1f, 1f, 1f, 0.8f),
        pressedColor = new Color32(200, 200, 200, byte.MaxValue),
        selectedColor = new Color32(245, 245, 245, byte.MaxValue),
        disabledColor = new Color32(200, 200, 200, 128),
        colorMultiplier = 1f,
        fadeDuration = 0.1f
    };

    readonly ColorBlock selectedColor = new()
    {
        normalColor = new(1, 1, 1, 1),
        highlightedColor = new(1f, 1f, 1f, 0.8f),
        pressedColor = new Color32(200, 200, 200, byte.MaxValue),
        selectedColor = new Color32(245, 245, 245, byte.MaxValue),
        disabledColor = new Color32(200, 200, 200, 128),
        colorMultiplier = 1f,
        fadeDuration = 0.1f
    };

    Camera cam;

    void Start()
    {
        CurrentMode = Mode.Drag;
    }

    void Update()
    {

    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        foreach (var modeButton in modeButtons)
        {
            var button = Instantiate(ModeButtonPrefab, ModeButtonsContainer.transform);
            button.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Icons/{modeButton.IconName}");

            var buttonComp = button.GetComponent<Button>();
            buttonComp.colors = normalColors;
            var mode = modeButton.Mode;
            buttonComp.onClick.AddListener(() => OnClickMode(mode));

            modeButton.Button = buttonComp;
        }

        cam = Camera.main;
    }

    public void OnClickMode(Mode mode)
    {
        CurrentMode = mode;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var ray = cam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out var hit, 30f))
        {
            if (hit.collider.gameObject == MapRenderer.Instance.gameObject)
            {
                MapManager.Instance.OnClick(hit.point);
            }
        }
    }

    public void OnClick(PointerEventData eventData)
    {
        var ray = cam.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out var hit, 30f))
        {
            if (hit.collider.gameObject == MapRenderer.Instance.gameObject)
            {
                MapManager.Instance.OnClick(hit.point);
            }
        }
    }
}



public enum Mode
{
    Drag,
    Draw
}

class ModeButton
{
    public Mode Mode;
    public string IconName;
    public Button Button;
}