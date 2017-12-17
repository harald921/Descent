using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;


// Types
public enum EDataPacketTypes
{
    WorldData,
    ChunkData,
    TerrainData,
}
// Interface
public interface IPackable : IInPackable, IUnPackable { }

public interface IInPackable
{
    int GetPacketSize();
    void PackInto(NetOutgoingMessage inMsg);
}

public interface IUnPackable
{
    void UnpackFrom(NetIncomingMessage inMsg);
}
