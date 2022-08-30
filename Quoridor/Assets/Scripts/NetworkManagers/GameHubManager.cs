using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Text;
using System;
using UnityEngine.UIElements;

public class GameHubManager : MonoBehaviour
{
    // JoinGame
    public delegate void UserJoinedGameDelegate(string userName, Guid userId, DTO.Position position, DTO.Enums.Direction directionToWin);
    public event UserJoinedGameDelegate OnUserJoinedGame;

    public delegate void JoinGameSuccessDelegate(DTO.Position playerPosition, DTO.Position enemyPosition, bool isFull, bool IsAbleToMove, DTO.Enums.Direction directionToWin);
    public event JoinGameSuccessDelegate OnJoinGameSuccess;

    public delegate void JoinGameErrorDelegate(string message);
    public event JoinGameErrorDelegate OnJoinGameError;

    // LeaveGame
    public delegate void UserLeftGameDelegate(string userName, Guid userId);
    public event UserLeftGameDelegate OnUserLeftGame;

    public delegate void LeaveGameSuccessDelegate(string message);
    public event LeaveGameSuccessDelegate OnLeaveGameSuccess;

    public delegate void LeaveGameErrorDelegate(string message);
    public event LeaveGameErrorDelegate OnLeaveGameError;

    // Make Move
    public delegate void UserMadeWinningMoveDelegate(Guid userId, DTO.Enums.Direction direction);
    public event UserMadeWinningMoveDelegate OnUserMadeWinningMove;

    public delegate void PlayerWonAfterMoveDelegate(string message);
    public event PlayerWonAfterMoveDelegate OnWonAfterMove;

    public delegate void UserMadeMoveDelegate(Guid userId, DTO.Enums.Direction direction);
    public event UserMadeMoveDelegate OnUserMadeMove;

    public delegate void MakeMoveSuccessDelegate(string message);
    public event MakeMoveSuccessDelegate OnMakeMoveSuccess;

    public delegate void MakeMoveErrorDelegate(string message);
    public event MakeMoveErrorDelegate OnMakeMoveError;

    // Build Wall
    public delegate void UserBuiltWallDelegate(Guid userId, DTO.Position positionStart, DTO.Position positionEnd);
    public event UserBuiltWallDelegate OnUserBuiltWall;

    public delegate void BuildWallSuccessDelegate(string message);
    public event BuildWallSuccessDelegate OnBuildWallSuccess;

    public delegate void BuildWallErrorDelegate(string message);
    public event BuildWallErrorDelegate OnBuildWallError;

    // Chat
    public delegate void ReceiveMessageDelegate(string message);
    public event ReceiveMessageDelegate OnReceiveMessage;

    private TokenManager mTokenManager;
    private HubConnection mGameConnection;
    private Configuration mConfiguration;
    private HubStatus mHubStatus = HubStatus.NeedToInitialize;

    private static GameHubManager mInstance = null;

    public static GameHubManager Instance
    {
        get
        {
            return mInstance;
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
        }
        else if (mInstance == this)
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        this.mConfiguration = Configuration.GetInstance();
        this.mTokenManager = TokenManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.mHubStatus != HubStatus.NeedToInitialize
            && this.mTokenManager.GetTokenStatus() != TokenStatus.Updated)
        {
            Debug.Log("token refreshed, so it's time to refresh Hub.");
            this.mHubStatus = HubStatus.NeedToInitialize;
        }

        if (this.mHubStatus == HubStatus.NeedToInitialize
            && this.mTokenManager.GetTokenStatus() == TokenStatus.Updated
            && string.IsNullOrEmpty(this.mTokenManager.GetToken()) == false)
        {
            this.InitializeHub();
        }
    }

    public HubStatus GetHubStatus()
    {
        return this.mHubStatus;
    }

    public void JoinGame(Guid gameId)
    {
        if (this.mHubStatus == HubStatus.Initialized)
        {
            this.mGameConnection.InvokeAsync("JoinGameAsync", gameId);
        }
    }

    public void LeaveGame(Guid gameId)
    {
        if (this.mHubStatus == HubStatus.Initialized)
        {
            this.mGameConnection.InvokeAsync("LeaveGameAsync", gameId);
        }
    }

    public void MakeMove(Guid gameId, DTO.Enums.Direction direction)
    {
        if (this.mHubStatus == HubStatus.Initialized)
        {
            this.mGameConnection.InvokeAsync("MakeMoveAsync", gameId, direction);
        }
    }

    public void BuildWall(Guid gameId, DTO.Position positionStart, DTO.Position positionEnd)
    {
        if (this.mHubStatus == HubStatus.Initialized)
        {
            this.mGameConnection.InvokeAsync("BuildWallAsync", gameId, positionStart, positionEnd);
        }
    }

    public void SendMessageToChat(string message)
    {
        if (this.mHubStatus == HubStatus.Initialized)
        {
            this.mGameConnection.InvokeAsync("SendMessageAsync", message);
        }
    }

    private void InitializeHub()
    {
        this.mHubStatus = HubStatus.Initializing;

        Debug.Log("Trying to connect to hub...");

        this.mGameConnection = new HubConnectionBuilder()
            .WithUrl(this.mConfiguration.EndpointGameHub, connectionsOptions =>
            {
                connectionsOptions.AccessTokenProvider = () => Task.FromResult(this.mTokenManager.GetToken());
            })
            .Build();

        this.RegisterEvents();

        this.mGameConnection.StartAsync();
        this.mHubStatus = HubStatus.Initialized;
    }

    private void RegisterEvents()
    {
        // join game
        this.mGameConnection.On<string, Guid, DTO.Position, DTO.Enums.Direction>("OnUserJoinedGame", (userName, userId, enemyPosition, directionToWin) =>
        {
            this.OnUserJoinedGame.Invoke(userName, userId, enemyPosition, directionToWin);
        });

        this.mGameConnection.On<DTO.Position, DTO.Position, bool, bool, DTO.Enums.Direction>("OnJoinGameSuccess", (playerPosition, enemyPosition, isFull, IsAbleToMove, directionToWin) =>
        {
            this.OnJoinGameSuccess.Invoke(playerPosition, enemyPosition, isFull, IsAbleToMove, directionToWin);
        });

        this.mGameConnection.On<string>("OnJoinGameError", (message) =>
        {
            this.OnJoinGameError.Invoke(message);
        });

        // leave game
        this.mGameConnection.On<string, Guid>("OnUserLeftGame", (userName, userId) =>
        {
            this.OnUserLeftGame.Invoke(userName, userId);
        });

        this.mGameConnection.On<string>("OnLeaveGameSuccess", (message) =>
        {
            this.OnLeaveGameSuccess.Invoke(message);
        });

        this.mGameConnection.On<string>("OnLeaveGameError", (message) =>
        {
            this.OnLeaveGameError.Invoke(message);
        });

        // make move
        this.mGameConnection.On<Guid, DTO.Enums.Direction>("OnUserMadeWinningMove", (userId, direction) =>
        {
            this.OnUserMadeWinningMove.Invoke(userId, direction);
        });

        this.mGameConnection.On<string>("OnPlayerWonAfterMove", (message) =>
        {
            this.OnWonAfterMove.Invoke(message);
        });

        this.mGameConnection.On<Guid, DTO.Enums.Direction>("OnUserMadeMove", (userId, direction) =>
        {
            this.OnUserMadeMove.Invoke(userId, direction);
        });

        this.mGameConnection.On<string>("OnMakeMoveSuccess", (message) =>
        {
            this.OnMakeMoveSuccess.Invoke(message);
        });

        this.mGameConnection.On<string>("OnMakeMoveError", (message) =>
        {
            this.OnMakeMoveError.Invoke(message);
        });

        // build wall
        this.mGameConnection.On<Guid, DTO.Position, DTO.Position>("OnUserBuiltWall", (userId, positionStart, positionEnd) =>
        {
            this.OnUserBuiltWall.Invoke(userId, positionStart, positionEnd);
        });

        this.mGameConnection.On<string>("OnBuildWallSuccess", (message) =>
        {
            this.OnBuildWallSuccess.Invoke(message);
        });

        this.mGameConnection.On<string>("OnBuildWallError", (message) =>
        {
            this.OnBuildWallError.Invoke(message);
        });

        // Chat
        this.mGameConnection.On<string>("ReceiveMessage", (message) =>
        {
            this.OnReceiveMessage.Invoke(message);
        });
    }
}
