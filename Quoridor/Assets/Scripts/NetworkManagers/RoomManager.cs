using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject GameBoardPrefab;

    private GameObject mPlayer;
    private GameObject mEnemy;
    private GameObject mGameBoard;

    private GameHubManager mGameHubManager;
    private bool mIsNeedToJoinGame = true;

    public static GameObject Create(GameObject prefab)
    {
        return Instantiate(prefab);
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
                this.mGameHubManager.JoinGame(new Guid("d4ba47bb-326e-4904-b0f6-e5104ec1a6e2"));
            }
        }
    }

    private void OnUserJoinedGame(string userName, Guid userId)
    {
        var gameBoard = this.mGameBoard.GetComponent<GameBoard>();
        this.mEnemy = Enemy.Create(EnemyPrefab, gameBoard);
    }

    private void OnJoinGameSuccess(DTO.Position playerPosition)
    {
        this.mGameBoard = GameBoard.Create(GameBoardPrefab);
        var gameBoard = this.mGameBoard.GetComponent<GameBoard>();
        this.mPlayer = Player.Create(PlayerPrefab, gameBoard);
        this.mIsNeedToJoinGame = false;
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
        Destroy(this.mGameBoard);
        Destroy(this.mEnemy);
        Destroy(this.mPlayer);
    }

    private void OnLeaveGameError(string message)
    {
        Debug.LogError(message);
    }
}
