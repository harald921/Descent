using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    class Tile : IPackable
    {
        ETerrainType _terrainType;
        public ETerrainType terrainType { get { return _terrainType; } }




        // Constructor
        public Tile(NetIncomingMessage inMsg) { UnpackFrom(inMsg); }
        public Tile(ETerrainType inTerrainType)
        {
            _terrainType = inTerrainType;
        }



        public int GetPacketSize()
        {
            return NetUtility.BitsToHoldUInt((uint)ETerrainType.Length);
        }

        public void PackInto(NetOutgoingMessage inMsg)
        {
            inMsg.WriteRangedInteger(0, (uint)ETerrainType.Length, ((uint)_terrainType));                 // Type
        }

        public void UnpackFrom(NetIncomingMessage inMsg)
        {
            throw new NotImplementedException();
        }
    }

    // Byt ut mot en struct "TerrainType" som innehåller flags som "Walkable, Burning, Slow" etc.
    public enum ETerrainType
    {
        Dirt,
        Stone,

        Length
    }
}
