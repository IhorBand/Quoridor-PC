using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration
{
    public string UserName { get; set; }
    public string UserPassword { get; set; }
    public string EndpointGetToken = "https://localhost:7127/api/token";
    public string EndpointGameHub = "https://localhost:7127/hubs/GameHub";
    public Guid GameId { get; set; }

    private static Configuration instance;

    private Configuration()
    { }

    public static Configuration GetInstance()
    {
        if (instance == null)
            instance = new Configuration();
        return instance;
    }

}
