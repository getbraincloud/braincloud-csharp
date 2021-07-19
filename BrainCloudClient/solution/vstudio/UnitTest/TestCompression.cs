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
        //Value is huge to get accurate compression testing
        private readonly string _defaultEntityValue = 
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce malesuada aliquet orci, vel ornare eros mollis non. Sed elementum dictum sapien, sit amet tincidunt diam hendrerit ac. Sed placerat ante auctor ex tempus, sit amet aliquet ipsum iaculis. Pellentesque eu turpis dapibus, venenatis leo et, luctus metus. Sed sodales ante eu felis bibendum ornare. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Sed tempor in ipsum et sodales. Ut posuere nibh aliquam nunc laoreet, a tincidunt mi dignissim. Phasellus volutpat diam bibendum, hendrerit dolor a, imperdiet nulla. Fusce malesuada leo sed est laoreet mattis. Mauris efficitur pretium augue non mollis. Quisque nec metus diam.
            Proin finibus sem non bibendum lacinia. In sodales id augue at tempus. Duis pulvinar vestibulum urna, vel placerat diam molestie eget. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Sed risus nisl, rutrum et eros condimentum, tempor facilisis est. Nulla a tempor massa. Phasellus luctus maximus finibus. Ut nec libero at nunc semper eleifend ac mollis velit.
            Aliquam scelerisque sem urna, ut pretium tortor ultricies sed. Proin at vestibulum nunc. Sed vitae pellentesque est, vitae faucibus mi. Vivamus quis dolor vitae eros fringilla pharetra. Suspendisse pharetra at mi nec imperdiet. Maecenas tincidunt quam nec lectus volutpat, vitae maximus turpis condimentum. In hac habitasse platea dictumst.
            Cras dapibus rutrum massa a dignissim. Etiam vulputate tincidunt venenatis. Maecenas turpis neque, molestie et nisl non, aliquam suscipit diam. Aenean non sem felis. Quisque eget elementum magna. Nulla neque tortor, vestibulum quis nisl sit amet, fermentum varius dolor. Praesent sit amet nunc eget neque volutpat ultricies quis eu tortor. Mauris ut consectetur odio.
            Fusce fermentum laoreet elit eu scelerisque. Nullam et orci sagittis, vehicula purus ut, euismod purus. Ut tincidunt suscipit nunc, a fermentum felis rhoncus ac. Proin at odio pharetra, tempor nisl non, posuere ante. Pellentesque malesuada gravida purus id porta. Nam tincidunt eget eros ultricies pretium. Phasellus condimentum, lectus a porttitor interdum, dolor nisl laoreet justo, et semper urna justo sit amet lorem. Nulla facilisi. Suspendisse eget nulla tristique, ullamcorper mauris quis, consequat urna. Donec bibendum laoreet diam, in vehicula elit pharetra vitae.";
        
        [Test]
        public void TestCompressEntity()
        {
            TestResult tr = new TestResult(_bc);
            //No compression
            _bc.EntityService.CreateEntity
            (
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                new ACL(ACL.Access.None).ToJsonString(),
                tr.ApiSuccess,
                tr.ApiError
            );
            Console.WriteLine("UNCOMPRESSED REQUEST PACKET SIZE: " + MimicPacketSize(false));
            tr.Run();
            if (tr.m_done)
            {
                ReadResponsePacketSize(tr.m_response, false);
            }
            
            //Compression
            _bc.Client.AuthenticationService.AuthenticateUniversal
            (
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess,
                tr.ApiError,
                null,
                true
            );
            _bc.Client.EnableCompression(true);
            _bc.EntityService.CreateEntity
                (
                    _defaultEntityType,
                    Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                    new ACL(ACL.Access.None).ToJsonString(),
                    tr.ApiSuccess,
                    tr.ApiError
                );
            Console.WriteLine("COMPRESSED REQUEST PACKET SIZE: " + MimicPacketSize(true));
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
        
        private int MimicPacketSize(bool isCompressed)
        {
            //mimic data for a creating entity
            var data = new Dictionary<string, object>();
            data["entityType"] = _defaultEntityType;
            var jsonEntityData = Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue);
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
