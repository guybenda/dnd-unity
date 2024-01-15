
using Firebase.Firestore;
using Unity.Netcode;
using UnityEngine;

public struct User : INetworkSerializable
{
    public string DisplayName;
    public string Email;
    public UserDice Dice;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref DisplayName);
        serializer.SerializeValue(ref Email);
        serializer.SerializeValue(ref Dice);
    }
}

[FirestoreData]
public class UserFirebase
{
    public const string CollectionName = "users";

    [FirestoreProperty]
    public string DisplayName { get; set; }

    [FirestoreProperty]
    public string Email { get; set; }

    [FirestoreProperty]
    public string Dice { get; set; }

    public UserFirebase()
    {
    }

    public UserFirebase(User user)
    {
        DisplayName = user.DisplayName;
        Email = user.Email;
        Dice = user.Dice.ToString();
    }

    public User ToUser()
    {
        return new User
        {
            DisplayName = DisplayName,
            Email = Email,
            Dice = UserDice.FromString(Dice)
        };
    }
}