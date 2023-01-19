using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 1f;

    private GameBoard mGameBoard;
    private GameHubManager mGameHubManager;
    private DTO.Position mCurrentPosition;
    private DTO.Enums.Direction mDirectionToWin;
    public static GameObject Create(GameObject prefab, GameBoard gameBoard, DTO.Position position, DTO.Enums.Direction directionToWin)
    {
        var enemyWorldPosition = gameBoard.WalkableTiles[position].transform.position;

        var gameObject = Instantiate(prefab, new Vector3(enemyWorldPosition.x, 10, enemyWorldPosition.z), Quaternion.Euler(0, 0, 0));
        var enemy = gameObject.GetComponent<Enemy>();
        enemy.mGameBoard = gameBoard;
        enemy.SetPosition(position);
        enemy.SetDirectionToWin(directionToWin);
        return gameObject;
    }

    public void SetPosition(DTO.Position position)
    {
        this.mCurrentPosition = position;
    }
    public void SetDirectionToWin(DTO.Enums.Direction directionToWin)
    {
        this.mDirectionToWin = directionToWin;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.mGameHubManager = GameHubManager.Instance;
        this.mGameHubManager.OnUserMadeMove += OnEnemyMadeMove;
    }

    // Update is called once per frame
    void Update()
    {
        var targetPos = this.mGameBoard.WalkableTiles[this.mCurrentPosition].transform.position;
        targetPos.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * Speed);
    }

    private void OnEnemyMadeMove(Guid userId, DTO.Enums.Direction direction)
    {
        var tilesToMove = 2;

        if (direction == DTO.Enums.Direction.Up)
        {
            this.mCurrentPosition.Y += tilesToMove;
        }
        else if (direction == DTO.Enums.Direction.Down)
        {
            this.mCurrentPosition.Y -= tilesToMove;
        }
        else if (direction == DTO.Enums.Direction.Right)
        {
            this.mCurrentPosition.X -= tilesToMove;
        }
        else if (direction == DTO.Enums.Direction.Left)
        {
            this.mCurrentPosition.X += tilesToMove;
        }
    }
}
