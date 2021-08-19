using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataSources
{
    public class Website : IDownloadable
    {
        public string Domain { get; set; }

        public int NewsCount { get; set; }
        public HttpClient httpClient { get; set; }
        public string URL { get; set; }

        public HtmlElement StartElementFirstPage { get; set; }

        public HtmlElement NextElementFirstPage { get; set; }
        
        public HtmlElement StartElementChildPage { get; set; }

        public HtmlElement NextElementChildPage { get; set; }

        public HtmlElement ContentElement { get; set; }
        public HtmlElement DateElement { get; set; }

        public async Task<List<string>> DownloadAsync()
        {
            var result = new List<string>();
            var linksP = new List<Task<string>>();
            var links = await GetNewsFromRIA(URL,NewsCount,new List<string>(),StartElementFirstPage,NextElementFirstPage);
            foreach(var link in links)
            {
                linksP.Add(GetTextFromPage(link, ContentElement));
            }
            while (linksP.Any())
            {
                var l = await Task.WhenAny(linksP);
                linksP.Remove(l);
                result.Add(await l);
            }
            return result;
        }

        

        private async Task<List<string>> GetNewsFromRIA(string url, int countNews, List<string> result, HtmlElement element, HtmlElement next)
        {
            var body = await httpClient.GetAsync(url);
            if (body.IsSuccessStatusCode)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(await body.Content.ReadAsStringAsync());
                var links = doc.DocumentNode.SelectNodes(element.XPath).ToList();
                foreach (var link in links)
                {
                    if (result.Count >= countNews)
                        return result;
                    result.Add(link.GetAttributeValue(element.AttributeName, ""));
                }

                result = await GetNewsFromRIA
                    (
                        Domain + doc.DocumentNode.SelectSingleNode(next.XPath).GetAttributeValue(next.AttributeName, ""),
                        countNews,
                        result,
                        StartElementChildPage,
                        NextElementChildPage
                        /*new HtmlElement
                        {
                            XPath = @".//a[@class='list-item__image']",
                            AttributeName = "href"
                        },*/
                        /*new HtmlElement
                        {
                            XPath = @".//div[@class='list-items-loaded']",
                            AttributeName = "data-next-url"
                        }*/
                    );
            }
            return result;
        }

        //Сайти рионовостей
        public async Task<string> GetTextFromPage(string link, HtmlElement htmlElement)
        {
            var text = new StringBuilder();
            var page = await httpClient.GetAsync(link);
            var doc = new HtmlDocument();

            while (!page.IsSuccessStatusCode)
            {
                //await Task.Delay(1);
                page = await httpClient.GetAsync(link);
            }
            if (page.IsSuccessStatusCode)
            {
                doc.LoadHtml(await page.Content.ReadAsStringAsync());
                foreach (var element in doc.DocumentNode.SelectNodes(htmlElement.XPath))
                    text.Append(element.InnerText);
                text.Append("\n"+doc.DocumentNode.SelectSingleNode(DateElement.XPath).InnerText);
            }
            else
            {
                Console.WriteLine($"eror page load: {link} {page.StatusCode}");
            }
            return text.ToString();
        }
    }
}
