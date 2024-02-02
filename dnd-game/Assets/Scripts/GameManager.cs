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
    public static GameManager Instance { get; private set; }

    GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

    }

    public override void OnDestroy()
    {
        Instance = null;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        playerPrefab = Resources.Load<GameObject>("PlayerPrefab");

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        // Players.Value.Add(new(AuthManager.Instance.CurrentUser)
        // {
        //     IsAllowedToRoll = true
        // });
    }

    // I gave up on approval for now, this only spawns the player
    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false;

        var userEmail = Encoding.ASCII.GetString(request.Payload);

        ConnectPlayer(request.ClientNetworkId, userEmail);

    }

    public async void ConnectPlayer(ulong clientId, string email, bool isAllowedToRoll = false)
    {
        var user = await User.Get(email);

        if (user == null)
        {
            Debug.LogError($"User {email} not found");
            NetworkManager.Singleton.DisconnectClient(clientId);
            return;
        }

        var player = Instantiate(playerPrefab);

        var playerScript = player.GetComponent<Player>();
        playerScript.Email = email;

        var playerNetworkObject = player.GetComponent<NetworkObject>();
        playerNetworkObject.SpawnAsPlayerObject(clientId);

        playerScript.IsAllowedToRoll = isAllowedToRoll;

    }

    void OnClientDisconnect(ulong obj)
    {
        throw new NotImplementedException();
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

}
