using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    class World : IInPackable
    {
        // Data
        Noise.Parameters[] _parameters = new Noise.Parameters[]
        {
            new Noise.Parameters() // Heightmap
            {
                scale       = 50,
                octaves     = 7,
                persistance = 1.0f,
                lacunarity  = 1.0f,
                seed        = 0
            }
        };

        const uint _worldSize = 4;
        const uint _chunkSize = 64;

        // References
        Dictionary<Vector2DI, Chunk> _worldChunks = new Dictionary<Vector2DI, Chunk>();




        // Constructor
        public World()
        {
            for (int x = 0; x < _worldSize; x++)
                for (int y = 0; y < _worldSize; y++)
                    _worldChunks.Add(new Vector2DI(x,y), new Chunk(new Vector2DI(x,y), (int)_chunkSize));


            Network.OnPlayerJoin += (NetConnection inConnection) =>
            {
                Console.WriteLine("Beginning world data");
                Network.Send(this, EDataPacketTypes.WorldData, inConnection, NetDeliveryMethod.ReliableUnordered);
                Console.WriteLine("World data sent");
            };
        }



        // Networking
        public int GetPacketSize()
        {
            int bitsNeeded = 0;

            bitsNeeded += NetUtility.BitsToHoldUInt(_chunkSize);

            bitsNeeded += NetUtility.BitsToHoldUInt((uint)_parameters.Length);
            for (int i = 0; i < _parameters.Length; i++)
                bitsNeeded += _parameters[i].GetPacketSize();

            return bitsNeeded;
        }

        public void PackInto(NetOutgoingMessage inMsg)
        {
            inMsg.WriteVariableUInt32(_chunkSize);               // Chunk size

            inMsg.WriteVariableUInt32((uint)_parameters.Length); // Number of parameters
            for (int i = 0; i < _parameters.Length; i++)         // Parameters
                _parameters[i].PackInto(inMsg);
        }
    }
}
