using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string message;
    public bool isNeedToSendData = false;
    public int messageCount = 0;

    private GameHubManager mGameHubManager;

    // Start is called before the first frame update
    void Start()
    {
        this.mGameHubManager = GameHubManager.Instance;
        this.mGameHubManager.OnReceiveMessage += OnMessageReceived;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.mGameHubManager.GetHubStatus() == HubStatus.Initialized)
        {
            if(this.isNeedToSendData)
            {
                this.mGameHubManager.SendMessageToChat(message);
            }
        }
    }

    public void OnMessageReceived(string message)
    {
        Debug.Log(message);
        messageCount++;
    }
}
