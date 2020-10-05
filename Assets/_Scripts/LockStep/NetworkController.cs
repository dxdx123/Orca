using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(-999)]
public class NetworkController : MonoBehaviour
{
    private static NetworkController _instance;

    public static NetworkController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkController>();
                Assert.IsNotNull(_instance);
            }

            return _instance;
        }
    }

    private NetworkConnect _networkConnect;
    
    public string PlayerId { get; set; }
    public int SeatId { get; set; }
    
    private void Awake()
    {
        _networkConnect = new NetworkConnect();
    }

    private void Update()
    {
        _networkConnect.Update();
    }

    private void OnDestroy()
    {
        _networkConnect.DestroyImmediate();
    }
    
    // =========================================
    
    public void Connect(string host, int port, string password)
    {
        _networkConnect.Connect(host, port, password);
    }

    public void SendObjectRaw<T>(byte identifier, T obj)
    {
        _networkConnect.SendObjectRaw(identifier, obj);
    }
}
