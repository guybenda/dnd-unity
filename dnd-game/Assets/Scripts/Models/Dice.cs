using System;

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


    public class Dice
    {
        public static readonly DiceType[] types = (DiceType[])Enum.GetValues(typeof(DiceType));
    }
}
