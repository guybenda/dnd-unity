using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatManager : NetworkBehaviour
{
    SyncExecutor exec = new();

    public static ChatManager Instance { get; private set; }

    public GameObject MessagesContainer;
    public TMP_InputField ChatInput;

    public bool IsTyping { get; private set; }

    const int maxMessageLength = 140;
    const float messageCooldown = 0.5f;
    const int maxMessages = 200;
    Dictionary<ulong, float> clientIdToMessageCooldown = new();

    NonDraggableScrollRect scrollRect;
    ChatCommandManager chatCommandManager;

    GameObject messagePrefab;


    void Start()
    {
        ChatInput.gameObject.SetActive(false);
    }

    void Update()
    {
        exec.Execute();

        foreach (var clientId in clientIdToMessageCooldown.Keys.ToList())
        {
            clientIdToMessageCooldown[clientId] -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            IsTyping = !IsTyping;

            ChatInput.gameObject.SetActive(IsTyping);
            scrollRect.verticalNormalizedPosition = 0f;
            scrollRect.vertical = IsTyping;

            if (IsTyping)
            {
                ChatInput.ActivateInputField();
            }
            else
            {

                var message = ChatInput.text;
                if (message.Length > 0)
                {
                    SendChatMessageRpc(message);
                }
            }

            ChatInput.text = string.Empty;
        }
        else if (!IsTyping && Input.GetKeyDown(KeyCode.Slash))
        {
            IsTyping = !IsTyping;

            ChatInput.gameObject.SetActive(IsTyping);
            scrollRect.verticalNormalizedPosition = 0f;
            scrollRect.vertical = IsTyping;
            ChatInput.ActivateInputField();
            ChatInput.text = "/";
            ChatInput.caretPosition = 1;
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

        messagePrefab = Resources.Load<GameObject>("ChatMessagePrefab");

        scrollRect = GetComponent<NonDraggableScrollRect>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        chatCommandManager = gameObject.AddComponent<ChatCommandManager>();
    }

    public override void OnDestroy()
    {
        if (Instance != this) return;
        Instance = null;
    }


    [Rpc(SendTo.Server)]
    public void SendChatMessageRpc(string message, RpcParams rpcParams = default)
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

        if (message[0] == '/')
        {
            chatCommandManager.ProcessCommand(message, sender, clientId);
            return;
        }

        if (message.Length > maxMessageLength)
        {
            message = message.Trim()[..maxMessageLength];
        }

        PublishChatMessage(message, sender);
    }

    [Rpc(SendTo.Everyone, AllowTargetOverride = true, RequireOwnership = true, DeferLocal = true)]
    public void PublishChatMessageRpc(string message, RpcParams rpcParams = default)
    {
        InstantiateMessage(message);

        // TODO pool
        if (MessagesContainer.transform.childCount > maxMessages)
        {
            Destroy(MessagesContainer.transform.GetChild(0).gameObject);
        }
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
        var messageFormatted = $"[{sender.User.ColoredDisplayName()}]: <noparse>{message}</noparse>";

        PublishChatMessageRpc(messageFormatted);
    }

    public void SendMessageToClient(string message, ulong clientId)
    {
        PublishChatMessageRpc(message, RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    void InstantiateMessage(string message)
    {
        var messageObject = Instantiate(messagePrefab, MessagesContainer.transform);
        var texts = messageObject.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = message;
        texts[1].text = message;
    }
}
