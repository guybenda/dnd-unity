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

        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
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


        // if (approvedUsers.Contains(userEmail))
        // {
        //     response.Approved = true;
        //     response.
        //     return;
        // }
        // if (pendingUsers.Contains(userEmail))
        // {
        //     response.Approved = false;
        //     response.Reason = "Waiting for approval, please try again later";
        //     return;
        // }

        // pendingUsers.Add(userEmail);
        // //TODO approve
    }

    async void ConnectPlayer(ulong clientId, string email)
    {
        var user = await User.Get(email);

        var player = Instantiate(playerPrefab);

        var playerScript = player.GetComponent<Player>();
        playerScript.Email = email;

        var playerNetworkObject = player.GetComponent<NetworkObject>();
        playerNetworkObject.SpawnAsPlayerObject(clientId);

    }

    void OnClientDisconnect(ulong obj)
    {
        throw new NotImplementedException();
    }

}
