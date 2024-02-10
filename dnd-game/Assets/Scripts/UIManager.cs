using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject MainGUI;
    public GameObject PlayersContainer;
    public GameObject RollButtons;

    public GameObject TabGUI;

    public TMP_Text DiceHoverText;
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

                if (highlightedDice.IsStatic)
                {
                    var result = highlightedDice.Result();
                    DiceHoverText.text = result.ToString();
                    DiceHoverText.rectTransform.position = mousePos - new Vector3(4, 0, 0);

                    if (highlightedDice.Type != DndCommon.DiceType.D100)
                    {
                        if (result == 1)
                        {
                            DiceHoverText.color = Color.red;
                        }
                        else if (result == (int)highlightedDice.Type)
                        {
                            DiceHoverText.color = Color.green;
                        }
                        else
                        {
                            DiceHoverText.color = Color.white;
                        }
                    }
                    else
                    {
                        DiceHoverText.color = Color.white;
                    }
                }
            }

        }
        else
        {
            highlightedDice = null;
            DiceHoverText.text = string.Empty;
            DiceHoverText.rectTransform.position = new Vector3(1000, 1000, 0);
        }

        if (prevHighlightedDice != null && prevHighlightedDice != highlightedDice)
        {
            prevHighlightedDice.IsHovered = false;
        }
    }

    void FixedUpdate()
    {
        var total = diceContainer.DiceTotal;
        var breakdown = diceContainer.DiceBreakdown;

        if (breakdown == "")
        {
            resultText.text = "";
            return;
        }

        resultText.text = $"{breakdown} = {total}";
    }

    public PlayerUITile GetPlayerUIByEmail(string email)
    {
        return PlayersContainer.transform.Find(email)?.GetComponent<PlayerUITile>();
    }

    public void AddPlayerUI(Player player)
    {
        var playerUI = Instantiate(playerUIPrefab, PlayersContainer.transform).GetComponent<PlayerUITile>();
        playerUI.SetPlayer(player);

        // TODO order
        // var order = 
    }

    public void UpdatePlayerUI(Player player)
    {
        if (string.IsNullOrEmpty(player.Email))
        {
            return;
        }

        var playerUI = GetPlayerUIByEmail(player.Email);
        if (playerUI != null)
        {
            playerUI.SetPlayer(player);
        }
        else
        {
            AddPlayerUI(player);
        }

        if (player != GameManager.Instance.CurrentPlayer()) return;

        RollButtons.SetActive(player.IsAllowedToRoll);
    }

    public void RemovePlayerUI(string email)
    {
        var playerUI = GetPlayerUIByEmail(email);
        if (playerUI != null)
        {
            Destroy(playerUI.gameObject);
        }
    }

    public void OnExit()
    {
        NetworkManager.Singleton.Shutdown();
        SceneTransitionManager.Instance.MainMenu();
    }
}
