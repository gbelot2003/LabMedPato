using FormRender.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static FormRender.Config;

namespace FormRender.Utils
{
    public static class PatoClient
    {
        public static async Task<InformeResponse> GetResponse(int id, int fact, string user, string password, string exRt = null)
        {
            var request = WebRequest.Create(API + exRt);
            request.Method = "POST";
            var pd = new StringBuilder();
            pd.Append($"serial={id}&");
            pd.Append($"factura={fact}&");
            pd.Append($"username={user}&");
            pd.Append($"password={password}");
            byte[] pb = Encoding.ASCII.GetBytes(pd.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = pb.Length;
            var postStream = await request.GetRequestStreamAsync();
            await postStream.WriteAsync(pb, 0, pb.Length);
            await postStream.FlushAsync();
            postStream.Close();
            var k = await request.GetResponseAsync();
            var l = k.GetResponseStream();
            var m = new StreamReader(l);
            JsonSerializer n = new JsonSerializer();
            return n.Deserialize<InformeResponse>(new JsonTextReader(m));
        }
        public static async Task<bool> Login(string username, string password)
        {
            var request = WebRequest.Create(usrPath);
            request.Method = "POST";
            var pd = new StringBuilder();
            pd.Append($"username={username}&");
            pd.Append($"password={password}");
            byte[] pb = Encoding.ASCII.GetBytes(pd.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = pb.Length;
            var postStream = await request.GetRequestStreamAsync();
            await postStream.WriteAsync(pb, 0, pb.Length);
            await postStream.FlushAsync();
            postStream.Close();
            var k = await request.GetResponseAsync();
            var l = k.GetResponseStream();
            var m = new StreamReader(l);
            return await m.ReadToEndAsync() == "ok";
        }
    }
}