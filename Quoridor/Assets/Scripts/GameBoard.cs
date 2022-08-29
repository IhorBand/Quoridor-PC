using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public GameObject tileWhite; //If use one mGameBoard design, remove the array.
    public GameObject tileWall;
    public GameObject tileNullWall;
    public GameObject tileWhiteWall;

    public Dictionary<DTO.Position, GameObject> WallTiles;
    public Dictionary<DTO.Position, GameObject> WalkableTiles;


    public Dictionary<Vector2Int, Vector2> positions = new Dictionary<Vector2Int, Vector2>();

    public static GameObject Create(GameObject prefab)
    {
        return Instantiate(prefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.WallTiles = new Dictionary<DTO.Position, GameObject>();
        this.WalkableTiles = new Dictionary<DTO.Position, GameObject>();
        DrawChessBoard();
        Debug.Log("GameBoard is ready.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DrawChessBoard()
    {
        float x = 0;
        float y = 0;
        float distanceBetweenTiles = 0.65f;

        for(int row = 0; row <= 18; row++)
        {
            // Tiles To Win
            if (row == 0 || row == 18)
            {
                for (int column = 1; column <= 17; column++)
                {
                    var isTileToMove = column % 2 != 0;

                    if (column > 1)
                    {
                        x -= distanceBetweenTiles;
                    }

                    positions.Add(new Vector2Int(column, row), new Vector2(x, y));
                    Vector3 pos = new Vector3(x, 0, y);

                    GameObject tile = Instantiate(isTileToMove ? tileWhite : tileWhiteWall, pos, Quaternion.identity, this.transform);
                    
                    if (isTileToMove)
                    {
                        var gameBoardTilePosition = tile.GetComponent<GameBoardTilePosition>();
                        gameBoardTilePosition.Position = new DTO.Position(column, row);
                        gameBoardTilePosition.TileType = TileType.WinTile;
                        WalkableTiles.Add(gameBoardTilePosition.Position, tile);
                    }
                }
            }
            else
            {
                // Wall Row
                if (row % 2 == 0)
                {
                    for (int column = 1; column <= 17; column++)
                    {
                        var isTileToPlaceWall = column % 2 != 0;

                        if (column > 1)
                        {
                            x -= distanceBetweenTiles;
                        }

                        positions.Add(new Vector2Int(column, row), new Vector2(x, y));

                        Vector3 pos = new Vector3(x, 0, y);
                        GameObject tile = Instantiate(isTileToPlaceWall ? tileWall : tileNullWall,
                            pos,
                            column % 2 == 0 ? Quaternion.identity : Quaternion.Euler(0f, 90f, 0f),
                            this.transform);

                        if (isTileToPlaceWall)
                        {
                            var gameBoardTilePosition = tile.GetComponent<GameBoardTilePosition>();
                            gameBoardTilePosition.Position = new DTO.Position(column, row);
                            gameBoardTilePosition.TileType = TileType.WallTile;
                            WallTiles.Add(gameBoardTilePosition.Position, tile);
                        }
                    }
                }
                //Movable row
                else
                {
                    for (int column = 1; column <= 17; column++)
                    {
                        var isTileToMove = column % 2 != 0;

                        if (column > 1)
                        {
                            x -= distanceBetweenTiles;
                        }

                        positions.Add(new Vector2Int(column, row), new Vector2(x, y));

                        Vector3 pos = new Vector3(x, 0, y);
                        GameObject tile = Instantiate(isTileToMove ? tileWhite : tileWall , pos, Quaternion.identity, this.transform);

                        var gameBoardTilePosition = tile.GetComponent<GameBoardTilePosition>();
                        gameBoardTilePosition.Position = new DTO.Position(column, row);

                        if (isTileToMove)
                        {
                            gameBoardTilePosition.TileType = TileType.PlayerTile;
                            WalkableTiles.Add(gameBoardTilePosition.Position, tile);
                        }
                        else
                        {
                            gameBoardTilePosition.TileType = TileType.WallTile;
                            WallTiles.Add(gameBoardTilePosition.Position, tile);
                        }
                    }
                }
            }
            if (row == 0 || row == 17)
            {
                y += 1;
            }
            else
            {
                y += distanceBetweenTiles;
            }

            x = 0;
        }

        Debug.Log(positions);
    }
}
