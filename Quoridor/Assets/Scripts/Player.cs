using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    private DTO.Enums.Direction mDirectionToWin;
    public static GameObject Create(GameObject prefab, GameBoard Board, DTO.Position playerPosition, bool isAbleToMove, DTO.Enums.Direction directionToWin)
    {
        var playerWorldPosition = Board.WalkableTiles[playerPosition].transform.position;

        var gameObject = Instantiate(prefab, new Vector3(playerWorldPosition.x, 10, playerWorldPosition.z), Quaternion.Euler(0, 0, 0));
        var player = gameObject.GetComponent<Player>();
        player.mGameBoard = Board;
        player.SetIsAbleToMove(isAbleToMove);
        player.SetPosition(playerPosition);
        player.SetDirectionToWin(directionToWin);
        return gameObject;
    }

    public void SetPosition(DTO.Position position)
    {
        this.mCurrentPosition = position;
    }
    
    public void SetIsAbleToMove(bool isAbleToMove)
    {
        this.mIsMyTurn = isAbleToMove;
    }

    public void SetDirectionToWin(DTO.Enums.Direction directionToWin)
    {
        this.mDirectionToWin = directionToWin;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.mGameHubManager = GameHubManager.Instance;
        this.mGameHubManager.OnReceiveMessage += OnMessageReceived;

        this.mGameHubManager.OnMakeMoveSuccess += OnMakeMoveSuccess;
        this.mGameHubManager.OnMakeMoveError += OnMakeMoveError;
        this.mGameHubManager.OnBuildWallSuccess += OnBuildWallSuccess;
        this.mGameHubManager.OnBuildWallError += OnBuildWallError;

        this.mGameHubManager.OnUserBuiltWall += OnEnemyBuiltWall;
        this.mGameHubManager.OnUserMadeMove += OnEnemyMadeMove;

        var mouseAimCamera = Camera.main.gameObject.GetComponent<MouseAimCamera>();
        var cameraTransform = this.transform.Find("CameraPos");
        mouseAimCamera.SetTarget(cameraTransform, new Vector3(0, -1.97f, 2.5f));

        if(this.mDirectionToWin == DTO.Enums.Direction.Down)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
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
        Debug.Log("[Player]IsMyTurn: " + this.mIsMyTurn);

        // reverse
        if(this.mDirectionToWin == DTO.Enums.Direction.Down)
        {
            if(direction == DTO.Enums.Direction.Up)
            {
                direction = DTO.Enums.Direction.Down;
            }
            else if (direction == DTO.Enums.Direction.Down)
            {
                direction = DTO.Enums.Direction.Up;
            }
            else if (direction == DTO.Enums.Direction.Left)
            {
                direction = DTO.Enums.Direction.Right;
            }
            else if (direction == DTO.Enums.Direction.Right)
            {
                direction = DTO.Enums.Direction.Left;
            }
        }

        this.mDirectionToMove = direction;
        this.mIsMovingAuthorized = false;
        this.mGameHubManager.MakeMove(Configuration.GetInstance().GameId, direction);
    }

    private void MovePlayerInDirection()
    {
        if(this.mIsMovingAuthorized)
        {
            var targetPos = this.mGameBoard.WalkableTiles[this.mCurrentPosition].transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * Speed);
        }
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
        Debug.Log("[Player]IsMyTurn: " + this.mIsMyTurn);

        var tilesToMove = 2;

        if (this.mDirectionToMove == DTO.Enums.Direction.Up)
        {
            this.mCurrentPosition.Y += tilesToMove;
        }
        else if (this.mDirectionToMove == DTO.Enums.Direction.Down)
        {
            this.mCurrentPosition.Y -= tilesToMove;
        }
        else if (this.mDirectionToMove == DTO.Enums.Direction.Right)
        {
            this.mCurrentPosition.X -= tilesToMove;
        }
        else if (this.mDirectionToMove == DTO.Enums.Direction.Left)
        {
            this.mCurrentPosition.X += tilesToMove;
        }
    }

    private void OnMakeMoveError(string message)
    {
        this.mIsMyTurn = true;
        Debug.Log("[Player]IsMyTurn: " + this.mIsMyTurn);
        this.mIsMovingAuthorized = false;
        Debug.LogError(message);
    }

    private void OnBuildWallSuccess(string message)
    {
        this.mIsMyTurn = false;
        Debug.Log("[Player]IsMyTurn: " + this.mIsMyTurn);
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
        Debug.Log("[Player]IsMyTurn: " + this.mIsMyTurn);
    }
}
