using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lidgren.Network;

public struct Tile : IUnPackable  
{
    // Data
    ETerrainType _terrainType;
    public ETerrainType terrainType
    {
        get { return _terrainType; }
    }



    // Constructor
    public Tile(ETerrainType inTerrainType)
    {
        _terrainType = inTerrainType;
    }



    // Networking
    public void UnpackFrom(NetIncomingMessage inMsg)
    {
        _terrainType = (ETerrainType)inMsg.ReadVariableUInt32();
    }
}

// TODO: Byt ut mot en struct "TerrainType" som innehåller flags som "Walkable, Burning, Slow" etc.
public enum ETerrainType
{
    Grass,
    Sand,

    Length
}
