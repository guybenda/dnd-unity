using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    SyncExecutor exec = new();

    NetworkVariable<FixedString512Bytes> email = new("");
    public string Email
    {
        get => email.Value.ToString();
        set
        {
            email.Value = value;
        }
    }

    NetworkVariable<bool> isAllowedToRoll = new(false);
    public bool IsAllowedToRoll
    {
        get => isAllowedToRoll.Value;
        set
        {
            isAllowedToRoll.Value = value;
            isDirty = true;
        }
    }

    bool isDirty = true;

    public User User { get; private set; }

    void Update()
    {
        exec.Execute();

        if (isDirty)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdatePlayerUI(this);
                isDirty = false;
            }
        }
    }

    void Awake()
    {
    }

    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        base.OnSynchronize(ref serializer);
    }

    public override void OnNetworkSpawn()
    {
        LoadUser(Email);
        email.OnValueChanged += OnEmailChange;
        isAllowedToRoll.OnValueChanged += (_, _) => isDirty = true;
    }

    public override void OnNetworkDespawn()
    {
        UIManager.Instance.RemovePlayerUI(Email);
    }

    void OnEmailChange(FixedString512Bytes prevValue, FixedString512Bytes newValue)
    {
        LoadUser(newValue.ToString());
    }

    async void LoadUser(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return;
        }

        // There's a race here but the email should only be set once
        User = await User.Get(email);
        isDirty = true;
    }

    public void OnChangeCanRoll(bool canRoll)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning($"Player {Email} tried to change can roll but is server.");
            return;
        }

        IsAllowedToRoll = canRoll;
    }

    public void OnKick()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning($"Player {Email} tried to kick but is not server.");
            return;
        }

        NetworkManager.Singleton.DisconnectClient(GameManager.Instance.ClientIdByEmail(Email));

        var message = $"<color=#{User.ChatColor()}>{User.DisplayName}</color> has been kicked";
        ChatManager.Instance.PublishChatMessageRpc(message);
    }
}