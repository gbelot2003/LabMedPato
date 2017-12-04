using FormRender.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using static FormRender.Misc;

namespace FormRender.Utils
{
    public static class PatoClient
    {
        public static InformeResponse GetResponse(int id, int fact, string user, string password)
        {
            var request = WebRequest.Create(API);
            request.Method = "POST";
            //request.Credentials = new NetworkCredential("gbelot", "Luna0102");
            StringBuilder pd = new StringBuilder();
            pd.Append($"serial={id}&");
            pd.Append($"factura={fact}&");
            pd.Append($"username={user}&");
            pd.Append($"password={password}");
            byte[] pb = Encoding.ASCII.GetBytes(pd.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = pb.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(pb, 0, pb.Length);
            postStream.Flush();
            postStream.Close();
            var k = request.GetResponse();
            var l = k.GetResponseStream();
            var m = new StreamReader(l);
            JsonSerializer n = new JsonSerializer();
            return n.Deserialize<InformeResponse>(new JsonTextReader(m));
        }
    }
}