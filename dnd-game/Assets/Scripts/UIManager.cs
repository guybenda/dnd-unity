using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject MainGUI;
    public GameObject PlayersContainer;
    public GameObject RollButtons;

    public GameObject TabGUI;

    public GameObject playerUIPrefab;

    TextMeshProUGUI resultText;
    DiceContainer diceContainer;
    Camera cam;

    int diceMask;
    DiceScript highlightedDice;


    void Start()
    {

    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        resultText = GameObject.Find("ResultText").GetComponent<TextMeshProUGUI>();
        diceContainer = GameObject.Find("DiceContainer").GetComponent<DiceContainer>();
        cam = Camera.main;

        diceMask = LayerMask.GetMask("Dice");

    }

    void OnDestroy()
    {
        if (Instance != this) return;
        Instance = null;
    }

    void Update()
    {
        var mousePos = Input.mousePosition;
        var ray = cam.ScreenPointToRay(mousePos);

        var prevHighlightedDice = highlightedDice;

        if (Physics.Raycast(ray, out var hit, 20f, diceMask))
        {
            highlightedDice = hit.collider.gameObject.GetComponent<DiceScript>();
            if (highlightedDice != null)
            {
                highlightedDice.IsHovered = true;
            }

        }
        else
        {
            highlightedDice = null;
        }

        if (prevHighlightedDice != null && prevHighlightedDice != highlightedDice)
        {
            prevHighlightedDice.IsHovered = false;
        }
    }

    void FixedUpdate()
    {
        // counter = (counter + 1) % 10;
        // if (counter != 0) return;

        var total = diceContainer.DiceTotal;
        var breakdown = diceContainer.DiceBreakdown;

        if (breakdown == "")
        {
            resultText.text = "";
            return;
        }

        resultText.text = $"{breakdown} = {total}";
    }

    public void AddPlayer(Player player)
    {

    }
}
