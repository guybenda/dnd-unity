using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DndCommon;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public class DiceContainer : NetworkBehaviour
{
    readonly Vector3 startingVelocity = new(0, 5, 0);

    // NetworkVariable<int> id = new(-1);
    NetworkVariable<int> total = new(0);
    NetworkVariable<FixedString512Bytes> breakdown = new("");

    int diceCalcCounter = 0;

    public int DiceTotal
    {
        get => total.Value;
        private set
        {
            total.Value = value;
        }
    }

    public string DiceBreakdown
    {
        get => breakdown.Value.ToString();
        private set
        {
            if (breakdown.Value.Equals(value)) return;
            breakdown.Value = value;
        }
    }

    public Transform Target;

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        diceCalcCounter = (diceCalcCounter + 1) % 4;
        if (diceCalcCounter != 0) return;

        (DiceTotal, DiceBreakdown) = Total();

    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // id.Value = Random.Range(0, int.MaxValue);
    }

    Vector3 GetRandomOrigin()
    {
        return Target.transform.position + new Vector3(Random.Range(-8f, 8f), 5, Random.Range(-8f, 8f));
    }

    [ServerRpc(RequireOwnership = false)]
    public void NewDiceServerRpc(DiceType type, ServerRpcParams serverRpcParams = default)
    {
        var sender = serverRpcParams.MustPlayer();
        if (!sender.IsAllowedToRoll)
        {
            Debug.Log($"User {sender.Email} tried to roll but is not allowed to");
            return;
        }

        NewDice(type, sender);
    }

    public DiceScript NewDice(DiceType type, Player sender, uint rollId = 0)
    {
        var origin = GetRandomOrigin();
        var velocity = startingVelocity + (Target.position - origin) * 1.5f;

        var die = DiceManager.Instance.MakeDie(type, sender.User, position: origin, velocity: velocity, rollId: rollId);
        return die;
    }

    GameObject[] GetCurrentDice(uint? rollId = null)
    {
        var dice = GameObject.FindGameObjectsWithTag("Dice");
        var children = new List<GameObject>(dice.Length);

        for (int i = 0; i < dice.Length; i++)
        {
            if (!rollId.HasValue || dice[i].GetComponent<DiceScript>().RollId == rollId.Value)
            {
                children.Add(dice[i]);
            }
        }

        return children.ToArray();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClearServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var sender = serverRpcParams.MustPlayer();
        if (!sender.IsAllowedToRoll)
        {
            Debug.Log($"User {sender.Email} tried to clear but is not allowed to");
            return;
        }

        foreach (var child in GetCurrentDice())
        {
            child.gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    public void Clear()
    {
        ClearServerRpc();
    }


    readonly Dictionary<DiceType, int> counts = new(){
            { DiceType.D4, 0 },
            { DiceType.D6, 0 },
            { DiceType.D8, 0 },
            { DiceType.D10, 0 },
            { DiceType.D12, 0 },
            { DiceType.D20, 0 },
            { DiceType.D100, 0 },
        };

    public (int total, string breakdown) Total()
    {
        var sum = 0;
        var dice = GetCurrentDice();
        Dice.types.ForEach(d => counts[d] = 0);

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

        var builder = new StringBuilder();

        var first = true;
        foreach (var (key, value) in counts)
        {
            if (value > 0)
            {
                builder.Append($"{value}d{(int)key}");
                if (!first)
                {
                    builder.Append(" + ");
                }
                else
                {
                    first = false;
                }
            }
        }

        return (sum, builder.ToString());
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
