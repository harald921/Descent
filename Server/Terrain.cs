using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    public struct Terrain
    {
        // Static data
        public class StaticData : IInPackable
        {
            // Data
            public readonly int textureID;
            public readonly float moveSpeed;


            // Constructor
            public StaticData(int inTextureID, float inMoveSpeed)
            {
                textureID = inTextureID;
                moveSpeed = inMoveSpeed;
            }



            // Networking
            public int GetPacketSize()
            {
                int bitsNeeded = 0;

                bitsNeeded += NetUtility.BitsToHoldUInt((uint)textureID);
                bitsNeeded += 32;

                return bitsNeeded;
            }

            public void PackInto(NetOutgoingMessage inMsg)
            {
                inMsg.WriteVariableUInt32((uint)textureID);
                inMsg.Write(moveSpeed);
            }
        }
        static DataManager _dataManager = new DataManager();
       
        // Data
        public enum Type
        {
            Grass,
            Sand,
            LENGTH
        }
        public readonly Type _type;

        public StaticData data => DataManager.staticTerrainData[_type];



        // Constructors
        static Terrain()
        {
            Network.OnPlayerJoin += (NetConnection inConnection) => Network.Send(_dataManager, EDataPacketTypes.TerrainData, inConnection, NetDeliveryMethod.ReliableUnordered);
        }

        public Terrain(Type inType)
        {
            _type = inType;
        }

        class DataManager : IInPackable
        {
            // Data
            public readonly static Dictionary<Type, StaticData> staticTerrainData = new Dictionary<Type, StaticData>()
            {
                { Type.Grass, new StaticData(inTextureID: 2, inMoveSpeed: 0.9f) },
                { Type.Sand,  new StaticData(inTextureID: 1, inMoveSpeed: 0.8f) }
            };


            // Networking
            public int GetPacketSize()
            {
                int bitsNeeded = 0;

                bitsNeeded += NetUtility.BitsToHoldUInt((uint)staticTerrainData.Count);
                foreach (KeyValuePair<Type, StaticData> entry in staticTerrainData)
                {
                    bitsNeeded += NetUtility.BitsToHoldUInt((uint)entry.Key);
                    bitsNeeded += entry.Value.GetPacketSize();
                }

                return bitsNeeded;
            }

            public void PackInto(NetOutgoingMessage inMsg)
            {
                inMsg.WriteVariableUInt32((uint)staticTerrainData.Count);
                foreach (KeyValuePair<Type, StaticData> entry in staticTerrainData)
                {
                    inMsg.WriteVariableUInt32((uint)entry.Key);
                    entry.Value.PackInto(inMsg);
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
            else return               Terrain.Type.Sand;
        }
    }
}








//public struct Terrain : IInPackable
//{
//    // Instance data
//    public readonly Type _type;
//
//    // Properties
//    public StaticData data => _staticTerrainData[_type];
//
//    // Constructor
//    public Terrain(Type inType)
//    {
//        _type = inType;
//    }
//
//
//
//
//    // Static data
//    public class StaticData : IInPackable
//    {
//        // Data
//        public readonly int   textureID;
//        public readonly float moveSpeed;
//
//
//        // Constructor
//        public StaticData(int inTextureID, float inMoveSpeed)
//        {
//            textureID = inTextureID;
//            moveSpeed = inMoveSpeed;
//        }
//
//
//
//        // Networking
//        public int GetPacketSize()
//        {
//            int bitsNeeded = 0;
//
//            bitsNeeded += NetUtility.BitsToHoldUInt((uint)textureID);
//            bitsNeeded += 32;
//
//            return bitsNeeded;
//        }
//
//        public void PackInto(NetOutgoingMessage inMsg)
//        {
//            inMsg.WriteVariableUInt32((uint)textureID);
//            inMsg.Write(moveSpeed);
//        }
//    }
//
//    static Dictionary<Type, StaticData> _staticTerrainData = new Dictionary<Type, StaticData>()
//    {
//            { Type.Grass, new StaticData(inTextureID: 1, inMoveSpeed: 0.9f) },
//            { Type.Sand,  new StaticData(inTextureID: 2, inMoveSpeed: 0.8f) }
//    };
//
//    static Terrain()
//    {
//        Network.OnPlayerJoin += (NetConnection inConnection) => Network.Send(this, EDataPacketTypes.TerrainData, )
//    }
//
//
//    // Terrain types
//    public enum Type
//    {
//        Grass,
//        Sand,
//        LENGTH
//    }
//
//
//    // Networking
//    public int GetPacketSize()
//    {
//        int bitsNeeded = 0;
//
//        bitsNeeded += NetUtility.BitsToHoldUInt((uint)_staticTerrainData.Count);
//        foreach (KeyValuePair<Type, StaticData> entry in _staticTerrainData)
//        {
//            bitsNeeded += NetUtility.BitsToHoldUInt((uint)entry.Key);
//            bitsNeeded += entry.Value.GetPacketSize();
//        }
//
//        return bitsNeeded;
//    }
//
//    public void PackInto(NetOutgoingMessage inMsg)
//    {
//        inMsg.WriteVariableUInt32((uint)_staticTerrainData.Count);
//        foreach (KeyValuePair<Type, StaticData> entry in _staticTerrainData)
//        {
//            inMsg.WriteVariableUInt32((uint)entry.Key);
//            entry.Value.PackInto(inMsg);
//        }
//    }
//}