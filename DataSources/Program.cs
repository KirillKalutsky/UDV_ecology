using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataSources
{
    public class Program
    {
        public static async Task Main()
        {
            var a = new HttpClient();
            var page = await a.GetAsync("https://1prime.ru/News/");
            var doc = new HtmlDocument();
            doc.LoadHtml(await page.Content.ReadAsStringAsync());
            Regex reg = new Regex("[^0-9А-Яа-я: .]");
            var links= doc.DocumentNode.SelectNodes(@".//p[@class='rubric-list__article-announce']/a");
            Console.WriteLine(links.Count);
        }
    }
}
