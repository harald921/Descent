using UnityEngine;

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class ChunkGenerator
{
    // References
    readonly World _world;
    readonly ChunkDataGenerator _chunkDataGenerator;
    readonly ChunkViewGenerator _chunkViewGenerator;



    // Constructor
    public ChunkGenerator(uint inChunkSize, Noise.Parameters[] inNoiseParameters, World inWorld)
    {
        _world = inWorld;
        _chunkDataGenerator = new ChunkDataGenerator(inChunkSize, inNoiseParameters, inWorld);
        _chunkViewGenerator = new ChunkViewGenerator(inChunkSize, inWorld);
    }



    // External
    public Chunk GenerateChunk(Vector2DI inChunkPos)
    {
        Chunk.Data newChunkData = _chunkDataGenerator.Generate(inChunkPos);
        Chunk.View newChunkView = _chunkViewGenerator.Generate(inChunkPos, newChunkData);

        Chunk newChunk = new Chunk(newChunkData, newChunkView, _world);

        return newChunk;
    }

    public void RegenerateChunkView(Chunk.Data inChunkData, Chunk.View inChunkView)
    {
        _chunkViewGenerator.RegenerateChunkUV2(inChunkData, inChunkView);
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

            NoiseGenerator.Output   chunkNoiseData   = _noiseGenerator.Generate(inChunkPos);
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
                        newOutput.tiles[x, y] = new Tile(new Vector2DI(x,y), inChunkPos, new Terrain(TerrainGenerator.GetTerrainType(inNoiseData.heightMap[x, y])));

                return newOutput;
            }
        }
    }

    class ChunkViewGenerator
    {
        // Data
        readonly uint _chunkSize;
        readonly Material _chunkMaterial;

        // References
        MeshGenerator _meshGenerator;


        // Constructor
        public ChunkViewGenerator(uint inChunkSize, World inWorld)
        {
            _chunkSize = inChunkSize;
            _meshGenerator = new MeshGenerator(inChunkSize, inWorld);

            _chunkMaterial = (Material)Resources.Load("Material_Terrain", typeof(Material));
        }


        // External
        public Chunk.View Generate(Vector2DI inPosition, Chunk.Data inChunkData)
        {
            GameObject   newChunkGO = new GameObject("Chunk");

            MeshFilter   meshFilter   = newChunkGO.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newChunkGO.AddComponent<MeshRenderer>();

            meshRenderer.material = _chunkMaterial;

            newChunkGO.transform.position = new Vector3(inPosition.x, 0, inPosition.y);

            MeshGenerator.Output meshData = _meshGenerator.Generate(inChunkData);
            ApplyMesh(meshFilter, meshData); 

            Chunk.View newChunkView = new Chunk.View(newChunkGO);
            return newChunkView;
        }

        public void RegenerateChunkUV2(Chunk.Data inChunkData, Chunk.View inChunkView)
        {
            inChunkView.mesh.uv2 = _meshGenerator.GenerateUV2(inChunkData);
        }


        // Internal
        void ApplyMesh(MeshFilter inFilter, MeshGenerator.Output inMeshData)
        {
            inFilter.mesh.vertices  = inMeshData.vertices;
            inFilter.mesh.triangles = inMeshData.triangles;
            inFilter.mesh.uv2       = inMeshData.uv2;

            inFilter.mesh.RecalculateNormals();
        }

        class MeshGenerator
        {
            // Data
            readonly int  _chunkSize;
            readonly int  _vertexSize;
            readonly int  _vertexCount;

            readonly int[]     _triangles;
            readonly Vector3[] _vertices;


            // References
            World _world;


            // Output data
            public class Output
            {
                // Cached data
                readonly public Vector3[] vertices;
                readonly public int[]     triangles;

                // Data
                public Vector2[] uv2;

                // Constructor
                public Output(Vector3[] inVertices, int[] inTriangles)
                {
                    vertices = inVertices;
                    triangles = inTriangles;
                }
            }


            // Constructor
            public MeshGenerator(uint inChunkSize, World inWorld)
            {
                _world = inWorld;

                // Calculate sizes and counts
                _chunkSize   = (int)inChunkSize;
                _vertexSize  = _chunkSize * 2;
                _vertexCount = _vertexSize * _vertexSize * 4;


                // Generate vertices
                _vertices = new Vector3[_vertexCount];
                int vertexID = 0;
                for (int y = 0; y < _chunkSize; y++)
                {
                    for (int x = 0; x < _chunkSize; x++)
                    {
                        // Generate a quad 
                        _vertices[vertexID               + 0].x = x;
                        _vertices[vertexID               + 0].z = y;
                                                         
                        _vertices[vertexID               + 1].x = x + 1;
                        _vertices[vertexID               + 1].z = y;

                        _vertices[vertexID + _vertexSize + 0].x = x;
                        _vertices[vertexID + _vertexSize + 0].z = y + 1;

                        _vertices[vertexID + _vertexSize + 1].x = x + 1;
                        _vertices[vertexID + _vertexSize + 1].z = y + 1;

                        vertexID += 2;
                    }
                    vertexID += _vertexSize;
                }


                // Generate triangle ID's
                _triangles = new int[inChunkSize * inChunkSize * 6];
                int currentQuad = 0;
                for (int y = 0; y < _vertexSize; y += 2)
                    for (int x = 0; x < _vertexSize; x += 2)
                    {
                        int triangleOffset = currentQuad * 6;
                        int currentVertex = y * _vertexSize + x;

                        _triangles[triangleOffset + 0] = currentVertex + 0;                 // Bottom - Left
                        _triangles[triangleOffset + 1] = currentVertex + _vertexSize + 1;   // Top    - Right
                        _triangles[triangleOffset + 2] = currentVertex + 1;                 // Bottom - Right

                        _triangles[triangleOffset + 3] = currentVertex + 0;                 // Bottom - Left
                        _triangles[triangleOffset + 4] = currentVertex + _vertexSize + 0;   // Top    - Left
                        _triangles[triangleOffset + 5] = currentVertex + _vertexSize + 1;   // Top    - Right

                        currentQuad++;
                    }
            }


            // External
            public Output Generate(Chunk.Data inChunkData)
            {
                Output newOutput = new Output(_vertices, _triangles);

                newOutput.uv2 = GenerateUV2(inChunkData);

                return newOutput;
            }

            public Vector2[] GenerateUV2(Chunk.Data inChunkData)
            {
                Vector2[] newUV2 = new Vector2[_vertexCount];

                int vertexID = 0;
                for (int y = 0; y < _chunkSize; y++)
                {
                    for (int x = 0; x < _chunkSize; x++)
                    {

                        int tileTextureID = inChunkData.GetTile(x, y).terrain.data.textureID;

                        newUV2[vertexID               + 0] = new Vector2(tileTextureID, tileTextureID);
                        newUV2[vertexID               + 1] = new Vector2(tileTextureID, tileTextureID);
                        newUV2[vertexID + _vertexSize + 0] = new Vector2(tileTextureID, tileTextureID);
                        newUV2[vertexID + _vertexSize + 1] = new Vector2(tileTextureID, tileTextureID);

                        vertexID += 2;
                    }
                    vertexID += _vertexSize;
                }

                return newUV2;
            }
        }
    }
}
