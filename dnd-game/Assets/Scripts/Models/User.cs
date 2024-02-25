
using System;
using System.Threading.Tasks;
using DndFirebase;
using Firebase.Firestore;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class User : INetworkSerializable
{
    public FixedString512Bytes DisplayName;
    public FixedString512Bytes Email;
    public UserDice Dice;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref DisplayName);
        // serializer.SerializeValue(ref Email);
        Dice.NetworkSerialize(serializer);
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

    User(UserFirebase dbUser)
    {
        DisplayName = dbUser.DisplayName;
        Email = dbUser.Email;
        Dice = new(dbUser.Dice);
    }

    UserFirebase ToDbUser(User user)
    {
        return new UserFirebase
        {
            DisplayName = user.DisplayName.ToString(),
            Email = user.Email.ToString(),
            Dice = user.Dice.ToString()
        };
    }

    public string ChatColor()
    {
        Color.RGBToHSV(Dice.MainColor, out var h, out var s, out var v);

        var amountToDesaturate = Mathf.Min(0.5f, 1 - v);
        s = Mathf.Max(s - amountToDesaturate, 0f);

        v = Mathf.Max(v, 0.7f);


        return ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(h, s, v));
    }

    public string ColoredDisplayName()
    {
        return $"<color=#{ChatColor()}>{DisplayName}</color>";
    }
}

[FirestoreData]
class UserFirebase
{
    const string CollectionName = "users";

    [FirestoreProperty]
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