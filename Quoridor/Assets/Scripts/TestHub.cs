using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Text;

public class TestHub : MonoBehaviour
{
    private HubConnection _connection;

    public bool isNeedToSendData = true;


    private string token = string.Empty;
    private bool isHubInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetToken());
    }

    // Update is called once per frame
    void Update()
    {
        if (isNeedToSendData && isHubInitialized)
        {
            _connection.InvokeAsync<string>("SendMessageAsync", "test");
            isNeedToSendData = false;
        }

        if (isHubInitialized == false 
            && string.IsNullOrEmpty(token) == false)
        {
            Debug.Log("Trying to connect to hub...");

            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7127/testhub", connectionsOptions =>
                {
                    connectionsOptions.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            _connection.On<string>("ReceiveMessage", (name) =>
            {
                Debug.Log(name);
            });

            _connection.StartAsync();
            isHubInitialized = true;
        }
    }

    IEnumerator GetToken()
    {
        Debug.Log("Trying to get Token...");

        var json = "{\"email\": \"test@email.com\", \"password\": \"test\"}";


        var request = new UnityWebRequest("https://localhost:7127/api/token", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("accept", "*/*");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            // done
            Debug.Log("Token is here, Baby ! :)");
            Debug.Log(request.downloadHandler.text);
            token = request.downloadHandler.text;
        }
        else if(request.result == UnityWebRequest.Result.InProgress)
        {
            // in progress
            Debug.Log("Still waiting For token...");
        }
        else
        {
            //error
            Debug.Log("Cannot get token from server");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.error);
            Debug.LogError(request.downloadHandler.text);
        }
    }
}
