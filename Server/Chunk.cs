using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    public class Chunk
    {
        // Data
        public class Data
        {
            // Data
            public readonly Vector2DI position;
            Tile[,]   tiles;


            // Constructor
            public Data(Vector2DI inPos)
            {
                position = inPos;
            }


            // External
            public Tile GetTile(int x, int y)
            {
                return tiles[x, y];
            }

            public void SetTile(int x, int y, Tile inTile)
            {
                tiles[x, y] = inTile;
            }

            public void SetTiles(Tile[,] inTiles)
            {
                tiles = inTiles;
            }
        }
        Data _data;
        public Data data => _data;

        // References
        World _world;



        // Constructor
        public Chunk(Data inData, World inWorld)
        {
            _data     = inData;
            _world    = inWorld;

            Console.WriteLine("New Chunk: " + _data.position.x + "," + _data.position.y);
        }
    }
}
