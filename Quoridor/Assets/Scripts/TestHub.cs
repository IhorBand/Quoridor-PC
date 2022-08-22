using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;

public class TestHub : MonoBehaviour
{
    private HubConnection _connection;

    public bool isNeedToSendData = true;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Trying to connect to hub...");

        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7127/testhub")
            .Build();

        _connection.On<string>("ReceiveMessage", (name) =>
        {
            Debug.Log(name);
        });

        _connection.StartAsync();
    }

    // Update is called once per frame
    void Update()
    {
        if(isNeedToSendData)
        {
            _connection.InvokeAsync<string>("SendMessageAsync", "test");
            isNeedToSendData = false;
        }
    }
}
