//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudScript
    {
        private BrainCloudClient _client;
        public BrainCloudScript(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Executes a script on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - Run
        /// </remarks>
        /// <param name="scriptName">
        /// The name of the script to be run
        /// </param>
        /// <param name="jsonScriptData">
        /// Data to be sent to the script in json format
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void RunScript(
            string scriptName,
            string jsonScriptData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = scriptName;

            if (Util.IsOptionalParameterValid(jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.Run, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Allows cloud script executions to be scheduled
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - ScheduleCloudScript
        /// </remarks>
        /// <param name="scriptName"> Name of script </param>
        /// <param name="jsonScriptData"> JSON bundle to pass to script </param>
        /// <param name="startDateInUTC">  The start date as a DateTime object </param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void ScheduleRunScriptUTC(
            string scriptName,
            string jsonScriptData,
            DateTime startDateInUTC,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = scriptName;

            if (Util.IsOptionalParameterValid(jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.ScriptServiceStartDateUTC.Value] = Util.DateTimeToBcTimestamp(startDateInUTC);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.ScheduleCloudScript, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Allows cloud script executions to be scheduled
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - ScheduleCloudScript
        /// </remarks>
        /// <param name="scriptName"> Name of script </param>
        /// <param name="jsonScriptData"> JSON bundle to pass to script </param>
        /// <param name="minutesFromNow"> Number of minutes from now to run script </param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void ScheduleRunScriptMinutes(
            string scriptName,
            string jsonScriptData,
            long minutesFromNow,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = scriptName;

            if (Util.IsOptionalParameterValid(jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.ScriptServiceStartMinutesFromNow.Value] = minutesFromNow;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.ScheduleCloudScript, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Run a cloud script in a parent app
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - RUN_PARENT_SCRIPT
        /// </remarks>
        /// <param name="scriptName"> Name of script </param>
        /// <param name="jsonScriptData"> JSON bundle to pass to script </param>
        /// <param name="parentLevel"> The level name of the parent to run the script from </param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void RunParentScript(
            string scriptName,
            string jsonScriptData,
            string parentLevel,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = scriptName;

            if (Util.IsOptionalParameterValid(jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.ScriptServiceParentLevel.Value] = parentLevel;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.RunParentScript, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Cancels a scheduled cloud code script
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - CANCEL_SCHEDULED_SCRIPT
        /// </remarks>
        /// <param name="jobId"> ID of script job to cancel </param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void CancelScheduledScript(
            string jobId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceJobId.Value] = jobId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.CancelScheduledScript, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Runs a script from the context of a peer
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - RUN_PEER_SCRIPT
        /// </remarks>
        /// <param name="scriptName">The name of the script to run</param>
        /// <param name="jsonScriptData">JSON data to pass into the script</param>
        /// <param name="peer">Identifies the peer</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The user object sent to the callback</param>
        public void RunPeerScript(
            string scriptName,
            string jsonScriptData,
            string peer,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = scriptName;

            if (Util.IsOptionalParameterValid(jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.Peer.Value] = peer;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.RunPeerScript, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Runs a script asynchronously from the context of a peer
        /// This operation does not wait for the script to complete before returning
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - RUN_PEER_SCRIPT
        /// </remarks>
        /// <param name="scriptName">The name of the script to run</param>
        /// <param name="jsonScriptData">JSON data to pass into the script</param>
        /// <param name="peer">Identifies the peer</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The user object sent to the callback</param>
        public void RunPeerScriptAsync(
            string scriptName,
            string jsonScriptData,
            string peer,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = scriptName;

            if (Util.IsOptionalParameterValid(jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.Peer.Value] = peer;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.RunPeerScriptAsync, data, callback);
            _client.SendRequest(sc);
        }
    }
}
