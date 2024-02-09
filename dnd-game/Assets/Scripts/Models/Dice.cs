using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DndCommon
{
    public enum DiceType
    {
        D4 = 4,
        D6 = 6,
        D8 = 8,
        D10 = 10,
        D12 = 12,
        D20 = 20,
        D100 = 100
    }

    public enum DiceQuality
    {
        Low,
        Medium,
        High,
    }

    public class RollCommand
    {
        public DiceType Type;
        public uint Count;
        public DropKeepDice Keep;

        public uint Id;


        public class DropKeepDice
        {
            public bool Keep = true;
            public bool Highest = true;
            public uint Count = 1;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Count > 1) builder.Append(Count);
            builder.Append('d');
            builder.Append((int)Type);

            if (Keep == null) return builder.ToString();

            builder.Append(Keep.Keep ? 'k' : 'd');
            builder.Append(Keep.Highest ? 'h' : 'l');
            if (Keep.Count > 1) builder.Append(Keep.Count);

            return builder.ToString();
        }

        public static string ListToString(List<RollCommand> commands)
        {
            return string.Join(" + ", commands.Select(c => c.ToString()));
        }

        public static RollCommand FromString(string command)
        {
            if (!command.Contains('d')) return null;

            var rollParts = command.Split('d');

            if (rollParts.Length != 2) return null;

            uint count = 1;

            if (!string.IsNullOrEmpty(rollParts[0]) && !uint.TryParse(rollParts[0], out count)) return null;

            var dropKeepResult = ParseTypePart(rollParts[1]);
            if (!dropKeepResult.HasValue) return null;

            return new RollCommand
            {
                Type = dropKeepResult.Value.type,
                Count = count,
                Keep = dropKeepResult.Value.keep,
                Id = GetNextRollId()
            };
        }

        // This is bad bad bad bad
        static (DiceType type, DropKeepDice keep)? ParseTypePart(string command)
        {
            if (int.TryParse(command, out var typeNum))
            {
                if (!Enum.IsDefined(typeof(DiceType), typeNum)) return null;
                return ((DiceType)typeNum, null);
            }

            var keep = new DropKeepDice();

            var dropKeepParts = command.Split('k');
            if (dropKeepParts.Length > 2) return null;
            if (dropKeepParts.Length == 2)
            {
                keep.Keep = true;
            }
            else
            {
                dropKeepParts = command.Split('d');
                if (dropKeepParts.Length > 2) return null;
                if (dropKeepParts.Length == 2)
                {
                    keep.Keep = false;
                }
            }

            string countPart;

            if (dropKeepParts[1] == "l")
            {
                keep.Highest = false;
                countPart = dropKeepParts[1][1..];
            }
            else if (dropKeepParts[1] == "h")
            {
                keep.Highest = true;
                countPart = dropKeepParts[1][1..];
            }
            else
            {
                countPart = dropKeepParts[1];
            }

            if (!string.IsNullOrEmpty(countPart) && !uint.TryParse(countPart, out keep.Count))
                return null;


            if (!int.TryParse(command, out typeNum)) return null;
            if (!Enum.IsDefined(typeof(DiceType), typeNum)) return null;

            return ((DiceType)typeNum, keep);
        }

        static uint currentRollId = 1;
        static readonly object rollIdLock = new();

        static uint GetNextRollId()
        {
            lock (rollIdLock)
            {
                return currentRollId++;
            }
        }
    }

    public class Dice
    {
        public static readonly List<DiceType> types = ((DiceType[])Enum.GetValues(typeof(DiceType))).ToList();

        public static List<RollCommand> ParseRollCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;

            var result = new List<RollCommand>();

            var subCommands = command.Split('+');

            foreach (var subCommand in subCommands)
            {
                var roll = RollCommand.FromString(subCommand.Trim());

                if (roll == null) return null;

                result.Add(roll);
            }

            return result;
        }
    }
}
