using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MapTileSelectorScript : MonoBehaviour
{
    public GameObject TileImagePrefab;
    public GameObject TilesContainer;
    public GameObject TilesScrollView;

    bool _expanded = false;
    public bool Expanded
    {
        get => _expanded;
        set
        {
            _expanded = value;
            TilesScrollView.SetActive(value);
        }
    }

    void Start()
    {

    }

    void OnEnable()
    {
        OnClickTile((TileType)1, TilesContainer.transform.GetChild(0).gameObject);
    }

    void Update()
    {

    }

    void Awake()
    {
        var tileTypes = System.Enum.GetValues(typeof(TileType)).Cast<TileType>()
            .Where(tt => tt != TileType.Empty && tt != TileType.Unknown);

        foreach (var tileType in tileTypes)
        {
            var tileImage = Instantiate(TileImagePrefab, TilesContainer.transform);
            tileImage.GetComponent<Image>().sprite = tileType.SpriteAt(Vector2Int.zero);
            tileImage.GetComponent<Button>().onClick.AddListener(() => OnClickTile(tileType, tileImage));
            tileImage.name = Regex.Replace(tileType.ToString(), "(\\B[A-Z])", " $1");
        }
    }

    void OnClickTile(TileType tileType, GameObject selectedTile)
    {
        MapUI.Instance.CurrentTileType = tileType;

        foreach (var outline in TilesContainer.transform.GetComponentsInChildren<UnityEngine.UI.Outline>())
        {
            outline.enabled = false;
        }

        selectedTile.GetComponentInChildren<UnityEngine.UI.Outline>().enabled = true;
    }

    public void OnClickShowHide()
    {
        Expanded = !Expanded;
    }

}
