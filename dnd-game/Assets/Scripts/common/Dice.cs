using System;

namespace DndCommon
{
    [Serializable]
    public class Dice
    {
        public int Sides { get; set; }
        public int Count { get; set; }
        public int Modifier { get; set; }

        public Dice(int sides, int count, int modifier)
        {
            Sides = sides;
            Count = count;
            Modifier = modifier;
        }

        public Dice(int sides, int count) : this(sides, count, 0)
        {
        }

        public Dice(int sides) : this(sides, 1, 0)
        {
        }

        public Dice() : this(20, 1, 0)
        {
        }

        public int Roll()
        {
            var random = new Random();
            var total = 0;
            for (var i = 0; i < Count; i++)
            {
                total += random.Next(1, Sides + 1);
            }

            return total + Modifier;
        }

        public override string ToString()
        {
            return $"{Count}d{Sides}{(Modifier > 0 ? "+" : "")}{(Modifier != 0 ? Modifier.ToString() : "")}";
        }
    }
}
