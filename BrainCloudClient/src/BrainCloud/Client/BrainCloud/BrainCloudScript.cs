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
    }
}
