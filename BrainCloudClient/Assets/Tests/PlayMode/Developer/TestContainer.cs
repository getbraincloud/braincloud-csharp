using System;
using System.Collections;
using System.Collections.Generic;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using Tests.PlayMode;
using UnityEngine;
using UnityEngine.TestTools;
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
    
    public TestUser TestUserA;
    
    public bool m_done;
    public bool m_result;
    public int m_apiCountExpected;
    public Dictionary<string, object> m_response =  new Dictionary<string, object>();

    // if error
    public int m_statusCode;
    public int m_reasonCode;
    public string m_statusMessage;
    public int m_timeToWaitSecs = 300;
    public int m_globalErrorCount;
    public int m_networkErrorCount;
    public int failCount;
    public int successCount;
    public IEnumerator Run(int in_apiCount = 1, bool resetValues = true)
    {
        m_done = false;
        Debug.Log("Running...");
        IsRunning = true;
        if (resetValues)
        {
            Reset();
        }
        m_apiCountExpected = in_apiCount;
            
        yield return StartCoroutine(Spin());
            
        IsRunning = false;
    }

    public IEnumerator RunExpectFail(int in_expectedStatusCode, int in_expectedReasonCode, bool resetValues = true)
    {
        m_done = false;
        Debug.Log("Running...");
        IsRunning = true;
        if (resetValues)
        {
            Reset();
        }
        
        yield return StartCoroutine(Spin());
        
        IsRunning = false;
        if (in_expectedStatusCode != -1 && in_expectedStatusCode == m_statusCode)
        {
            failCount++;
        }
        if (in_expectedReasonCode != -1 && in_expectedReasonCode == m_reasonCode)
        {
            failCount++;
        }
    }

    public IEnumerator Spin()
    {
        var timeBefore = DateTime.Now;
        while (!m_done && (DateTime.Now - timeBefore).TotalSeconds < m_timeToWaitSecs)
        {
            if (bcWrapper)
            {
                bcWrapper.Update();    
            }
            yield return new WaitForFixedUpdate();
        }
    }
    
    public IEnumerator Spin(BrainCloudWrapper wrapper)
    {
        var timeBefore = DateTime.Now;
        while (!m_done && (DateTime.Now - timeBefore).TotalSeconds < m_timeToWaitSecs)
        {
            if (wrapper)
            {
                wrapper.Update();    
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator SetUpNewUser(Users user,bool resetCount = true, BrainCloudWrapper wrapper = null)
    {
        if (!_init)
        {
            Debug.Log(">> Initializing New Random Users");
            BrainCloudWrapper userWrapper = wrapper != null ? wrapper : bcWrapper;
            userWrapper.Client.EnableLogging(true);
                
            Random rand = new Random();

            TestUserA = gameObject.AddComponent<TestUser>();
            IEnumerator setUpUserRoutine = TestUserA.SetUp
            (
                userWrapper,
                user + "_UNITY" + "-",
                rand.Next(),
                this
            );
            
            yield return StartCoroutine(setUpUserRoutine);
            _init = true;
            
            successCount = resetCount ? 0 : successCount;
        }
    }
    
    public IEnumerator SetUpNewUser(BrainCloudWrapper wrapper = null)
    {
        if (!_init)
        {
            Debug.Log(">> Initializing New Random Users");
            BrainCloudWrapper userWrapper = wrapper != null ? wrapper : bcWrapper;
            userWrapper.Client.EnableLogging(true);

            TestUserA = gameObject.AddComponent<TestUser>();
            IEnumerator setUpUserRoutine = TestUserA.SetUp
            (
                userWrapper,
                this
            );
            
            yield return StartCoroutine(setUpUserRoutine);
            _init = true;

            successCount = 0;
        }
    }

    public IEnumerator GoToChildProfile(string in_childAppId)
    {
        bcWrapper.IdentityService.SwitchToSingletonChildProfile
        (
            in_childAppId,
            true,
            ApiSuccess,
            ApiError
        );
        yield return StartCoroutine(Run());
    }

    public IEnumerator DetachPeer(string in_peerName)
    {
        bcWrapper.IdentityService.DetachPeer(in_peerName, ApiSuccess, ApiError);
        yield return StartCoroutine(Run());
    }

    public IEnumerator AttachPeer(string in_peerName, AuthenticationType authType)
    {
        bcWrapper.IdentityService.AttachPeerProfile
            (
                in_peerName, 
                TestUserA.Id + "_peer",
                TestUserA.Password,
                authType,
                null,
                true, 
                ApiSuccess, 
                ApiError
            );
        yield return StartCoroutine(Run());
    }
    
    public void ApiSuccess(string json, object cb)
    {
        Debug.Log("Response Received");
        m_response = bcWrapper.Client.DeserializeJson(json);
        if (m_response == null)
        {
            Debug.Log("Attempting different deserialization....");
            m_response = JsonReader.Deserialize<Dictionary<string, object>>(json);
        }
        m_result = true;
        --m_apiCountExpected;
        successCount++;
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
        failCount++;
        Debug.Log($"Api Error: {jsonError}");
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
        //bcWrapper = null;
        _init = false;
        IsRunning = false;
        m_response =  new Dictionary<string, object>();
        TestUserA = null;
        Debug.Log($"Success Count: {successCount}");
        Debug.Log($"Fail Count: {failCount}");
        failCount = 0;
        successCount = 0;
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