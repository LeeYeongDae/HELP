using Unity.Netcode;
using System.Collections.Generic;

namespace LobbyRelay
{
    /// <summary>
    /// An example of a custom type serialized for use in RPC calls. This represents the state of a player as far as NGO is concerned,
    /// with relevant fields copied in or modified directly.
    /// </summary>
    public class PlayerData : INetworkSerializable
    {
        public string name;
        public ulong id;
        public List<int> items;
        public PlayerData() { } // A default constructor is explicitly required for serialization.
        public PlayerData(string name, ulong id, List<int> items = null) { this.name = name; this.id = id; this.items = items; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref name);
            serializer.SerializeValue(ref id);
            int length = 0;
            int[] Array;
            if (serializer.IsWriter)
            {
                Array = items.ToArray();
                length = Array.Length;
            }
            else
            {
                Array = new int[length];
            }
            serializer.SerializeValue(ref length);
            serializer.SerializeValue(ref Array);

            if (serializer.IsReader)
            {
                for (int i = 0; i < length; i++)
                    items.Add(Array[i]);
            }
        }
    }
}
