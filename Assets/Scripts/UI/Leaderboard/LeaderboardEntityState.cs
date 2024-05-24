using System;
using Unity.Collections;
using Unity.Netcode;

public struct leaderboardEntityState : INetworkSerializable, IEquatable<leaderboardEntityState>
{
    public ulong ClientID;
    public FixedString32Bytes PlayerName;
    public int Deaths;
    public int Kills;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Deaths);
        serializer.SerializeValue(ref Kills);
    }

    public bool Equals(leaderboardEntityState other)
    {
        return ClientID == other.ClientID &&
            PlayerName.Equals(other.PlayerName) &&
            Deaths == other.Deaths &&
            Kills == other.Kills;
    }
}
