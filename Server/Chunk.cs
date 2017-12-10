using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    class Chunk : IPackable
    {
        Vector2DI _position;
        public Vector2DI position { get { return _position; } }
        Tile[,] _tiles;
        

        // Constructor
        public Chunk(NetIncomingMessage inMsg) { UnpackFrom(inMsg); }
        public Chunk(Vector2DI inPosition, int inSize)
        {
            _position = inPosition;
            
            _tiles = new Tile[inSize, inSize];

            Random rng = new Random();

            for (int x = 0; x < inSize; x++)
                for (int y = 0; y < inSize; y++)
                    _tiles[x, y] = new Tile((ETerrainType)rng.Next(0, 2));
        }



        public int GetPacketSize()
        {
            int bitsNeeded = NetUtility.BitsToHoldUInt((uint)_tiles.GetLength(0));

            int tilesLength = _tiles.GetLength(0);
            for (int y = 0; y < tilesLength; y++)
                for (int x = 0; x < tilesLength; x++)
                    bitsNeeded += _tiles[x, y].GetPacketSize();

            return bitsNeeded;
        }

        public void PackInto(NetOutgoingMessage inMsg)
        {
            inMsg.WriteVariableUInt32((uint)_tiles.GetLength(0));             // Size 

            int tilesLength = _tiles.GetLength(0);
            for (int y = 0; y < tilesLength; y++)
                for (int x = 0; x < tilesLength; x++)
                    _tiles[x, y].PackInto(inMsg);
        }

        public void UnpackFrom(NetIncomingMessage inMsg)
        {
            throw new NotImplementedException();
        }
    }
}
