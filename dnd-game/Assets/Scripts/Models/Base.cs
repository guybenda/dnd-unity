
using Firebase.Firestore;

// TODO use this
public class BaseFirebaseEntity
{
    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public Timestamp UpdatedAt { get; set; }
}