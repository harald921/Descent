using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lidgren.Network;

public struct Terrain
{
    // Static data
    public class StaticData
    {
        // Data
        public readonly int textureID;
        public readonly float moveSpeed;


        // Constructor
        public StaticData(NetIncomingMessage inMsg)
        {
            textureID = (int)inMsg.ReadVariableUInt32();
            moveSpeed = inMsg.ReadSingle();
        }
    }
    public readonly static DataManager dataManager = new DataManager();

    // Data
    public enum Type
    {
        Grass,
        Sand,
        LENGTH
    }
    public readonly Type type;

    public StaticData data => DataManager.staticTerrainData[type];



    public Terrain(Type inType)
    {
        type = inType;
    }

    public class DataManager : IUnPackable
    {
        // Data
        public readonly static Dictionary<Type, StaticData> staticTerrainData = new Dictionary<Type, StaticData>();

        // Networking
        public void UnpackFrom(NetIncomingMessage inMsg)
        {
            uint dictEntries = inMsg.ReadVariableUInt32();
            for (int i = 0; i < dictEntries; i++)
            {
                Type type = (Type)inMsg.ReadVariableUInt32();
                StaticData staticData = new StaticData(inMsg);

                staticTerrainData.Add(type, staticData);
            }
        }
    }
}



public static class TerrainGenerator
{
    // External
    public static Terrain.Type GetTerrainType(float inHeight)
    {
        // TODO: Implement this properly
        if (inHeight == 0) return Terrain.Type.Grass;
        else return Terrain.Type.Sand;
    }
}
