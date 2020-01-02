using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace Console.App
{
    class Program
    {
        static readonly string Broe2API = "https://webapi.britishrowing.org/api/";
        static readonly string ApiKey = "c7ac8d8f-3ce2-48eb-a88c-05c98173e3d8";
        static readonly string AuthKey = "1032316e-2fb7-477f-a2cd-d61b74455b75";
        static readonly string MeetingKey = "db340ff9c571440a91241ec017f7b3d9";

        static void Main(string[] args)
        {
            GetClubs();
            GetEntries();
        }

        static void GetClubs()
        {
            string json = BROE2API_GetInfo("OE2ClubInformation");

            if (!string.IsNullOrEmpty(json))
            {

                List<Club> aList = JsonConvert.DeserializeObject<List<Club>>(json);

                foreach (Club aClub in aList)
                {
                    Debug.Print(aClub.Name);
                }
            }

        }

        static void GetEntries()
        {
            string json = BROE2API_GetInfo("OE2CrewInformation");

            if (!string.IsNullOrEmpty(json))
            {
                XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "json");

                foreach (XmlNode aNode in doc.SelectNodes("json/crews/name"))
                    Debug.Print(aNode.InnerText);
            }
        }

        static string BROE2API_GetInfo(string sEndpoint)
        {
            WebResponse response = null;

            Stream dataStream;
            WebRequest request;
            IDictionary<string, string> aHeaders = new Dictionary<string, string>();
            string sHeaders;

            try
            {

                aHeaders.Add("api_key", ApiKey);
                aHeaders.Add("meetingIdentifier", MeetingKey);

                sHeaders = JsonConvert.SerializeObject(aHeaders);

                request = WebRequest.Create(Broe2API + sEndpoint);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", AuthKey);

                request.GetRequestStream().Write(Encoding.UTF8.GetBytes(sHeaders), 0, Encoding.UTF8.GetBytes(sHeaders).Count());

                response = request.GetResponse();

                dataStream = response.GetResponseStream();
                string json;
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    json = reader.ReadToEnd();
                }

                return json.ToString();
            }
            catch (WebException ex)
            {
                Debug.Print(ex.ToString());
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }

            return null;

        }
    }
    class Club {
        public string Name;
        public string abbreviation;
        public string IndexCode;
        public string entriesSecretaryName;
        public string entriesSecretaryEmail;
        public string entriesSecretaryPhone;
        public string entriesSecretaryMobile;
        public string clubSecretaryName;
        public string clubSecretaryEmail;
        public string clubSecretaryPhone;
        public string clubSecretaryMobile;
        public string colours;
        public string blades;
        public string bladeImage;
        public object additionalInformationAnswers;
    }
    
}
