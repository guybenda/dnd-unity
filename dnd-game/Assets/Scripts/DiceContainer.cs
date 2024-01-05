using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using Unity.Netcode;
using UnityEngine;


public class DiceContainer : NetworkBehaviour
{
    public Transform SpawnPoint;

    DiceManager diceManager;

    void Start()
    {

    }

    void Update()
    {

    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        diceManager = GameObject.FindGameObjectWithTag("DiceManager").GetComponent<DiceManager>();
    }

    [ServerRpc]
    public void NewDiceServerRpc(DiceType type)
    {
        var die = diceManager.MakeDie(type, parent: this.transform, position: SpawnPoint.position);
    }

    DiceScript[] Children()
    {
        return GetComponentsInChildren<DiceScript>();
    }

    public void Clear()
    {
        foreach (var child in Children())
        {
            Destroy(child.gameObject);
        }
    }

    public (int total, string breakdown) Total()
    {
        var sum = 0;
        var dice = Children();
        var counts = new Dictionary<DiceType, int>{
            { DiceType.D4, 0 },
            { DiceType.D6, 0 },
            { DiceType.D8, 0 },
            { DiceType.D10, 0 },
            { DiceType.D12, 0 },
            { DiceType.D20, 0 },
            { DiceType.D100, 0 },
        };

        for (var i = 0; i < dice.Length; i++)
        {
            if (!dice[i].IsStatic)
            {
                continue;
            }

            sum += dice[i].Result();
            counts[dice[i].Type]++;
        }

        var countStr = string.Join(" + ", counts.Where(a => a.Value > 0).Select(a => $"{a.Value}d{(int)a.Key}"));

        return (sum, countStr);
    }

    public void D4()
    {
        NewDiceServerRpc(DiceType.D4);
    }

    public void D6()
    {
        NewDiceServerRpc(DiceType.D6);
    }

    public void D8()
    {
        NewDiceServerRpc(DiceType.D8);
    }

    public void D10()
    {
        NewDiceServerRpc(DiceType.D10);
    }

    public void D12()
    {
        NewDiceServerRpc(DiceType.D12);
    }

    public void D20()
    {
        NewDiceServerRpc(DiceType.D20);
    }

    public void D100()
    {
        NewDiceServerRpc(DiceType.D100);
    }
}
