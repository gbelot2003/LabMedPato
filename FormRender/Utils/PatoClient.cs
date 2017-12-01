using FormRender.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace FormRender.Utils
{
    public static class PatoClient
    {
        private const string API = "http://192.168.2.101/histoData/";
        public static InformeResponse GetResponse(int id)
        {
            var request = WebRequest.Create(API + id.ToString());
            request.Method = "GET";
            var k = request.GetResponse();
            var l = k.GetResponseStream();
            var m = new StreamReader(l);
            JsonSerializer n = new JsonSerializer();
            return n.Deserialize<InformeResponse>(new JsonTextReader(m));
        }
    }
}