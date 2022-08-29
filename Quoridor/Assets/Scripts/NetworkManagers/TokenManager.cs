using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TokenManager : MonoBehaviour
{
    private Configuration mConfiguration;
    private string mToken = string.Empty;
    private TokenStatus mTokenStatus = TokenStatus.NeedToUpdate;
    private DateTime mTimeToRefreshToken = DateTime.MinValue;
    private static TokenManager mInstance = null;

    public static TokenManager Instance
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
    }

    // Update is called once per frame
    void Update()
    {
        if(DateTime.UtcNow > this.mTimeToRefreshToken 
            && this.mTokenStatus == TokenStatus.Updated)
        {
            Debug.Log("It's time to refresh token");
            this.UpdateToken();
        }

        if(this.mTokenStatus == TokenStatus.NeedToUpdate)
        {
            StartCoroutine(this.GetNewToken());
        }
    }

    public void UpdateToken()
    {
        this.mTokenStatus = TokenStatus.NeedToUpdate;
    }

    public string GetToken()
    {
        return this.mToken;
    }

    public TokenStatus GetTokenStatus()
    {
        return this.mTokenStatus;
    }

    private IEnumerator GetNewToken()
    {
        this.mTokenStatus = TokenStatus.Updating;
        Debug.Log("Trying to get Token...");

        var json = "{\"email\": \"" + this.mConfiguration.UserName + "\", \"password\": \"" + this.mConfiguration.UserPassword + "\"}";

        var request = new UnityWebRequest(this.mConfiguration.EndpointGetToken, "POST");
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
            this.mTimeToRefreshToken = DateTime.UtcNow.AddMinutes(10);
            this.mToken = request.downloadHandler.text;
            this.mTokenStatus = TokenStatus.Updated;
        }
        else if (request.result == UnityWebRequest.Result.InProgress)
        {
            // in progress
            Debug.Log("Still waiting For mToken...");
        }
        else
        {
            //error
            Debug.Log("Cannot get mToken from server");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.error);
            Debug.LogError(request.downloadHandler.text);
        }
    }
}
