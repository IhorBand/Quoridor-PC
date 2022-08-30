using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject GameBoard;

    private GameObject mPlayer;
    private GameObject mEnemy;

    private GameHubManager mGameHubManager;
    private bool mIsNeedToJoinGame = true;

    public static GameObject Create(GameObject prefab, GameObject gameBoard)
    {
        var go = Instantiate(prefab);
        var roomManager = go.GetComponent<RoomManager>();
        roomManager.GameBoard = gameBoard;
        return go;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.mGameHubManager = GameHubManager.Instance;
        this.mGameHubManager.OnUserJoinedGame += OnUserJoinedGame;
        this.mGameHubManager.OnJoinGameSuccess += OnJoinGameSuccess;
        this.mGameHubManager.OnJoinGameError += OnJoinGameError;

        this.mGameHubManager.OnUserLeftGame += OnUserLeftGame;
        this.mGameHubManager.OnLeaveGameSuccess += OnLeaveGameSuccess;
        this.mGameHubManager.OnLeaveGameError += OnLeaveGameError;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.mGameHubManager.GetHubStatus() == HubStatus.Initialized)
        {
            if(this.mIsNeedToJoinGame)
            {
                this.mIsNeedToJoinGame = false;
                var configuration = Configuration.GetInstance();
                configuration.GameId = new Guid("d4ba47bb-326e-4904-b0f6-e5104ec1a6e2");
                this.mGameHubManager.JoinGame(configuration.GameId);
            }
        }
    }

    private void OnUserJoinedGame(string userName, Guid userId, DTO.Position enemyPosition, DTO.Enums.Direction directionToMove)
    {
        var gameBoard = this.GameBoard.GetComponent<GameBoard>();
        Debug.Log("Enemy direction To win: " + directionToMove.ToString());
        this.mEnemy = Enemy.Create(EnemyPrefab, gameBoard, enemyPosition, directionToMove);
    }

    private void OnJoinGameSuccess(DTO.Position playerPosition, DTO.Position enemyPosition, bool isFull, bool IsAbleToMove, DTO.Enums.Direction directionToMove)
    {
        this.mIsNeedToJoinGame = false;

        var gameBoard = this.GameBoard.GetComponent<GameBoard>();

        Debug.Log("Player direction To win: " + directionToMove.ToString());
        this.mPlayer = Player.Create(PlayerPrefab, gameBoard, playerPosition, IsAbleToMove, directionToMove);
        
        if (isFull)
        {
            var enemyDirectionToWin = directionToMove == DTO.Enums.Direction.Up ? DTO.Enums.Direction.Down : DTO.Enums.Direction.Up;
            Debug.Log("Enemy direction To win: " + enemyDirectionToWin.ToString());
            this.mEnemy = Enemy.Create(EnemyPrefab, gameBoard, enemyPosition, enemyDirectionToWin);
        }
    }

    private void OnJoinGameError(string message)
    {
        //this.mIsNeedToJoinGame = true;
        Debug.LogError(message);
    }

    private void OnUserLeftGame(string userName, Guid userId)
    {

    }

    private void OnLeaveGameSuccess(string message)
    {
        Destroy(this.mEnemy);
        Destroy(this.mPlayer);
    }

    private void OnLeaveGameError(string message)
    {
        Debug.LogError(message);
    }
}
