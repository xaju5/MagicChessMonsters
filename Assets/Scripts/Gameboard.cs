using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    private int TILE_COUNT_X = 8;
    private int TILE_COUNT_Y = 8;
    private float TILE_SIZE = 1f;
    private GameObject[,] tiles;

    private void Awake()
    {
        GenerateBattleGround();
    }

    private void GenerateBattleGround()
    {
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                tiles[x,y] = GenerateSingleTile(TILE_SIZE,x,y);
        
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0},Y:{1}",x,y));
        tileObject.transform.SetParent(transform);
        
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x + tileSize, y + tileSize, 0);
        vertices[1] = new Vector3(x + tileSize, (y + 1) + tileSize, 0);
        vertices[2] = new Vector3((x + 1) + tileSize, y + tileSize, 0);
        vertices[3] = new Vector3((x + 1) + tileSize, (y + 1) + tileSize, 0);

        int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        tileObject.AddComponent<BoxCollider2D>();

        return tileObject;
    }

}
