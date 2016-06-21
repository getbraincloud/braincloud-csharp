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
        private BrainCloudClient m_brainCloudClientRef;
        public BrainCloudScript(BrainCloudClient brainCloudClientRef)
        {
            m_brainCloudClientRef = brainCloudClientRef;
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
            m_brainCloudClientRef.SendRequest(sc);
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
            m_brainCloudClientRef.SendRequest(sc);
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
            m_brainCloudClientRef.SendRequest(sc);
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
            m_brainCloudClientRef.SendRequest(sc);
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
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
