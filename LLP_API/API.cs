using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Generic;

namespace LLP_API
{
    public class ServerAPI
    {
        private string ServerURL = string.Empty;
        private string userId = string.Empty;
        private string userToken = string.Empty;

        public ServerAPI(string URL, string UserId,string UserToken)
        {
            ServerURL = URL;
            this.userId = UserId;
            this.userToken = UserToken;
        }

        public string AddBeaconInformations(List<BeaconInformation> Value)
        {
            string JsonString = JsonConvert.SerializeObject(new {
                Table = "BeaconInformation",
                Action = "Add",
                Data = JsonConvert.SerializeObject(Value),
                UserId = userId,
                UserToken = userToken
            });
            return PostDataToServer(JsonString);
        }

        public string UpdateBeaconInformations(List<BeaconInformation> Value)
        {
            string JsonString = JsonConvert.SerializeObject(new
            {
                Table = "BeaconInformation",
                Action = "Update",
                Data = JsonConvert.SerializeObject(Value),
                UserId = userId,
                UserToken = userToken
            });
            return PostDataToServer(JsonString);
        }

        public string DeleteBeaconInformations(List<BeaconInformation> Value)
        {
            string JsonString = JsonConvert.SerializeObject(new
            {
                Table = "BeaconInformation",
                Action = "Delete",
                Data = JsonConvert.SerializeObject(Value),
                UserId = userId,
                UserToken = userToken
            });
            return PostDataToServer(JsonString);
        }

        public string AddLaserPointerInformations(List<LaserPointerInformation> Value)
        {
            string JsonString = JsonConvert.SerializeObject(new
            {
                Table = "LaserPointerInformation",
                Action = "Add",
                Data = JsonConvert.SerializeObject(Value),
                UserId = userId,
                UserToken = userToken
            });
            return PostDataToServer(JsonString);
        }

        public string UpdateLaserPointerInformations(List<LaserPointerInformation> Value)
        {
            string JsonString = JsonConvert.SerializeObject(new
            {
                Table = "LaserPointerInformation",
                Action = "Update",
                Data = JsonConvert.SerializeObject(Value),
                UserId = userId,
                UserToken = userToken
            });
            return PostDataToServer(JsonString);
        }

        public string DeleteLaserPointerInformations(List<LaserPointerInformation> Value)
        {
            string JsonString = JsonConvert.SerializeObject(new
            {
                Table = "LaserPointerInformation",
                Action = "Delete",
                Data = JsonConvert.SerializeObject(Value),
                UserId = userId,
                UserToken = userToken
            });
            return PostDataToServer(JsonString);
        }

        private string PostDataToServer(string Data)
        {
            //跳過SSL檢查
            ServicePointManager.ServerCertificateValidationCallback
                = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            try
            {
                HttpWebRequest request = WebRequest.Create(ServerURL) as HttpWebRequest;
                request.Method = WebRequestMethods.Http.Post;
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                string param = "=" + Data;
                byte[] bs = Encoding.UTF8.GetBytes(param);
                request.ContentLength = bs.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Flush();
                }

                string retMsg = string.Empty; ;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Server回傳的資料。
                        Stream data = response.GetResponseStream();
                        StreamReader sr = new StreamReader(data);
                        retMsg = sr.ReadToEnd();
                        sr.Dispose();
                        data.Dispose();
                    }
                }
                return retMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public (List<BeaconInformation>, List<LaserPointerInformation>) GetDataFromServer()
        {
            //跳過SSL檢查
            ServicePointManager.ServerCertificateValidationCallback 
                = new RemoteCertificateValidationCallback(ValidateServerCertificate);

            List<BeaconInformation> Beacons = null;
            List<LaserPointerInformation> LaserPointers = null;

            HttpWebRequest request = WebRequest.Create(ServerURL) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var temp = reader.ReadToEnd();

                        //TODO:反序列化
                        dynamic JsonData = JsonConvert.DeserializeObject(temp);
                        Beacons = JsonConvert.DeserializeObject<List<BeaconInformation>>(JsonData["BeaconInformation"].ToString());
                        LaserPointers = JsonConvert.DeserializeObject<List<LaserPointerInformation>>(JsonData["LaserPointerInformation"].ToString());
                    }
                }
            }

            return (Beacons, LaserPointers);
        }

        //跳過SSL檢查
        private static bool ValidateServerCertificate(Object sender, 
            X509Certificate certificate, 
            X509Chain chain, 
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
