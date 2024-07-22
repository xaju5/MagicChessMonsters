using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    [Header("ART")]
    [SerializeField] private Material tileMaterial;

    private int TILE_COUNT_X = 8;
    private int TILE_COUNT_Y = 8;
    private float TILE_SIZE = 1f;
    private GameObject[,] tiles;
    private Camera currentCamera;

    private void Awake()
    {
        GenerateBattleGround();
    }
    private void Update() {
        if(!currentCamera){
            currentCamera = Camera.current;
            return;
        }

        RaycastHit info;
        Ray ray= currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile"))){
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);
        }
    }

    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        throw new Exception("LookupTileIndex_NotFound");
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
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, y * tileSize, 0);
        vertices[1] = new Vector3(x * tileSize, (y + 1) * tileSize, 0);
        vertices[2] = new Vector3((x + 1) * tileSize, y * tileSize, 0);
        vertices[3] = new Vector3((x + 1) * tileSize, (y + 1) * tileSize, 0);

        int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider2D>();

        return tileObject;
    }

}
