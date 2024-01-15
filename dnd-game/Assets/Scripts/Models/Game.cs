using Unity.Netcode;

public class Game : INetworkSerializable
{

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }
}