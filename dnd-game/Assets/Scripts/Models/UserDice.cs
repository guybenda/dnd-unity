using System;
using Firebase.Firestore;
using Unity.Netcode;
using UnityEngine;

public struct UserDice : INetworkSerializable, IComparable<UserDice>
{
    public Color MainColor;
    public Color SecondaryColor;
    public Color NumbersColor;
    public float Smoothness;
    public bool Metallic;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref MainColor);
        serializer.SerializeValue(ref SecondaryColor);
        serializer.SerializeValue(ref NumbersColor);
        serializer.SerializeValue(ref Smoothness);
        serializer.SerializeValue(ref Metallic);
    }

    public override string ToString()
    {
        var (c1, c2, c3) = (
            ColorUtility.ToHtmlStringRGB(MainColor),
            ColorUtility.ToHtmlStringRGBA(SecondaryColor),
            ColorUtility.ToHtmlStringRGB(NumbersColor)
        );

        var metallic = Metallic ? "1" : "0";

        return $"#{c1}_#{c2}_#{c3}_{Smoothness}_{metallic}";
    }

    public static UserDice Default()
    {
        return new UserDice
        {
            MainColor = Color.white,
            SecondaryColor = Color.white,
            NumbersColor = Color.black,
            Smoothness = 0.7f,
            Metallic = false,
        };
    }

    public int CompareTo(UserDice other)
    {
        return ToString().CompareTo(other.ToString());
    }

    public UserDice(string from)
    {
        if (string.IsNullOrEmpty(from))
        {
            this = Default();
            return;
        }

        var parts = from.Split('_');

        if (parts.Length != 5)
        {
            this = Default();
            return;
        }

        MainColor = ColorUtility.TryParseHtmlString(parts[0], out var mainColor) ? mainColor : Color.white;
        SecondaryColor = ColorUtility.TryParseHtmlString(parts[1], out var secondaryColor) ? secondaryColor : Color.white;
        NumbersColor = ColorUtility.TryParseHtmlString(parts[2], out var numbersColor) ? numbersColor : Color.black;
        Smoothness = float.TryParse(parts[3], out var smoothness) ? smoothness : 0.7f;
        Metallic = parts[4] == "1";
    }

    public UserDice(UserDice userDice)
    {
        MainColor = userDice.MainColor;
        SecondaryColor = userDice.SecondaryColor;
        NumbersColor = userDice.NumbersColor;
        Smoothness = userDice.Smoothness;
        Metallic = userDice.Metallic;
    }
}
