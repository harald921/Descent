using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lidgren.Network;

public class Chunk : IUnPackable
{
    // Data
    public struct Data
    {
        // Data
        public bool isDirty;


        // References
        Tile[,] _tiles;


        // External
        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }

        public void SetTile(int x, int y, Tile inTile)
        {
            _tiles[x, y] = inTile;

            isDirty = true;
        }

        public void SetTiles(Tile[,] inTiles)
        {
            _tiles = inTiles;

            isDirty = true;
        }
    }
    Data _data;

    public struct View
    {
        public readonly Mesh       mesh;
        public readonly GameObject gameObject;

        public View(GameObject inChunkGO)
        {
            gameObject = inChunkGO;
            mesh = inChunkGO.GetComponent<MeshFilter>().mesh;
        }
    }
    View _view;


    // References
    World _terrain;



    // Constructor
    public Chunk(Data inData, View inView, World inTerrain)
    {
        _data = inData;
        _view = inView;
        _terrain = inTerrain;
    }



    public void RawUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _data.SetTile(0, 0, new Tile(ETerrainType.Sand));
            _data.SetTile(2, 0, new Tile(ETerrainType.Sand));

        }

        if (_data.isDirty)
        {
            // TODO: Regenerate the view somehow
            _terrain.chunkGenerator.RegenerateChunkView(_data, _view);

            _data.isDirty = false;
        }
    }


    // Networking
    public void UnpackFrom(NetIncomingMessage inMsg)
    {
        // TODO: Here a struct of "changes" should be recieved. Or "difference" from the original chunk
    }
}