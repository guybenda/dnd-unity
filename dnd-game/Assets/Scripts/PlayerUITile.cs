using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUITile : MonoBehaviour
{
    public TMP_Text Name;
    public Image MainColor;
    public UnityEngine.UI.Outline SecondaryColor;
    public Toggle CanRoll;
    public Button KickButton;

    public event Action<bool> OnChangeCanRoll;

    bool didInit = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            CanRoll.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        CanRoll.onValueChanged.AddListener(OnCanRollChanged);
    }

    void OnCanRollChanged(bool value)
    {
        OnChangeCanRoll?.Invoke(value);
    }

    public void SetPlayer(Player player)
    {
        var isCurrentPlayer = player.Email == NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().Email;

        if (!didInit)
        {
            transform.name = player.Email;
            KickButton.onClick.AddListener(player.OnKick);
            didInit = true;
        }

        if (player.User == null)
        {
            Name.text = "Loading...";
        }
        else
        {
            Name.text = player.User.DisplayName.ToString();
            MainColor.color = player.User.Dice.MainColor;
            SecondaryColor.effectColor = player.User.Dice.SecondaryColor;
            CanRoll.SetIsOnWithoutNotify(player.IsAllowedToRoll);
        }

        KickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && !isCurrentPlayer);
        CanRoll.gameObject.SetActive(!(isCurrentPlayer && NetworkManager.Singleton.IsServer));

        OnChangeCanRoll = player.OnChangeCanRoll;
    }
}
