using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    public class World
    {
        // Data
        class Data : IInPackable
        {
            public const uint chunkSize = 64;

            public static readonly Noise.Parameters[] parameters = new Noise.Parameters[]
            {
                // Height map
                new Noise.Parameters()
                {
                    scale       = 50,
                    octaves     = 7,
                    persistance = 1.01f,
                    lacunarity  = 1.01f,
                    seed        = 0
                }
            };




            // Networking
            public int GetPacketSize()
            {
                int bitsNeeded = 0;
                bitsNeeded += NetUtility.BitsToHoldUInt(chunkSize);
                bitsNeeded += NetUtility.BitsToHoldUInt((uint)parameters.Length);

                for (int i = 0; i < parameters.Length; i++)
                    bitsNeeded += parameters[i].GetPacketSize();

                return bitsNeeded;
            }

            public void PackInto(NetOutgoingMessage inMsg)
            {
                inMsg.WriteVariableUInt32(chunkSize);
                inMsg.WriteVariableUInt32((uint)parameters.Length);

                for (int i = 0; i < parameters.Length; i++)
                    parameters[i].PackInto(inMsg);
            }
        }
        Data _data = new Data();


        // References
        static Dictionary<Vector2DI, Chunk> _worldChunks = new Dictionary<Vector2DI, Chunk>();

        public ChunkGenerator chunkGenerator { get; private set; }


        // Constructor
        public World()
        {
            chunkGenerator = new ChunkGenerator(Data.chunkSize, Data.parameters, this);

            // DEBUG:
            _worldChunks.Add(Vector2DI.Zero, chunkGenerator.GenerateChunk(Vector2DI.Zero));

            Network.OnPlayerJoin += (NetConnection inConnection) =>
            {
                Network.Send(_data, EDataPacketTypes.WorldData, inConnection, NetDeliveryMethod.ReliableUnordered);
            };
        }


        // External
        public static Chunk GetChunk(Vector2DI inChunkPos) => _worldChunks[inChunkPos];

    }
}
