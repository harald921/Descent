using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lidgren.Network;

public class Chunk
{
    // Data
    public class Data
    {
        // Data
        public bool isDirty;

        public readonly Vector2DI position;
        Tile[,] tiles;


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
            isDirty = true;
        }

        public void SetTiles(Tile[,] inTiles)
        {
            tiles = inTiles;
            isDirty = true;
        }
    }
    Data _data;
    public Data data => _data;

    // View
    public struct View
    {
        public readonly Mesh mesh;
        public readonly GameObject gameObject;

        public View(GameObject inChunkGO)
        {
            gameObject = inChunkGO;
            mesh = inChunkGO.GetComponent<MeshFilter>().mesh;
        }
    }
    View _view;

    // References
    World _world;



    // Constructor
    public Chunk(Data inData, View inView, World inWorld)
    {
        _data = inData;
        _view = inView;
        _world = inWorld;
    }

    public void RawUpdate()
    {
        if (_data.isDirty)
        {
            // TODO: Regenerate the view somehow
            _world.chunkGenerator.RegenerateChunkView(_data, _view);

            _data.isDirty = false;
        }
    }
}

// public class Chunk : IUnPackable
// {
//     // Data
//     public struct Data
//     {
//         // Data
//         public bool isDirty;
//         Tile[,] _tiles;
// 
// 
//         // Constructor
// 
// 
//         // External
//         public Tile GetTile(int x, int y)
//         {
//             return _tiles[x, y];
//         }
// 
//         public void SetTile(int x, int y, Tile inTile)
//         {
//             _tiles[x, y] = inTile;
// 
//             isDirty = true;
//         }
// 
//         public void SetTiles(Tile[,] inTiles)
//         {
//             _tiles = inTiles;
// 
//             isDirty = true;
//         }
//     }
//     Data _data;
//     public Data data => _data;
// 
//     public struct View
//     {
//         public readonly Mesh       mesh;
//         public readonly GameObject gameObject;
// 
//         public View(GameObject inChunkGO)
//         {
//             gameObject = inChunkGO;
//             mesh = inChunkGO.GetComponent<MeshFilter>().mesh;
//         }
//     }
//     View _view;
// 
// 
//     // References
//     World _terrain;
// 
// 
// 
//     // Constructor
//     public Chunk(Data inData, View inView, World inTerrain)
//     {
//         _data = inData;
//         _view = inView;
//         _terrain = inTerrain;
//     }
// 
// 
// 
//     public void RawUpdate()
//     {
//         if (_data.isDirty)
//         {
//             // TODO: Regenerate the view somehow
//             _terrain.chunkGenerator.RegenerateChunkView(_data, _view);
// 
//             _data.isDirty = false;
//         }
//     }
// 
// 
//     // Networking
//     public void UnpackFrom(NetIncomingMessage inMsg)
//     {
//         // TODO: Here a struct of "changes" should be recieved. Or "difference" from the original chunk
//     }
// }