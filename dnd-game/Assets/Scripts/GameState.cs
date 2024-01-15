using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameState : NetworkBehaviour
{
    public static GameState Instance { get; private set; }

    // players


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
            Debug.LogError("Multiple GameStates, destroying");
            Destroy(this);
            return;
        }

        Instance = this;


    }

    public override void OnDestroy()
    {
        Instance = null;
    }

    public override void OnNetworkSpawn()
    {

    }
}
