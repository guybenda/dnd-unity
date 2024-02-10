using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DndFirebase;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    SyncExecutor exec = new();

    public static GameManager Instance { get; private set; }

    // This can be moved into Player
    Dictionary<ulong, string> clientIdToEmail = new();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        exec.Execute();
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (NetworkManager.Singleton.IsClient) return;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    public override void OnDestroy()
    {
        if (Instance != this) return;
        Instance = null;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
    }

    // I gave up on approval for now, this only spawns the player
    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // response.Pending = true;
        response.Approved = true;
        response.CreatePlayerObject = true;

        var userEmail = Encoding.ASCII.GetString(request.Payload);
        Debug.Log($"User {userEmail} begin connection");

        clientIdToEmail.Add(request.ClientNetworkId, userEmail);

        // ConnectPlayer(request.ClientNetworkId, userEmail, response);
    }

    async void OnClientConnected(ulong clientId)
    {
        var email = clientIdToEmail.GetValueOrDefault(clientId);
        if (email == null)
        {
            Debug.LogError($"Client {clientId} connected but no email found");
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }

        var user = await User.Get(email);
        if (user == null)
        {
            Debug.LogError($"User {email} connected but not found in database");
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }

        var player = PlayerByClientId(clientId);
        if (player == null)
        {
            Debug.LogError($"Player {email} has no player object when executing OnClientConnected");
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }

        var playerScript = player.GetComponent<Player>();
        playerScript.Email = email;
        playerScript.IsAllowedToRoll = clientId == NetworkManager.Singleton.LocalClientId;

        Debug.Log($"User {email} client {clientId} finished connection");

        var message = $"{user.ColoredDisplayName()} connected";
        ChatManager.Instance.PublishChatMessageRpc(message);
    }

    void OnClientDisconnect(ulong clientId)
    {
        var player = PlayerByClientId(clientId);
        if (player == null)
        {
            return;
        }

        clientIdToEmail.Remove(clientId);

        Debug.Log($"User {player.Email} client {clientId} disconnected");

        var message = $"{player.User.ColoredDisplayName()} disconnected";
        ChatManager.Instance.PublishChatMessageRpc(message);
    }

    public Player CurrentPlayer()
    {
        return NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
    }

    public List<Player> Players()
    {
        var playerObjects = NetworkManager.Singleton.ConnectedClientsList;
        var playersList = new List<Player>(playerObjects.Count);

        for (int i = 0; i < playerObjects.Count; i++)
        {
            playersList.Add(playerObjects[i].PlayerObject.GetComponent<Player>());
        }

        return playersList;
    }

    public Player PlayerByEmail(string email)
    {
        var players = Players();

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Email == email)
            {
                return players[i];
            }
        }

        return null;
    }

    public Player PlayerByClientId(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            var playerObject = networkClient.PlayerObject;
            if (playerObject == null)
            {
                Debug.LogError($"Player {clientId} found in ConnectedClients but has no PlayerObject");
                return null;
            }

            return networkClient.PlayerObject.GetComponent<Player>();
        }

        return null;
    }

    public ulong ClientIdByEmail(string email)
    {
        foreach (var (clientId, e) in clientIdToEmail)
        {
            if (e == email)
            {
                return clientId;
            }
        }

        throw new Exception($"No client found for email {email}");
    }

    public void PermitRolling(string email, bool isAllowedToRoll = true)
    {
        var player = PlayerByEmail(email);
        player.IsAllowedToRoll = isAllowedToRoll;
    }

    // public void RegisterUICallbacks(Action)
}
