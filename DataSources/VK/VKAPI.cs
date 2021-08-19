using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataSources
{
    public class VKAPI : IDownloadable
    {
        readonly string token = "07c4a15607c4a15607c4a156c107bc638c007c407c4a15666c63002c593e62fc9c5e1c2";
        public List<string> SourceGroup { get; set; }
        public HttpClient client;
        Regex onlyText = new Regex("[^\\w0-9,.?:;! ]");

        public  Task<List<string>> DownloadAsync()
        {
            return ScripeVKPosts(SourceGroup,10);
        }
        

        public Task<string> DoMethod(string method, string par, string v = "5.131")
        {
            var req = $"https://api.vk.com/method/{method}?{par}&access_token={token}&v={v}";

            return client.GetStringAsync(req);
        }

        private  async Task<List<string>> ScripeVKPosts(List<string> groups, int postCount)
        {
            var result = new List<string>();
            foreach (var g in groups)
            {
                var count = 0;
                var curGroupPost = new List<Task<string>>();
                while (count < postCount)
                {
                    var posts = DoMethod("wall.get", $"domain={g}&count=100&offset={count}");
                    curGroupPost.Add(posts);
                    count += 100;
                }
                while (curGroupPost.Any())
                {
                    var posts = await Task.WhenAny(curGroupPost);
                    curGroupPost.Remove(posts);

                    var res = PrettyJson(await posts);
                    foreach (var line in GetMaintDataOfPost(res))
                        result.Add(line);
                }
            }
            return result; 
        }

        public string PrettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(unPrettyJson);
            return System.Text.Json.JsonSerializer.Serialize(jsonElement, options);
        }

        private List<string> GetMaintDataOfPost(string json)
        {
            //классы Response и Post нужны для удобной работы с JSON
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, Response>>(json);
            if (parsed == null) return null;
            var result = new List<string>();
            //там только один ключ - "response"
            foreach (var item in parsed["response"].items)
            {
                var text = onlyText.Replace(item.text, " ");
                text = Regex.Replace(text, @"\s+", " ");
                if (!(String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text) || String.Empty.CompareTo(text) == 0))
                    result.Add($"{text}\t{UnixTimeToDateTime(long.Parse(item.date))}");
                //builder.AppendLine(UnixTimeToDateTime(long.Parse(item.date)).ToString());
            }
            return result;
        }

        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }
}
