using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descent_Server
{
    public class ChunkGenerator
    {
        // References
        readonly World _world;
        readonly ChunkDataGenerator _chunkDataGenerator;



        // Constructor
        public ChunkGenerator(uint inChunkSize, Noise.Parameters[] inNoiseParameters, World inWorld)
        {
            _world = inWorld;
            _chunkDataGenerator = new ChunkDataGenerator(inChunkSize, inNoiseParameters, inWorld);
        }



        // External
        public Chunk GenerateChunk(Vector2DI inChunkPos)
        {
            Chunk.Data newChunkData = _chunkDataGenerator.Generate(inChunkPos);

            Chunk newChunk = new Chunk(newChunkData, _world);

            return newChunk;
        }

        class ChunkDataGenerator
        {
            // References
            NoiseGenerator _noiseGenerator;
            TileMapGenerator _tileMapGenerator;


            // Constructor
            public ChunkDataGenerator(uint inChunkSize, Noise.Parameters[] inNoiseParameters, World inTerrain)
            {
                _noiseGenerator = new NoiseGenerator(inChunkSize, inNoiseParameters);
                _tileMapGenerator = new TileMapGenerator(inChunkSize, inTerrain);
            }


            // Exposed
            public Chunk.Data Generate(Vector2DI inChunkPos)
            {
                Chunk.Data newChunkData = new Chunk.Data(inChunkPos);

                NoiseGenerator.Output chunkNoiseData = _noiseGenerator.Generate(inChunkPos);
                TileMapGenerator.Output chunkTileMapData = _tileMapGenerator.Generate(inChunkPos, chunkNoiseData);

                newChunkData.SetTiles(chunkTileMapData.tiles);

                return newChunkData;
            }



            class NoiseGenerator
            {
                // Data
                readonly uint _chunkSize;
                readonly Noise.Parameters[] _noiseParameters;


                // Return data
                public class Output
                {
                    public float[,] heightMap;
                }


                // Constructor
                public NoiseGenerator(uint inChunkSize, Noise.Parameters[] inNoiseParameters)
                {
                    _chunkSize = inChunkSize;
                    _noiseParameters = inNoiseParameters;
                }


                // External
                public Output Generate(Vector2DI inChunkPos)
                {
                    return new Output { heightMap = Noise.Generate(_chunkSize, _noiseParameters[0], inChunkPos) };
                }
            }



            class TileMapGenerator
            {
                // Data
                readonly uint _chunkSize;

                // References
                World _world;

                // Output data
                public class Output
                {
                    public Tile[,] tiles;

                    public Output(uint inChunkSize)
                    {
                        tiles = new Tile[inChunkSize, inChunkSize];
                    }
                }


                // Constructor
                public TileMapGenerator(uint inChunkSize, World inWorld)
                {
                    _chunkSize = inChunkSize;
                    _world = inWorld;
                }


                // External
                public Output Generate(Vector2DI inChunkPos, NoiseGenerator.Output inNoiseData)
                {
                    Output newOutput = new Output(_chunkSize);

                    for (int y = 0; y < _chunkSize; y++)
                        for (int x = 0; x < _chunkSize; x++)
                            newOutput.tiles[x, y] = new Tile(new Vector2DI(x, y), inChunkPos, new Terrain(TerrainGenerator.GetTerrainType(inNoiseData.heightMap[x, y])));

                    return newOutput;
                }
            }
        }
    }
}
