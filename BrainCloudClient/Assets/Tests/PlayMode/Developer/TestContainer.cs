using System;
using System.Collections;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using Tests.PlayMode;
using UnityEngine;
using Random = System.Random;

/*
 * The main unit testing object that will run with an Update loop with the following features:
 * - Sets Up new user for authentication
 * - Will "Spin" to allow tests to be updated over time
 * - Generic Api Success and Fail callbacks
 */

public enum Users { UserA, UserB, UserC }
public class TestContainer : MonoBehaviour
{
    public BrainCloudWrapper bcWrapper;
    public Server Server;
    public bool IsRunning;
    private static bool _init;
    
    protected TestUser _currentUser;
    
    public bool m_done;
    public bool m_result;
    public int m_apiCountExpected;
    public Dictionary<string, object> m_response =  new Dictionary<string, object>();

    // if error
    public int m_statusCode;
    public int m_reasonCode;
    public string m_statusMessage;
    public int m_timeToWaitSecs = 120;
    public int m_globalErrorCount;
    public int m_networkErrorCount;

    public void StartRun()
    {
        m_done = false;
        StartCoroutine(Run());
    }
    
    public void RunAuth()
    {
        StartCoroutine(SetUpAuth());
    }
    
    public IEnumerator Run(int in_apiCount = 1)
    {
        Debug.Log("Running...");
        IsRunning = true;
        Reset();
        m_apiCountExpected = in_apiCount;
            
        var timeBefore = DateTime.Now;
        //Spin()
        while (!m_done && (DateTime.Now - timeBefore).TotalSeconds < m_timeToWaitSecs)
        {
            if (bcWrapper)
            {
                bcWrapper.Update();    
            }
            yield return new WaitForFixedUpdate();
        }
            
        IsRunning = false;
    }
    
    public IEnumerator SetUpAuth()
    {
        Debug.Log("Set Up Authentication Started...");

        StartCoroutine(SetUpNewUser(Users.UserA));
            
        //Loop until user is set up
        while (!_init)
        {
            yield return new WaitForFixedUpdate();
        }
    }
    
    private IEnumerator SetUpNewUser(Users user)
    {
        if (!_init)
        {
            Debug.Log(">> Initializing New Random Users");
            bcWrapper.Client.EnableLogging(true);
                
            Random rand = new Random();

            _currentUser = gameObject.AddComponent<TestUser>();
            IEnumerator routine = _currentUser.SetUp
            (
                bcWrapper,
                user + "_CS" + "-",
                rand.Next(),
                this
            );
                
            StartCoroutine(routine);
            while (_currentUser.IsRunning) 
                yield return new WaitForFixedUpdate();
            _init = true;
        }
    }
    
    public void ApiSuccess(string json, object cb)
    {
        m_response = JsonReader.Deserialize<Dictionary<string, object>>(json);
        m_result = true;
        --m_apiCountExpected;
            
        if (m_apiCountExpected <= 0)
        {
            m_done = true;
        }
    }

    public void ApiError(int statusCode, int reasonCode, string jsonError, object cb)
    {
        m_statusCode = statusCode;
        m_reasonCode = reasonCode;
        m_statusMessage = jsonError;
        m_result = false;
        --m_apiCountExpected;
        if (m_apiCountExpected <= 0)
        {
            m_done = true;
        }
    }
    
    public void Reset()
    {
        m_done = false;
        m_result = false;
        m_apiCountExpected = 0;
        m_response = null;
        m_statusCode = 0;
        m_reasonCode = 0;
        m_statusMessage = null;
        m_globalErrorCount = 0;
        m_networkErrorCount = 0;
    }
    
    public void CleanUp()
    {
        Reset();
        Server = null;
        bcWrapper = null;
        _init = false;
        IsRunning = false;
        m_response =  new Dictionary<string, object>();
        _currentUser = null;
    }
}

public class Server
{
    public string Host;
    public int WsPort = -1;
    public int TcpPort = -1;
    public int UdpPort = -1;
    public string Passcode;
    public string LobbyId;

    public Server(Dictionary<string, object> serverJson)
    {
        var connectData = serverJson["connectData"] as Dictionary<string, object>;
        var ports = connectData["ports"] as Dictionary<string, object>;

        Host = connectData["address"] as string;
        WsPort = (int)ports["ws"];
        TcpPort = (int)ports["tcp"];
        UdpPort = (int)ports["udp"];
        Passcode = serverJson["passcode"] as string;
        LobbyId = serverJson["lobbyId"] as string;
    }
}