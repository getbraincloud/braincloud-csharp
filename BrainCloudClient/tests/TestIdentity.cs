// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.Common;
using NUnit.Core;
using NUnit.Framework;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestIdentity : TestFixtureBase
    {
        [Test]
        public void TestSwitchToChildProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToChildProfile(
                null,
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.PlayerStateService.DeleteUser(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToSingletonChildProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToParentProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.IdentityService.SwitchToParentProfile(
                ParentLevel,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestDetachParent()
        {
            GoToChildProfile();

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachParent(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void AttachParentWithIdentity()
        {
            GoToChildProfile();

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachParent(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            TestUser user = GetUser(Users.UserA);
            _bc.IdentityService.AttachParentWithIdentity(
                user.Id,
                user.Password,
                AuthenticationType.Universal,
                null,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetChildProfiles()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetChildProfiles(
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestAttachUniversalIdentity()
        {
            string externalId = GenerateUniversalId();

            TestResult tr = new TestResult(_bc);
            _bc.Client.Wrapper.Logout(true, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.Wrapper.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.IdentityService.AttachUniversalIdentity(
                externalId,
                Guid.NewGuid().ToString(),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.IdentityService.DetachUniversalIdentity(
                externalId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestAttachEmailIdentity()
        {
            string email = GenerateEmailId();

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachEmailIdentity(
                email,
                Guid.NewGuid().ToString(),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            _bc.IdentityService.DetachEmailIdentity(
                email,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetIdentites()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetIdentities(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetExpiredIdentites()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetExpiredIdentities(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestRefreshIdentity()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.RefreshIdentity(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                AuthenticationType.Universal,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(400, 40464);
        }

        [Test]
        public void TestAttachPeerProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachPeerProfile(
                PeerName,
                GetUser(Users.UserA).Id + "_peer",
                GetUser(Users.UserA).Password,
                AuthenticationType.Universal,
                null,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DetachPeer();
        }

        [Test]
        public void TestDetachPeer()
        {
            AttachPeer(Users.UserA, AuthenticationType.Universal);

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachPeer(
                PeerName,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPeerProfiles()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetPeerProfiles(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
        
        [Test]
        public void TestAttachNonLoginUniversalId()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachNonLoginUniversalId("braincloudtest@gmail.com", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(202, ReasonCodes.DUPLICATE_IDENTITY_TYPE);
        }

        [Test]
        public void TestUpdateUniversalIdLogin()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.UpdateUniversalIdLogin("braincloudtest@gmail.com", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(400, ReasonCodes.NEW_CREDENTIAL_IN_USE);
        }

        [Test]
        public void TestAttachAdvancedIdentity()
        {
            string externalId = GenerateEmailId();

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachAdvancedIdentity(
                AuthenticationType.Email,
                new AuthenticationIds() { 
                    externalId = externalId,
                    authenticationToken = Guid.NewGuid().ToString(),
                    authenticationSubType = null
                },
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.IdentityService.DetachAdvancedIdentity(
                AuthenticationType.Email,
                externalId,
                true,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestAttachAdvancedIdentityInvalidAuthType()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachAdvancedIdentity(
                AuthenticationType.Unknown,
                new AuthenticationIds()
                {
                    externalId = GenerateUniversalId(),
                    authenticationToken = Guid.NewGuid().ToString(),
                    authenticationSubType = null
                },
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.INVALID_AUTHENTICATION_TYPE);
        }

        [Test]
        public void TestMergeAdvancedIdentity()
        {
            TestResult tr = new TestResult(_bc);

            var ids = CreateEmailUserToMergeWith(tr);

            _bc.IdentityService.MergeAdvancedIdentity(
                AuthenticationType.Email,
                ids,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.IdentityService.DetachAdvancedIdentity(
                AuthenticationType.Email,
                ids.externalId,
                true,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestMergeAdvancedIdentityDuplicateIdentity()
        {
            TestResult tr = new TestResult(_bc);

            var ids = CreateUniversalUserToMergeWith(tr);

            _bc.IdentityService.MergeAdvancedIdentity(
                AuthenticationType.Universal,
                ids,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.DUPLICATE_IDENTITY_TYPE);
        }

        [Test]
        public void TestAttachBlockChain()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachBlockChainIdentity(
                "config",
                "ehhhwwwhhhhh2",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            TestResult tr1 = new TestResult(_bc);
            _bc.IdentityService.DetachBlockChainIdentity(
                "config",
                tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();
        }

        [Test]
        public void TestDetachBlockChain()
        {
            TestResult tr1 = new TestResult(_bc);
            _bc.IdentityService.AttachBlockChainIdentity(
                "config",
                "ew2",                
                tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachBlockChainIdentity(
                "config",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestAttachAdvancedIdentitySubTypeError()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachAdvancedIdentity(
                AuthenticationType.Email,
                new AuthenticationIds()
                {
                    externalId = GenerateEmailId(),
                    authenticationToken = Guid.NewGuid().ToString(),
                    authenticationSubType = "someSortOfSubType"
                },
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.INVALID_AUTHENTICATION_TYPE);
        }

        // Generating our own users
        private static string GenerateUniversalId() => Guid.NewGuid().ToString().Replace("-", string.Empty);

        private static string GenerateEmailId() => $"{GenerateUniversalId()}@bctestuser.com";

        // These logs out of the default user (Users.UserA) and creates a new one before logging back into the default user
        private AuthenticationIds CreateUniversalUserToMergeWith(TestResult tr)
        {
            _bc.Client.Wrapper.Logout(true, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var ids = new AuthenticationIds()
            {
                externalId = GenerateUniversalId(),
                authenticationToken = Guid.NewGuid().ToString(),
                authenticationSubType = null
            };

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                ids.externalId,
                ids.authenticationToken,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.Wrapper.Logout(true, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            return ids;
        }

        private AuthenticationIds CreateEmailUserToMergeWith(TestResult tr)
        {
            _bc.Client.Wrapper.Logout(true, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var ids = new AuthenticationIds()
            {
                externalId = GenerateEmailId(),
                authenticationToken = Guid.NewGuid().ToString(),
                authenticationSubType = null
            };

            _bc.Client.AuthenticationService.AuthenticateEmailPassword(
                ids.externalId,
                ids.authenticationToken,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.Wrapper.Logout(true, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            return ids;
        }
    }
}
