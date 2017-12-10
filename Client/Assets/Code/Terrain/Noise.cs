using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lidgren.Network;

public class Noise
{
    // Parameters
    public struct Parameters : IUnPackable
    {
        public uint  scale;
        public uint  octaves;
        public float persistance;
        public float lacunarity;
        public int   seed;

        // Networking
        public void UnpackFrom(NetIncomingMessage inMsg)
        {
            scale       = inMsg.ReadVariableUInt32();
            octaves     = inMsg.ReadVariableUInt32();
            persistance = inMsg.ReadSingle();
            lacunarity  = inMsg.ReadSingle();
            seed        = (int)inMsg.ReadVariableUInt32(); // TODO: Make a struct which is a networkable int

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
 