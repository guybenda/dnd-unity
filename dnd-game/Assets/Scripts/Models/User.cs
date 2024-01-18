
using System;
using System.Threading.Tasks;
using DndFirebase;
using Firebase.Firestore;
using Unity.Netcode;
using UnityEngine;

public class User : INetworkSerializable
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

    public User() { }

    public static async Task<User> Get(string email)
    {
        var dbUser = await UserFirebase.Get(email);
        return new User(dbUser);
    }

    public async Task Save()
    {
        await ToDbUser(this).Save();
    }

    public User(UserFirebase dbUser)
    {
        DisplayName = dbUser.DisplayName;
        Email = dbUser.Email;
        Dice = UserDice.FromString(dbUser.Dice);
    }

    public UserFirebase ToDbUser(User user)
    {
        return new UserFirebase
        {
            DisplayName = user.DisplayName,
            Email = user.Email,
            Dice = user.Dice.ToString()
        };
    }
}

[FirestoreData]
public class UserFirebase
{
    public const string CollectionName = "users";

    public string DisplayName { get; set; }

    public string Email { get; set; }

    [FirestoreProperty]
    public string Dice { get; set; }

    static CollectionReference Col()
    {
        return DndFirestore.Instance.Db.Collection(CollectionName);
    }

    public static async Task<UserFirebase> Get(string email)
    {
        var doc = await Col().Document(email).GetSnapshotAsync();
        var user = doc.ConvertTo<UserFirebase>();
        user.Email = email;
        return user;
    }

    public async Task Save()
    {
        await Col().Document(Email).SetAsync(this);
    }
}