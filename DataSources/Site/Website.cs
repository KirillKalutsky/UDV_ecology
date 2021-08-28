using DataSources.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataSources
{
    public class Website : IDownloadable
    {
        public string DomainForNext { get; set; }

        public string DomainForStart { get; set; }
        //public int NewsCount { get; set; }
        public string URL { get; set; }

        public HtmlElement StartElementFirstPage { get; set; }

        public HtmlElement NextElementFirstPage { get; set; }
        
        public HtmlElement StartElementChildPage { get; set; }

        public HtmlElement NextElementChildPage { get; set; }

        public HtmlElement ContentElement { get; set; }
        public HtmlElement DateElement { get; set; }

        private readonly Regex reg = new Regex("[^0-9А-Яа-я: .]");

        public async Task<List<Publication>> DownloadAsync(HttpClient httpClient, DataSource source)
        {
            return await GetNews(httpClient, URL, new List<Publication>(), StartElementFirstPage, NextElementFirstPage, source);
        }

        private async Task<List<Publication>> GetNews(HttpClient httpClient, string url, List<Publication> result, HtmlElement element, HtmlElement next, DataSource source)
        {
            HttpResponseMessage body;
            try
            {
                body = await httpClient.GetAsync(url);
            }
            catch (Exception e)
            {
                return result;
            }

            if (body.IsSuccessStatusCode)
            {
                var flag = true;

                var doc = new HtmlDocument();
                doc.LoadHtml(await body.Content.ReadAsStringAsync());

                var links = doc.DocumentNode.SelectNodes(element.XPath).ToList();

                var publications = new List<Task<Publication>>();

                foreach (var link in links)
                {
                    publications.Add(GetTextFromPage(httpClient, link.GetAttributeValue(element.AttributeName, ""), ContentElement, source));
                }

                while (publications.Any())
                {
                    var publication = await Task.WhenAny(publications);
                    publications.Remove(publication);
                    var p = await publication;
                    if (p.Date < DateTime.Now.AddDays(-2))
                        flag = false;
                    result.Add(p);
                }

                if(flag)
                    result = await GetNews
                        (
                            httpClient,
                            DomainForNext + doc.DocumentNode.SelectSingleNode(next.XPath).GetAttributeValue(next.AttributeName, ""),
                            result,
                            StartElementChildPage,
                            NextElementChildPage,
                            source
                        );
            }
            return result;
            
        }

        //Сайти рионовостей
        public async Task<Publication> GetTextFromPage(HttpClient httpClient, string link, HtmlElement htmlElement, DataSource source)
        {
            var publication = new Publication();
            var text = new StringBuilder();
            link = DomainForStart + link;
            //Console.WriteLine(link);
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
                var nodes = doc.DocumentNode.SelectNodes(htmlElement.XPath);
                if (nodes != null)
                {
                    foreach (var element in nodes)
                        text.Append(element.InnerText);

                    var time = doc.DocumentNode.SelectSingleNode(DateElement.XPath).InnerText.Split(",")[0];
                    //Console.WriteLine(time);

                    publication.Date = GetTime(time);
                    publication.Text = text.ToString();
                    publication.URL = link;
                    publication.Source = source;
                }
            }
            else
            {
                Console.WriteLine($"eror page load: {link} {page.StatusCode}");
            }
            return publication;
        }

        public DateTime GetTime(string time)
        {
            var d = reg.Replace(time, " ");
            DateTime date;
            try
            {
                date = DateTime.Parse(d);
            }
            catch(FormatException e)
            {
                Debug.Print(e.HelpLink);
                date = DateTime.Now;
            }
            return date;
        }
    }
}
