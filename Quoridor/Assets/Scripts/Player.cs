using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Guid GameId;
    public string Message;
    public bool IsNeedToSendData = false;
    public int MessageCount = 0;
    public float Speed = 1;

    private GameBoard mGameBoard;
    private bool mIsMyTurn = false;
    private bool mIsMovingAuthorized = false;
    private DTO.Enums.Direction mDirectionToMove;
    private GameHubManager mGameHubManager;
    private DTO.Position mCurrentPosition;

    public static GameObject Create(GameObject prefab, GameBoard Board)
    {
        var gameObject = Instantiate(prefab);
        var player = gameObject.GetComponent<Player>();
        player.mGameBoard = Board;
        return gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.mGameHubManager = GameHubManager.Instance;
        this.mGameHubManager.OnReceiveMessage += OnMessageReceived;

        this.mGameHubManager.OnJoinGameSuccess += OnJoinGameSuccess;
        
        this.mGameHubManager.OnMakeMoveSuccess += OnMakeMoveSuccess;
        this.mGameHubManager.OnMakeMoveError += OnMakeMoveError;
        this.mGameHubManager.OnBuildWallSuccess += OnBuildWallSuccess;
        this.mGameHubManager.OnBuildWallError += OnBuildWallError;

        this.mGameHubManager.OnUserBuiltWall += OnEnemyBuiltWall;
        this.mGameHubManager.OnUserMadeMove += OnEnemyMadeMove;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.mGameHubManager.GetHubStatus() == HubStatus.Initialized)
        {
            if(this.IsNeedToSendData)
            {
                this.mGameHubManager.SendMessageToChat(Message);
            }

            if (this.mIsMyTurn)
            {
                if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
                {
                    this.SendMessageToMovePlayerInDirection(DTO.Enums.Direction.Up);
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
                {
                    this.SendMessageToMovePlayerInDirection(DTO.Enums.Direction.Down);
                }
                else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
                {
                    this.SendMessageToMovePlayerInDirection(DTO.Enums.Direction.Left);
                }
                else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
                {
                    this.SendMessageToMovePlayerInDirection(DTO.Enums.Direction.Right);
                } 
            }
        }

        MovePlayerInDirection();
    }

    private void SendMessageToMovePlayerInDirection(DTO.Enums.Direction direction)
    {
        this.mIsMyTurn = false;
        this.mDirectionToMove = direction;
        this.mIsMovingAuthorized = false;
        this.mGameHubManager.MakeMove(GameId, direction);
    }

    private void MovePlayerInDirection()
    {
        if(this.mIsMovingAuthorized)
        {
            var targetPos = this.mGameBoard.WalkableTiles[this.mCurrentPosition].transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * Speed);
        }
    }

    private void OnJoinGameSuccess(DTO.Position playerPosition)
    {
        this.mCurrentPosition = playerPosition;
    }    

    private void OnMessageReceived(string message)
    {
        Debug.Log(message);
        MessageCount++;
    }

    private void OnMakeMoveSuccess(string message)
    {
        this.mIsMovingAuthorized = true;
        this.mIsMyTurn = false;
        
        if (this.mDirectionToMove == DTO.Enums.Direction.Up)
        {
            this.mCurrentPosition.Y += 1;
        }
        else if (this.mDirectionToMove == DTO.Enums.Direction.Down)
        {
            this.mCurrentPosition.Y -= 1;
        }
        else if (this.mDirectionToMove == DTO.Enums.Direction.Right)
        {
            this.mCurrentPosition.X += 1;
        }
        else if (this.mDirectionToMove == DTO.Enums.Direction.Left)
        {
            this.mCurrentPosition.X -= 1;
        }
    }

    private void OnMakeMoveError(string message)
    {
        this.mIsMyTurn = true;
        this.mIsMovingAuthorized = false;
        Debug.LogError(message);
    }

    private void OnBuildWallSuccess(string message)
    {
        this.mIsMyTurn = false;
    }

    private void OnBuildWallError(string message)
    {
        this.mIsMyTurn = true;
        Debug.LogError(message);
    }

    private void OnEnemyBuiltWall(Guid userId, DTO.Position positionStart, DTO.Position positionEnd)
    {
        this.mIsMyTurn = true;
    }

    private void OnEnemyMadeMove(Guid userId, DTO.Enums.Direction direction)
    {
        this.mIsMyTurn = true;
    }
}
