using System.Collections;
using System.Collections.Generic;
using DndCommon;
using UnityEngine;

public class DiceEmitter : MonoBehaviour
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
