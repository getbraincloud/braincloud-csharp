//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections;

namespace BrainCloud.Internal
{
    internal class ServerCall
    {
        #region Constructors

        public ServerCall(ServiceName service, ServiceOperation operation, IDictionary jsonData, ServerCallback callback)
        {
            m_service = service.Value;
            m_operation = operation.Value;
            m_jsonData = jsonData;
            m_callback = callback;
        }

        #endregion

        #region Private

        private ServerCallback m_callback;
        private IDictionary m_jsonData;
        private string m_operation;
        private string m_service;

        #endregion Private

        #region Properties

        private string m_id;
        public string PacketID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        public string Operation
        {
            get
            {
                return GetOperation();
            }
        }

        public string Service
        {
            get
            {
                return GetService();
            }
        }

        #endregion
        public ServerCallback GetCallback()
        {
            return m_callback;
        }


        /// <summary>
        /// Get the type of operation to perform with this service. This value usually represents
        /// a particular server method, ie: read, update, create...
        /// </param>
        /// <returns>The operation</returns>
        public string GetOperation()
        {
            return m_operation;
        }

        /// <summary>
        /// Get the service name (or type) for this service. This value is usually mapped to
        /// a particular server key used to identify this service.
        /// </param>
        /// <returns> Name to identify what type of service this is.</returns>
        public string GetService()
        {
            return m_service;
        }

        /// <summary>
        /// Get the Json Data associated for this service. The original json data going out
        /// with the server call
        /// </param>
        /// <returns> Name to identify what type of service this is.</returns>
        public IDictionary GetJsonData()
        {
            return m_jsonData;
        }
    }
}
