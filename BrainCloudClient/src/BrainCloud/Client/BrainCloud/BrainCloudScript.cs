//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
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
        public BrainCloudScript(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Executes a script on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - Run
        /// </remarks>
        /// <param name="in_scriptName">
        /// The name of the script to be run
        /// </param>
        /// <param name="in_jsonScriptData">
        /// Data to be sent to the script in json format
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":null //// this value depends on what the script returns
        /// }
        /// @see The API documentation site for more details on cloud code
        /// </returns>
        public void RunScript(
            string in_scriptName,
            string in_jsonScriptData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = in_scriptName;

            if (Util.IsOptionalParameterValid(in_jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_scriptName"> Name of script </param>
        /// <param name="in_jsonScriptData"> JSON bundle to pass to script </param>
        /// <param name="in_startDateInUTC">  The start date as a DateTime object </param>
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "result": {},
        ///         "scriptName": "testScript",
        ///         "jobId": "48266b95-d197-464d-bb6b-da70aa1e22a9",
        ///         "runState": "Scheduled",
        ///         "description": null,
        ///         "gameId": "10170",
        ///         "runEndTime": 0,
        ///         "parameters": {
        ///             "testParm1": 1
        ///         },
        ///         "runStartTime": 0,
        ///         "scheduledStartTime": 1437576422378
        ///     }
        /// }
        /// @see The API documentation site for more details on cloud code
        /// </returns>
        public void ScheduleRunScriptUTC(
            string in_scriptName,
            string in_jsonScriptData,
            DateTime in_startDateInUTC,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = in_scriptName;

            if (Util.IsOptionalParameterValid(in_jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.ScriptServiceStartDateUTC.Value] = Util.DateTimeToBcTimestamp(in_startDateInUTC);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_scriptName"> Name of script </param>
        /// <param name="in_jsonScriptData"> JSON bundle to pass to script </param>
        /// <param name="in_minutesFromNow"> Number of minutes from now to run script </param>
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "result": {},
        ///         "scriptName": "testScript",
        ///         "jobId": "48266b95-d197-464d-bb6b-da70aa1e22a9",
        ///         "runState": "Scheduled",
        ///         "description": null,
        ///         "gameId": "10170",
        ///         "runEndTime": 0,
        ///         "parameters": {
        ///             "testParm1": 1
        ///         },
        ///         "runStartTime": 0,
        ///         "scheduledStartTime": 1437576422378
        ///     }
        /// }
        /// @see The API documentation site for more details on cloud code
        /// </returns>
        public void ScheduleRunScriptMinutes(
            string in_scriptName,
            string in_jsonScriptData,
            long in_minutesFromNow,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = in_scriptName;

            if (Util.IsOptionalParameterValid(in_jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.ScriptServiceStartMinutesFromNow.Value] = in_minutesFromNow;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.ScheduleCloudScript, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Run a script from the Parent Profile
        /// </summary>
        /// <remarks>
        /// Service Name - Script
        /// Service Operation - RunAsParent
        /// </remarks>
        /// <param name="in_scriptName"> Name of script </param>
        /// <param name="in_jsonScriptData"> JSON bundle to pass to script </param>
        /// <param name="in_parentLevel"> The level name of the parent to run the script from </param>
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// @see The API documentation site for more details on cloud code
        /// </returns>
        /// /*
        /// 
        public void RunAsParent(
            string in_scriptName,
            string in_jsonScriptData,
            string in_parentLevel,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ScriptServiceRunScriptName.Value] = in_scriptName;

            if (Util.IsOptionalParameterValid(in_jsonScriptData))
            {
                Dictionary<string, object> scriptData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonScriptData);
                data[OperationParam.ScriptServiceRunScriptData.Value] = scriptData;
            }

            data[OperationParam.ScriptServiceParentLevel.Value] = in_parentLevel;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Script, ServiceOperation.RunAsParent, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
