//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------
#if (DOT_NET)
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace BrainCloud.Internal
{
    internal class AsynchRequestAndResponse
    {

        #region Properties

        public string JsonRequestString;
        public string JsonResponseString;
        public string ErrorMessage;

        private string authenticationToken = "";
        private int packetIncrementer;
        private bool requestInProgress = false;
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        #endregion

        #region Constructors

        public AsynchRequestAndResponse()
        {
            packetIncrementer = 1;
        }


        #endregion

        #region PublicMethods


        public string PerformAuthenticationRequestResponse(string serverURl, string authToken, string incomingJsonRequestString)
        {
            authenticationToken = authToken; //This will be required for subsequent calls !
            string returnThis = PerformRequestResponse(serverURl, incomingJsonRequestString, null, ServiceOperation.Authenticate.Value);

            return returnThis;
        }


        public string PerformRequestResponse(string serverURl, string incomingJsonRequestString, string sessionId, string callingParameter)
        {

            Console.WriteLine("\n\nBrainCloud - AsynchRequestAndResponse - PerformRequestResponse - START !\n");


            //RESETS, THERE WILL BE NO RESPONSE UNTIL NEW VALUE IS OBTAINED (or 20 secs elapse)
            requestInProgress = true;
            JsonResponseString = "";

            JsonRequestString = incomingJsonRequestString;

            WebRequest request = WebRequest.Create(serverURl);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";

            /*
            //http://www.codeproject.com/Articles/159450/fastJSON
            JSON Json = JSON.Instance;
            Json.Parameters.UseFastGuid = false;
            Json.Parameters.UseOptimizedDatasetSchema = false;
            Json.Parameters.SerializeNullValues = true;
            Json.Parameters.UsingGlobalTypes = false;
            Json.Parameters.UseExtensions = false;
            */
            //replace the 'packetIdPlaceholder' with the actual packetIncrementer value
            JsonRequestString = JsonRequestString.Replace("packetIdPlaceholder", packetIncrementer.ToString());
            Console.WriteLine(">>> BrainCloud - AsynchRequestAndResponse - PerformRequestResponse - REQUEST [" + callingParameter + "]: " + JsonRequestString);

            //JsonRequestString = Json.Beautify(JsonRequestString);
            JsonRequestString = JsonRequestString.Replace("'", "\"");

            //Console.WriteLine("BrainCloud - AsynchRequestAndResponse - authenticationToken: " + authenticationToken);
            string SECRET_KEY = authenticationToken;
            string sig = CalculateMD5Hash(JsonRequestString + SECRET_KEY);

            if (!authenticationToken.Equals(""))
            {
                //This will only get hit if they're authenticating !
                //Console.WriteLine("BrainCloud - AsynchRequestAndResponse - SECRET_KEY: " + SECRET_KEY);
                request.Headers.Add("X-SIG", sig);
            }
            else
            {
                //header values for all other calls
                request.Headers.Add("X-SIG", sessionId + sig);
                //Console.WriteLine("BrainCloud - AsynchRequestAndResponse - PerformRequestResponse - ADDING X-SIG for read: " + sig);
            }

            //Set the length of the message
            byte[] byteArray = Encoding.UTF8.GetBytes(JsonRequestString);
            request.ContentLength = byteArray.Length;


            //-------------------------------
            //ASYNCHRONOUS REQUEST
            try
            {
                request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
                // Keep the main thread from continuing while the asynchronous
                // operation completes.
                allDone.WaitOne();

            }
            catch (Exception ex)
            {

                Debug.WriteLine("BrainCloud - BeginGetRequestStream - EX: " + ex.ToString());
                ErrorMessage = ex.Message;
                return ex.ToString(); //We'll want to know what happened on the app side
            }
            //-------------------------------

            //Increment for next time...reset once it hits a massively high number.
            if (packetIncrementer < Int32.MaxValue)
            {
                packetIncrementer++;
            }
            else
            {
                packetIncrementer = 1;
            }

            //------------------------------------------------------------
            //WAIT FOR THE CALLBACKS TO COMPLETE AND DELIVER RESPONSE JSON
#if !(DOT_NET)
            System.Threading.Timer twentySecTimer = new System.Threading.Timer(OnTimedEvent);
            twentySecTimer.Change(20000, 1);
#else
            System.Timers.Timer twentySecTimer = new System.Timers.Timer(20000); //20 secs
            twentySecTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            twentySecTimer.Interval = 1000; //1 sec
            twentySecTimer.Enabled = true; //start timer !
#endif

            while ((JsonResponseString.Equals("")) || (requestInProgress == true))
            {
                //wait until we get a response, or 20 secs has elapsed
                //the callback methods set the JsonResponseString value, so wait before executing below code
                //if the request fails and we time out, the response will be ""
            }
            //------------------------------------------------------------

            //JsonResponseString = Json.Beautify(JsonResponseString);
            Console.WriteLine(">>> BrainCloud - AsynchRequestAndResponse - PerformRequestResponse - RESPONSE [" + callingParameter + "]: " + JsonResponseString);

            return JsonResponseString;
        }

        #endregion


        #region PrivateMethods

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            //a callback method to handle the connection requests and begin receiving data from the network

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            Stream postStream = request.EndGetRequestStream(asynchronousResult);

            Console.WriteLine("GetRequestStreamCallback - JsonRequestString GOING OUT: " + JsonRequestString);

            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(JsonRequestString);

            // Write to the request stream.
            postStream.Write(byteArray, 0, JsonRequestString.Length);
            postStream.Close();

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }


        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            //a callback method to end receiving the data

            JsonResponseString = ""; //reset
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

            JsonResponseString = streamRead.ReadToEnd();

            //JsonResponseString = Json.Beautify(JsonResponseString);
            Console.WriteLine("GetResponseCallback - JsonResponseString COMING IN: " + JsonResponseString);

            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();
            allDone.Set();
        }


        private static string CalculateMD5Hash(string input)
        {
#if !(DOT_NET)
            MD5Unity.MD5 md5 = MD5Unity.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);  // UTF8, not ASCII
            byte[] hash = md5.ComputeHash(inputBytes);
#else
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);  // UTF8, not ASCII
            byte[] hash = md5.ComputeHash(inputBytes);
#endif

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }


#if !(DOT_NET)
        private void OnTimedEvent(object state)
#else
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
#endif
        {
            requestInProgress = false;
        }

        #endregion



    }

}
#endif
