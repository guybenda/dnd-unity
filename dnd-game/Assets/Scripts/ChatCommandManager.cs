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
    const float diceRollInterval = 0.1f;
    const int maxDice = 100;
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
        commands.Add("help", HelpCommand);

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

        var (success, message) = commandFunc(commandArgs, player);
        if (!success)
        {
            ChatManager.Instance.SendMessageToClient($"<color=red>{message}</color>", clientId);
        }
        else if (message != null)
        {
            ChatManager.Instance.SendMessageToClient($"{message}", clientId);
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

        var totalDice = rollCommands.Sum(r => r.Count);
        if (totalDice > maxDice)
        {
            return (false, $"You may not roll more than {maxDice} dice at once");
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
        Dictionary<uint, List<int>> results = new();
        foreach (var rollCommand in rollCommands)
        {
            for (int i = 0; i < rollCommand.Count; i++)
            {
                yield return new WaitForSeconds(diceRollInterval);
                if (!player) yield break;

                var die = diceContainer.NewDice(rollCommand.Type, player, rollCommand.Id);
                dice.Add(die);
            }
        }

        bool allDiceStopped = false;

        while (!allDiceStopped)
        {
            yield return new WaitForSeconds(0.5f);
            if (!player) yield break;

            if (dice.All(d => d.IsStatic))
            {
                allDiceStopped = true;
            }
        }

        foreach (var die in dice)
        {
            if (!results.ContainsKey(die.RollId))
            {
                results[die.RollId] = new List<int>();
            }

            results[die.RollId].Add(die.Result());
        }

        int total = 0;

        foreach (var rollCommand in rollCommands)
        {
            var orderedResults = results[rollCommand.Id].OrderBy(r => r);

            if (rollCommand.Keep == null)
            {
                total += orderedResults.Sum();
                continue;
            }

            var keepCount = rollCommand.Keep.Keep ? (int)rollCommand.Keep.Count : results[rollCommand.Id].Count - (int)rollCommand.Keep.Count;

            var chosenResults = rollCommand.Keep.Highest ? orderedResults.TakeLast(keepCount) : orderedResults.Take(keepCount);

            total += chosenResults.Sum();
        }

        ChatManager.Instance.PublishChatMessageRpc($"{player.User.ColoredDisplayName()}'s roll totaled: {total}");
    }


    (bool, string) HelpCommand(string command, Player player)
    {
        return (true, $"Available commands: {string.Join(", ", commands.Keys)}");
    }
}
