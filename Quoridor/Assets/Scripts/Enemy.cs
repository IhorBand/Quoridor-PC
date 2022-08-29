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

    public static GameObject Create(GameObject prefab, GameBoard gameBoard)
    {
        var gameObject = Instantiate(prefab);
        var enemy = gameObject.GetComponent<Enemy>();
        enemy.mGameBoard = gameBoard;
        return gameObject;
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
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * Speed);
    }

    private void OnEnemyMadeMove(Guid userId, DTO.Enums.Direction direction)
    {
        if (direction == DTO.Enums.Direction.Up)
        {
            this.mCurrentPosition.Y += 1;
        }
        else if (direction == DTO.Enums.Direction.Down)
        {
            this.mCurrentPosition.Y -= 1;
        }
        else if (direction == DTO.Enums.Direction.Right)
        {
            this.mCurrentPosition.X += 1;
        }
        else if (direction == DTO.Enums.Direction.Left)
        {
            this.mCurrentPosition.X -= 1;
        }
    }
}
