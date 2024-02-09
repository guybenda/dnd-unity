using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatCommandManager : MonoBehaviour
{
    const float diceRollInterval = 0.2f;
    readonly Dictionary<string, Func<string, Player, (bool, string)>> commands = new();

    DiceContainer diceContainer;

    void Start()
    {

    }

    void Update()
    {

    }

    void Awake()
    {
        commands.Add("roll", RollCommand);
        commands.Add("clear", ClearCommand);

        diceContainer = GameObject.Find("DiceContainer").GetComponent<DiceContainer>();
    }

    public void ProcessCommand(string command, Player player, ulong clientId)
    {
        var split = command.Split(' ', 2);
        var commandName = split[0].ToLower()[1..];
        var commandArgs = split.Length > 1 ? split[1] : null;

        if (!commands.TryGetValue(commandName, out var commandFunc))
        {
            ChatManager.Instance.SendMessageToClient($"<color=red>Unknown command: {commandName}</color>", clientId);
            return;
        }

        var (success, errorMessage) = commandFunc(commandArgs, player);
        if (!success)
        {
            ChatManager.Instance.SendMessageToClient($"<color=red>{errorMessage}</color>", clientId);
        }
    }

    (bool, string) RollCommand(string command, Player player)
    {
        if (!player.IsAllowedToRoll)
        {
            return (false, "You are not allowed to roll");
        }

        var rollCommands = Dice.ParseRollCommand(command);
        if (rollCommands == null)
        {
            return (false, "Syntax error in roll command");
        }

        var formattedRolls = DndCommon.RollCommand.ListToString(rollCommands);

        ChatManager.Instance.PublishChatMessageRpc($"{player.User.ColoredDisplayName()} rolled: {formattedRolls}");

        StartCoroutine(RollAndWaitForResults(rollCommands, player));

        return (true, null);
    }

    (bool, string) ClearCommand(string command, Player player)
    {
        if (!player.IsAllowedToRoll)
        {
            return (false, "You are not allowed to roll");
        }

        diceContainer.Clear();

        return (true, null);
    }

    IEnumerator RollAndWaitForResults(List<RollCommand> rollCommands, Player player)
    {
        List<DiceScript> dice = new();
        foreach (var rollCommand in rollCommands)
        {
            for (int i = 0; i < rollCommand.Count; i++)
            {
                yield return new WaitForSeconds(diceRollInterval);
                var die = diceContainer.NewDice(rollCommand.Type, player, rollCommand.Id);
                dice.Add(die);
            }
        }

        bool allDiceStopped = false;

        while (!allDiceStopped)
        {
            yield return new WaitForSeconds(0.5f);

            if (dice.All(d => d.IsStatic))
            {
                allDiceStopped = true;
            }
        }

        // TODO keep and drop logic
        var total = dice.Sum(d => d.Result());

        ChatManager.Instance.PublishChatMessageRpc($"{player.User.ColoredDisplayName()}'s roll totaled: {total}");
    }
}
