using System.Collections;
using System.Collections.Generic;
using Tests.PlayMode;
using UnityEngine;

/// <summary>
/// Holds data for a randomly generated user
/// </summary>
public class TestUser : MonoBehaviour
{
    public string Id = "";
    public string Password = "";
    public string ProfileId = "";
    public string Email = "";

    BrainCloudWrapper _bc;

    public TestUser(BrainCloudWrapper bc, string idPrefix, int suffix)
    {
        _bc = bc;

        Id = idPrefix + suffix;
        Password = Id;
        Email = Id + "@bctestuser.com";
        StartCoroutine(Authenticate());
    }

    public IEnumerator SetUpUser(BrainCloudWrapper bc, string idPrefix, int suffix)
    {
        _bc = bc;

        Id = idPrefix + suffix;
        Password = Id;
        Email = Id + "@bctestuser.com";
        yield return Authenticate();
    }

    private IEnumerator Authenticate()
    {
        GameObject gameObject = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        TestFixtureBase tr = gameObject.AddComponent<TestFixtureBase>();
        tr._bc = _bc;
        _bc.Client.AuthenticationService.AuthenticateUniversal(
            Id,
            Password,
            true,
            tr.ApiSuccess, tr.ApiError);
        yield return StartCoroutine(tr.Run());
            
        ProfileId = _bc.Client.AuthenticationService.ProfileId;

        if (((string)((Dictionary<string, object>)tr.m_response["data"])["newUser"]) == "true")
        {
            _bc.MatchMakingService.EnableMatchMaking(tr.ApiSuccess, tr.ApiError);
            yield return StartCoroutine(tr.Run());
            _bc.PlayerStateService.UpdateUserName(Id, tr.ApiSuccess, tr.ApiError);
            yield return StartCoroutine(tr.Run());
            _bc.PlayerStateService.UpdateContactEmail("braincloudunittest@gmail.com", tr.ApiSuccess, tr.ApiError);
            yield return StartCoroutine(tr.Run());
        }

        _bc.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
        yield return StartCoroutine(tr.Run());
    }
}
