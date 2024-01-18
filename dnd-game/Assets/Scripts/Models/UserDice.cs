using Firebase.Firestore;
using Unity.Netcode;
using UnityEngine;

public class UserDice : INetworkSerializable
{
    public Color MainColor;
    public Color SecondaryColor;
    public Color NumbersColor;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref MainColor);
        serializer.SerializeValue(ref SecondaryColor);
        serializer.SerializeValue(ref NumbersColor);
    }

    public override string ToString()
    {
        return $"{ColorUtility.ToHtmlStringRGB(MainColor)}_{ColorUtility.ToHtmlStringRGB(MainColor)}_{ColorUtility.ToHtmlStringRGB(MainColor)}";
    }

    public static UserDice FromString(string str)
    {
        var colors = str.Split('_');

        if (colors.Length != 3)
        {
            return Default();
        }

        return new UserDice
        {
            MainColor = ColorUtility.TryParseHtmlString(colors[0], out var mainColor) ? mainColor : Color.white,
            SecondaryColor = ColorUtility.TryParseHtmlString(colors[1], out var secondaryColor) ? secondaryColor : Color.white,
            NumbersColor = ColorUtility.TryParseHtmlString(colors[2], out var numbersColor) ? numbersColor : Color.black,
        };
    }

    public static UserDice Default()
    {
        return new UserDice
        {
            MainColor = Color.white,
            SecondaryColor = Color.white,
            NumbersColor = Color.black,
        };
    }
}
