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
    public delegate void ReceiveMessageDelegate(string message);
    public event ReceiveMessageDelegate OnReceiveMessage;

    private TokenManager mTokenManager;
    private HubConnection mGameConnection;
    private string mHubUrl = "https://localhost:7127/hubs/GameHub";
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
        this.mTokenManager = TokenManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.mHubStatus == HubStatus.NeedToInitialize
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
            .WithUrl(this.mHubUrl, connectionsOptions =>
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
        this.mGameConnection.On<string>("ReceiveMessage", (message) =>
        {
            this.OnReceiveMessage.Invoke(message);
        });
    }
}
