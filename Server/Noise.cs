using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Descent_Server
{
    public class Noise
    {
        // Parameters
        public struct Parameters : IInPackable
        {
            public uint  scale;
            public uint  octaves;
            public float persistance;
            public float lacunarity;
            public uint  seed;

            // Networking
            public int GetPacketSize()
            {
                int bitsNeeded = 0;
                bitsNeeded += NetUtility.BitsToHoldUInt(scale);
                bitsNeeded += NetUtility.BitsToHoldUInt(octaves);
                bitsNeeded += 32;
                bitsNeeded += 32;
                bitsNeeded += NetUtility.BitsToHoldUInt(seed);
                return bitsNeeded;
            }

            public void PackInto(NetOutgoingMessage inMsg)
            {
                inMsg.WriteVariableUInt32(scale);
                inMsg.WriteVariableUInt32(octaves);
                inMsg.Write(persistance);
                inMsg.Write(lacunarity);
                inMsg.WriteVariableUInt32(seed);
            }
        }


        // External
        public static float[,] Generate(uint inSize, Parameters inParameters, Vector2DI inOffset)
        {
            // TODO: Find a perlin noise lib and use it

            //uint scale = inParameters.scale;
            //uint octaves = inParameters.octaves;
            //float persistance = inParameters.persistance;
            //float lacunarity = inParameters.lacunarity;
            //
            //float amplitude = 1;
            //
            System.Random rng = new System.Random((int)inParameters.seed);
            //Vector2[] octaveOffsets = new Vector2[octaves];
            //for (int i = 0; i < octaves; i++)
            //{
            //    float octaveOffsetX = rng.Next(-100000, 100000) + (inOffset.x * inSize);
            //    float octaveOffsetY = rng.Next(-100000, 100000) - (inOffset.y * inSize);
            //    octaveOffsets[i] = new Vector2(octaveOffsetX, octaveOffsetY);
            //
            //    amplitude *= persistance;
            //}
            //
            ////inSize += 1;

            float[,] noiseMap = new float[inSize, inSize];

            // float halfSize = inSize / 2f;
            // 
             for (int y = 0; y < inSize; y++)
                 for (int x = 0; x < inSize; x++)
                 {
                    noiseMap[x, y] = rng.Next(0, 2);
            //         amplitude = 1;
            //         float frequency = 1;
            //         float noiseHeight = 0;
            // 
            //         for (int i = 0; i < octaves; i++)
            //         {
            //             float sampleX = (x - halfSize + octaveOffsets[i].x) / scale * frequency;
            //             float sampleY = (y - halfSize + octaveOffsets[i].y) / scale * frequency;
            // 
            //             float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            //             noiseHeight += perlinValue * amplitude;
            // 
            //             amplitude *= persistance;
            //             frequency *= lacunarity;
            //         }
            // 
            //         noiseMap[x, y] = noiseHeight * inParameters.rangeMultiplier;
                 }
            // 
            return noiseMap;
        }
    }

}
