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
    static Dictionary<Vector2DI, Chunk> _worldChunks = new Dictionary<Vector2DI, Chunk>();

    public ChunkGenerator chunkGenerator { get; private set; }



    void Update() 
    {
        foreach (KeyValuePair<Vector2DI, Chunk> entry in _worldChunks)
            entry.Value.RawUpdate();
    }


    // External
    public static Chunk GetChunk(Vector2DI inChunkPos) => _worldChunks[inChunkPos];


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

        // DEBUG Generate a chunk in the middle as a test
        _worldChunks.Add(new Vector2DI(0,0), chunkGenerator.GenerateChunk(new Vector2DI(0, 0)));
    }
}
