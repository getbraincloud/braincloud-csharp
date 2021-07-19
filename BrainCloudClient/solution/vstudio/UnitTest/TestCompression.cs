using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BrainCloud;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestCompression : TestFixtureBase
    {
        
        private readonly string _defaultEntityType = "placeholderText";
        private readonly string _defaultEntityValueName = "Lorem ipsum";
        private readonly string _entityValuePath = "C:/Users/Public/TestFile.txt";
        [Test]
        public void TestCompressEntity()
        {
            TestResult tr = new TestResult(_bc);
            var entityValueFromFile =  File.ReadAllText(_entityValuePath);;
            //No compression
            _bc.EntityService.CreateEntity
            (
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, entityValueFromFile),
                new ACL(ACL.Access.None).ToJsonString(),
                tr.ApiSuccess,
                tr.ApiError
            );
            Console.WriteLine("UNCOMPRESSED REQUEST PACKET SIZE: " + MimicPacketSize(false, entityValueFromFile));
            tr.Run();
            if (tr.m_done)
            {
                ReadResponsePacketSize(tr.m_response, false);
            }
            
            //Compression
            _bc.Client.EnableCompression(true);
            _bc.EntityService.CreateEntity
                (
                    _defaultEntityType,
                    Helpers.CreateJsonPair(_defaultEntityValueName, entityValueFromFile),
                    new ACL(ACL.Access.None).ToJsonString(),
                    tr.ApiSuccess,
                    tr.ApiError
                );
            Console.WriteLine("COMPRESSED REQUEST PACKET SIZE: " + MimicPacketSize(true, entityValueFromFile));
            tr.Run();
            if (tr.m_done)
            {
                ReadResponsePacketSize(tr.m_response, true);
            }
            DeleteAllDefaultEntities();
        }

        private void ReadResponsePacketSize(Dictionary<string,object> response, bool isCompressed)
        {
            string jsonResponseString = JsonWriter.Serialize(response);
            string beginningText = isCompressed ? "DECOMPRESSED " : "UNCOMPRESSED ";
            byte[] responseByteArray = Encoding.UTF8.GetBytes(jsonResponseString);
            Console.WriteLine(beginningText + "RESPONSE PACKET SIZE :" + responseByteArray.Length);
        }
        
        private int MimicPacketSize(bool isCompressed, string entityValue)
        {
            //mimic data for a creating entity
            var data = new Dictionary<string, object>();
            data["entityType"] = _defaultEntityType;
            var jsonEntityData = Helpers.CreateJsonPair(_defaultEntityValueName, entityValue);
            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data["data"] = entityData;
            var jsonEntityAcl = new ACL(ACL.Access.None).ToJsonString(); 
            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data["acl"] = acl;
            }
            
            string jsonRequestString = JsonWriter.Serialize(data);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonRequestString);
            if (isCompressed)
            {
                var outputStream = new MemoryStream();
                using (var stream = new GZipStream(outputStream, CompressionMode.Compress, true))
                {
                    stream.Write(byteArray, 0, byteArray.Length);
                }
                byteArray = outputStream.ToArray();
            }
            
            return byteArray.Length;
        }
        
        /// <summary>
        /// Deletes all default entities
        /// </summary>
        private void DeleteAllDefaultEntities()
        {
            TestResult tr = new TestResult(_bc);

            List<string> entityIds = new List<string>(0);

            //get all entities
            _bc.EntityService.GetEntitiesByType(_defaultEntityType, tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                Dictionary<string, object> data = (Dictionary<string, object>)tr.m_response["data"];
                object[] temp = (object[])data["entities"];

                if (temp.Length <= 0) return;

                Dictionary<string, object>[] entities = (Dictionary<string, object>[])data["entities"];

                for (int i = 0; i < entities.Length; i++)
                {
                    entityIds.Add((string)entities[i]["entityId"]);
                }
            }

            for (int i = 0; i < entityIds.Count; i++)
            {
                tr.Reset();
                _bc.EntityService.DeleteEntity(entityIds[i], -1, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }
        }
    }
}
