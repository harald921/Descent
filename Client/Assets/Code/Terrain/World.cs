using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lidgren.Network;

public class World : MonoBehaviour, IUnPackable
{
    class Data
    {
        public Noise.Parameters[] parameters; 
    }
    Data _data = new Data();

    // References
    Dictionary<Vector2DI, Chunk> _worldChunks = new Dictionary<Vector2DI, Chunk>();

    public ChunkGenerator     chunkGenerator { get; private set; }
    public TerrainTypeManager typeManager    { get; private set; }
    public TerrainDataManager dataManager    { get; private set; }



    void Update() // TODO: Do this manually
    {
        foreach (KeyValuePair<Vector2DI, Chunk> entry in _worldChunks)
            entry.Value.RawUpdate();
    }


    // Networking
    public void UnpackFrom(NetIncomingMessage inMsg)
    {
        // Get the world data from the server
        uint inChunkSize   = inMsg.ReadVariableUInt32(); // Chunk size

        uint numParameters = inMsg.ReadVariableUInt32(); // Number of parameters
        _data.parameters = new Noise.Parameters[numParameters];

        for (int i = 0; i < numParameters; i++)          // Parameters
            _data.parameters[i].UnpackFrom(inMsg);       


        // Create the chunkgenerator now that the world variables have been recieved
        chunkGenerator = new ChunkGenerator(inChunkSize, _data.parameters, this);
        dataManager    = new TerrainDataManager();
        typeManager    = new TerrainTypeManager();

        // DEBUG Generate a chunk in the middle as a test
        _worldChunks.Add(new Vector2DI(0,0), chunkGenerator.GenerateChunk(new Vector2DI(0, 0)));
    }




    public class TerrainDataManager // Holds all terrain models
    {
        // Contains a Dictionary or HashSet where the keys are ETerrainType, and the values are TerrainData
        // The values should be set on server and sent to clients
        Dictionary<ETerrainType, TerrainData> _terrainDataDict = new Dictionary<ETerrainType, TerrainData>();

        public struct TerrainData // The model for every terrain type
        {
            public int textureID;
            public int moveSpeed;
        }


        // Constructor
        public TerrainDataManager()
        {
            // TODO: Recieve the terrain data from the server

            // <DEBUG>
            _terrainDataDict.Add(ETerrainType.Grass, new TerrainData() { textureID = 0, moveSpeed = 1 });
            _terrainDataDict.Add(ETerrainType.Sand,  new TerrainData() { textureID = 1, moveSpeed = 1 });
            // </DEBUG>
        }


        // External
        public TerrainData GetTerrainData(ETerrainType inTerrainType)
        {
            return _terrainDataDict[inTerrainType];
        }
    } 

    public class TerrainTypeManager // Holds biome definitions
    {
        // External
        public ETerrainType GetTerrainType(float inHeight /*, int inTemperature, int inHumidity*/)
        {
            // TODO: Implement this properly
            if (inHeight == 0) return ETerrainType.Grass;
            else return ETerrainType.Sand;
        }

    }
}
