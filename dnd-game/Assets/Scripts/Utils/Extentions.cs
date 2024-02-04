
using System;
using Unity.Netcode;

public static class ServerRpcParamsExtentions
{
    public static Player Player(this ServerRpcParams serverRpcParams)
    {
        return GameManager.Instance.PlayerByClientId(serverRpcParams.Receive.SenderClientId);
    }

    public static Player MustPlayer(this ServerRpcParams serverRpcParams)
    {
        var player = serverRpcParams.Player();
        if (player == null)
        {
            throw new Exception($"Player with clientId {serverRpcParams.Receive.SenderClientId} not found");
        }

        return player;
    }
}