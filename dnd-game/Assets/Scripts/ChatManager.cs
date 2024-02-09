using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Instance { get; private set; }

    public bool IsHistoryVisible { get; private set; } = false;

    const int maxMessageLength = 140;
    const float messageCooldown = 0.5f;
    Dictionary<ulong, float> clientIdToMessageCooldown = new();

    GameObject MessagePrefab;

    void Start()
    {

    }

    void Update()
    {
        foreach (var clientId in clientIdToMessageCooldown.Keys)
        {
            clientIdToMessageCooldown[clientId] -= Time.deltaTime;
        }
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        MessagePrefab = Resources.Load<GameObject>("ChatMessage");
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

    }

    public override void OnDestroy()
    {
        if (Instance != this) return;
        Instance = null;
    }


    [Rpc(SendTo.Server)]
    public void SendChatMessageRpc(string message, RpcParams rpcParams)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        var sender = GameManager.Instance.PlayerByClientId(rpcParams.Receive.SenderClientId);
        if (sender == null)
        {
            Debug.Log($"received chat by unknown client {rpcParams.Receive.SenderClientId}");
            return;
        }
        if (sender.User == null)
        {
            Debug.Log($"player ${sender.Email} has no user when sending chat message");
            PublishChatMessageRpc("<color=red>Failed to send message</color>", RpcTarget.Single(clientId, RpcTargetUse.Temp));
            return;
        }

        if (!CanClientChat(clientId))
        {
            PublishChatMessageRpc("<color=red>Please wait before sending another message</color>", RpcTarget.Single(clientId, RpcTargetUse.Temp));
            return;
        }

        message = message.Trim()[..maxMessageLength];

        PublishChatMessage(message, sender);
    }

    [Rpc(SendTo.Everyone, AllowTargetOverride = true, RequireOwnership = true, DeferLocal = true)]
    void PublishChatMessageRpc(string message, RpcParams rpcParams = default)
    {
        InstantiateMessage(message);
    }

    bool CanClientChat(ulong sender)
    {
        if (sender == 0)
        {
            return true;
        }

        float cooldown;

        if (!clientIdToMessageCooldown.TryGetValue(sender, out cooldown))
        {
            clientIdToMessageCooldown.Add(sender, messageCooldown);
            return true;
        }

        if (cooldown < 0)
        {
            clientIdToMessageCooldown[sender] = messageCooldown;
            return true;
        }

        return false;
    }

    void PublishChatMessage(string message, Player sender)
    {
        var playerColor = ColorUtility.ToHtmlStringRGB(sender.ChatColor());
        var messageFormatted = $"[<color={playerColor}>{sender.User.DisplayName}</color>]: <noparse>{message}</noparse>";

        PublishChatMessageRpc(messageFormatted);
    }

    void InstantiateMessage(string message)
    {
        var messageObject = Instantiate(MessagePrefab, transform);
        var texts = messageObject.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = message;
        texts[1].text = message;
    }
}
