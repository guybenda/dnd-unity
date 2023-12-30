using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using UnityEngine;

public class DiceContainer : MonoBehaviour
{
    public Transform SpawnPoint;

    DiceManager diceManager;

    void Start()
    {

    }

    void Update()
    {

    }

    void Awake()
    {
        diceManager = GameObject.FindGameObjectWithTag("DiceManager").GetComponent<DiceManager>();
    }

    public void New(DiceType type)
    {
        var die = diceManager.MakeDie(type, parent: transform, position: SpawnPoint.position);
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

    public (int, string) Total()
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
            if (!dice[i].IsStatic())
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
        New(DiceType.D4);
    }

    public void D6()
    {
        New(DiceType.D6);
    }

    public void D8()
    {
        New(DiceType.D8);
    }

    public void D10()
    {
        New(DiceType.D10);
    }

    public void D12()
    {
        New(DiceType.D12);
    }

    public void D20()
    {
        New(DiceType.D20);
    }

    public void D100()
    {
        New(DiceType.D100);
    }
}
