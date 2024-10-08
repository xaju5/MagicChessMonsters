using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gameboard : MonoBehaviour
{
    [Header("Battlefield Art")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private TilemapRenderer tilemapRenderer;

    public static Gameboard Instance;
    public static readonly float TILE_SIZE = 1f;
    public static readonly int TILE_COUNT_X = 8;
    public static readonly int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Vector2Int currentHover;

    private void Awake()
    {
        SetUpSingleton();
        GenerateBattleGround();
    }
    private void SetUpSingleton()
    {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HoverSelectedTile();
    }

    private void GenerateBattleGround()
    {
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                tiles[x,y] = GenerateSingleTile(x,y);        
    }
    private GameObject GenerateSingleTile( int x, int y)
    {
        GameObject tileObject = new GameObject($"X:{x},Y:{y}");
        tileObject.transform.SetParent(transform);
        
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer tileObjectRender = tileObject.AddComponent<MeshRenderer>();
        tileObjectRender.material = tileMaterial;
        tileObjectRender.sortingOrder = tilemapRenderer.sortingOrder + 1;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * TILE_SIZE, y * TILE_SIZE, 0) + transform.position;
        vertices[1] = new Vector3(x * TILE_SIZE, (y + 1) * TILE_SIZE, 0) +  transform.position;
        vertices[2] = new Vector3((x + 1) * TILE_SIZE, y * TILE_SIZE, 0) +  transform.position;
        vertices[3] = new Vector3((x + 1) * TILE_SIZE, (y + 1) * TILE_SIZE, 0) +  transform.position;

        int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider2D>();

        return tileObject;
    }

    private void HoverSelectedTile()
    {
        RaycastHit2D hitInfo = CastRayToBoard();

        if (hitInfo.collider != null)
        {
            Vector2Int hitPosition = LookupTileIndex(hitInfo.transform.gameObject);

            //Set up a new hover
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            //Change to other tile hover
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = RestoreTileLayer(currentHover);
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {
            //Remove hover
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = RestoreTileLayer(currentHover);
                currentHover = -Vector2Int.one;
            }
        }
    }
    private RaycastHit2D CastRayToBoard()
    {
        float rayLength = 100f;
        RaycastHit2D hitInfo;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = new Ray(mousePosition, Vector2.zero);
        hitInfo = Physics2D.Raycast(ray.origin, ray.direction, rayLength, LayerMask.GetMask("Tile", "Hover", "Highlight", "Danger"));
        return hitInfo;
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);
        return -Vector2Int.one;
        // throw new Exception("LookupTileIndex_NotFound");
    }

    private LayerMask RestoreTileLayer(Vector2Int index)
    {
        if(BattleManager.Instance.IsValidAttack(index))
            return LayerMask.NameToLayer("Danger");

        if(BattleManager.Instance.IsValidMove(index))
            return LayerMask.NameToLayer("Highlight");
        
        return LayerMask.NameToLayer("Tile");
    }
    //Public methods
    public Vector3 GetTileCenter(int x, int y){
        return new Vector3(x * TILE_SIZE, y * TILE_SIZE, 0) +  transform.position + new Vector3(TILE_SIZE / 2, TILE_SIZE / 2, 0);
    }
    public Vector2Int GetCurrentHover(){
        return currentHover;
    }
    public TilemapRenderer GetTilemapRenderer(){
        return tilemapRenderer;
    }
    public void ChangeTilesLayers(List<Vector2Int> tilesToChange, string layerName){
        foreach (Vector2Int tileIndex in tilesToChange)
            tiles[tileIndex.x, tileIndex.y].layer = LayerMask.NameToLayer(layerName);
    }
    public void SetAllTilesToDefaultLayer(){
        foreach (GameObject tile in tiles)
            tile.layer = LayerMask.NameToLayer("Tile"); 
    }
}
