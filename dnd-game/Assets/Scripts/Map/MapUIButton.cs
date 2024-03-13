using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class MapUIButton<T>
{
    public T Mode;
    public string IconAssetName;
    public string HoverText;

    public Button Button;



    public Button Create(GameObject prefab, Transform parent, bool brightColor = false)
    {
        var button = GameObject.Instantiate(prefab, parent);
        button.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Icons/{IconAssetName}");

        var buttonComp = button.GetComponent<Button>();
        buttonComp.colors = brightColor ? MapUIButton.SelectedColor : MapUIButton.NormalColor;
        buttonComp.name = HoverText ?? Mode.ToString();

        Button = buttonComp;
        return buttonComp;
    }
}

class MapUIButton
{
    public static readonly List<MapUIButton<MapMode>> ModeButtons = new()
    {
        new() {
            Mode = MapMode.Drag,
            IconAssetName = "hand",
        },
        new() {
            Mode = MapMode.Draw,
            IconAssetName = "brush-01",
        },
        new() {
            Mode = MapMode.Erase,
            IconAssetName = "eraser",
        }
    };

    public static readonly List<MapUIButton<MapAction>> ActionButtons = new()
    {
        new() {
            Mode = MapAction.RealignCamera,
            HoverText = "Realign Camera",
            IconAssetName = "compass-03",
        },
        new() {
            Mode = MapAction.CenterCamera,
            HoverText = "Center Camera",
            IconAssetName = "marker-pin-06",
        },
        new() {
            Mode = MapAction.Save,
            IconAssetName = "save-01",
        },
        new() {
            Mode = MapAction.Discard,
            HoverText = "Discard Changes",
            IconAssetName = "trash-03",
        }
    };

    public static readonly ColorBlock NormalColor = new()
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