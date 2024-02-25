using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndFirebase;
using Firebase.Firestore;
using Unity.Collections;
using Unity.Netcode;

public class Game
{
    public string Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<string> Maps { get; set; }

    public Game()
    {
        Id = Guid.NewGuid().ToString();
    }

    Game(GameFirebase dbGame)
    {
        Id = dbGame.Id;
        Name = dbGame.Name;
        CreatedAt = dbGame.CreatedAt.ToDateTime();
        UpdatedAt = dbGame.UpdatedAt.ToDateTime();
        Maps = dbGame.Maps;
    }

    public static async Task<Game> Get(string id)
    {
        var dbGame = await GameFirebase.Get(id);
        return new Game(dbGame);
    }

    public static async Task<List<Game>> GetUserGames()
    {
        var dbGames = await GameFirebase.GetUserGames();
        return dbGames.Select(dbGame => new Game(dbGame)).ToList();
    }

    public async Task Save()
    {
        var dbGame = new GameFirebase
        {
            Id = Id,
            Name = Name,
            CreatedAt = Timestamp.FromDateTime(CreatedAt),
            UpdatedAt = Timestamp.GetCurrentTimestamp(),
            Maps = Maps
        };

        await dbGame.Save();
    }
}

[FirestoreData]
class GameFirebase
{
    const string CollectionName = "games";

    public string Id { get; set; }

    [FirestoreProperty]
    public string OwnerEmail { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public Timestamp UpdatedAt { get; set; }

    [FirestoreProperty]
    public List<string> Maps { get; set; }


    static CollectionReference Col()
    {
        return DndFirestore.Instance.Db.Collection(CollectionName);
    }

    public static async Task<List<GameFirebase>> GetUserGames()
    {
        var results = await Col()
            .WhereEqualTo("OwnerEmail", AuthManager.Instance.CurrentUser.Email.ToString())
            .OrderBy("UpdatedAt")
            .GetSnapshotAsync();

        var games = results.Documents.Select(doc =>
        {
            var game = doc.ConvertTo<GameFirebase>();
            game.Id = doc.Id;
            return game;
        }).ToList();

        return games;
    }

    public static async Task<GameFirebase> Get(string id)
    {
        var doc = await Col().Document(id).GetSnapshotAsync();
        var game = doc.ConvertTo<GameFirebase>();
        game.Id = doc.Id;
        return game;
    }

    public async Task Save()
    {
        OwnerEmail = AuthManager.Instance.CurrentUser.Email.ToString();
        await Col().Document(Id).SetAsync(this);
    }
}
