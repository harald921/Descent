using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descent_Server
{
    public class Tile
    {
        // Data
        public readonly Vector2DI localPosition;
        public readonly Vector2DI chunkPosition;

        Terrain _terrain;
        
        // References
        Chunk _chunk; 


        // Constructor
        public Tile(Vector2DI inLocalPosition, Vector2DI inChunkPosition, Terrain inTerrain)
        {
            localPosition = inLocalPosition;
            _terrain = inTerrain;
            chunkPosition = inChunkPosition;
        }



        // External
        public Tile GetNearbyTile(Vector2DI inDirection)
        {
            if (_chunk == null) _chunk = World.GetChunk(chunkPosition);
            return _chunk.data.GetTile(localPosition.x + inDirection.x, localPosition.y + inDirection.y);
        }
    }
}
