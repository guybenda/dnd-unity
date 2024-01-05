using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using Unity.Netcode;
using UnityEngine;


public class DiceContainer : NetworkBehaviour
{
    NetworkVariable<int> id = new(-1);

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
        id.Value = Random.Range(0, int.MaxValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NewDiceServerRpc(DiceType type)
    {
        var die = diceManager.MakeDie(type, parent: this.transform, position: SpawnPoint.position, containerId: id.Value);
    }

    GameObject[] Children()
    {
        var dice = GameObject.FindGameObjectsWithTag("Dice");
        var children = new List<GameObject>(dice.Length);

        for (int i = 0; i < dice.Length; i++)
        {
            if (dice[i].GetComponent<DiceScript>().Container == id.Value)
            {
                children.Add(dice[i]);
            }
        }

        return children.ToArray();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClearServerRpc()
    {
        foreach (var child in Children())
        {
            child.gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    public void Clear()
    {
        ClearServerRpc();
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
            var die = dice[i].GetComponent<DiceScript>();
            if (!die.IsStatic)
            {
                continue;
            }

            sum += die.Result();
            counts[die.Type]++;
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
