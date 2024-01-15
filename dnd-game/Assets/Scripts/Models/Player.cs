using Unity.Netcode;

public struct Player : INetworkSerializable
{
    public User User;
    public bool IsAllowedToRoll;


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref User);
        serializer.SerializeValue(ref IsAllowedToRoll);
    }
}