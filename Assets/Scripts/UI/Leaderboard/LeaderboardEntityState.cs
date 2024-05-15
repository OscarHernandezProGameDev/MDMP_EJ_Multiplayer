using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Deaths;
    public int Kills;

    public bool Equals(LeaderboardEntityState other)
        => ClientId == other.ClientId
        && PlayerName.Equals(other.PlayerName)
        && Deaths == other.Deaths
        && Kills == other.Kills;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Deaths);
        serializer.SerializeValue(ref Kills);
    }
}
