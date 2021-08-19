using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Crawler.Downloader;
using DataSources;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;

namespace Crawler
{

    public static class Program
    {
        public static List<IDownloadable> list = new List<IDownloadable>();
        public static readonly HttpClient HttpClient = new HttpClient();
        public static async Task Main(string[] args)
        {
            

            var downloader = new Downloader<IDownloadable>
            {
                Sources = new List<IDownloadable>
                {
                    /*new InstaAPI("double_k_v","abrakadabra77"),
                    new VKAPI{SourceGroup = new List<string> { "ecointegral"},client = HttpClient},*/
                    new Website
                    {
                        httpClient = HttpClient,
                        Domain = "https://ria.ru/",
                        URL = "https://ria.ru/incidents/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='list-item__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//a[@class='list-item__image']",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 5,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"},
                        DateElement = new HtmlElement{XPath=@".//div[@class='article__info-date']/a"}
                    },
                    new Website
                    {
                        httpClient = HttpClient,
                        Domain = "",
                        URL = "https://sakhalin.info/ecology",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='story-title-link']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']"},
                        StartElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='story-title-link']"},
                        NextElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']"},
                        NewsCount = 5,
                        ContentElement = new HtmlElement{XPath = @".//p[@class='text-style-text']"},
                        DateElement = new HtmlElement{XPath=@".//div[@class='article-date']"}
                    },
                   /* new Website
                    {
                        httpClient = HttpClient,
                        Domain = "https://ria.ru/",
                        URL = "https://ria.ru/incidents/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='list-item__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//a[@class='list-item__image']",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 10,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"}
                    },
                    new Website
                    {
                        httpClient = HttpClient,
                        Domain = "https://ria.ru/",
                        URL = "https://ria.ru/incidents/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='list-item__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//a[@class='list-item__image']",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 10,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"}
                    },
                    new Website
                    {
                        httpClient = HttpClient,
                        Domain = "https://ria.ru/",
                        URL = "https://ria.ru/incidents/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='list-item__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//a[@class='list-item__image']",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 10,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"}
                    },
                    new Website
                    {
                        httpClient = HttpClient,
                        Domain = "https://ria.ru/",
                        URL = "https://ria.ru/incidents/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='list-item__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//a[@class='list-item__image']",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 10,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"}
                    },*/
                    /*new VKAPI{SourceGroup = new List<string> { "ecointegral", "russiauncensored", "botkin_news", "news.region", "otrussia", "world_weather", "news72ru", "mchs__russia" }},
                    new InstaAPI("double_k_v","abrakadabra77"),
                    new Website
                    {
                        httpClient = HttpClient,
                        Domain = "https://ria.ru/",
                        URL = "https://ria.ru/incidents/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='list-item__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//a[@class='list-item__image']",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 10,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"}
                    },*/
                }
            };

            await downloader.StartDownload();
            

            /*var instClient = new InstaAPI();
            var flag = await instClient.Login(SharedData.instLogin, SharedData.instPass);
            Console.WriteLine(flag);
            if (!flag)
            {
                var fireOne = instClient.GetPostsByTag("пожар", 1);
                var fireFive = instClient.GetPostsByTag("пожар", 5);

                var list = new List<Task<IResult<InstaTagFeed>>> { fireFive, fireOne };
                while (list.Count > 0)
                {
                    var curTask = await Task.WhenAny(list);
                    list.Remove(curTask);
                    var curTaskParam = curTask.Result.Value.Medias.ToList().GetRange(0, 5);
                    foreach (var p in curTaskParam)
                    {
                        Console.WriteLine($"{p.Location.Address}");
                        Console.WriteLine($"{p.Location.City}");
                        Console.WriteLine($"{p.Location.ExternalId}");
                        Console.WriteLine($"{p.Location.ExternalSource}");
                        Console.WriteLine($"{p.Location.FacebookPlacesId}");
                        Console.WriteLine($"{p.Location.Lat}");
                        Console.WriteLine($"{p.Location.Lng}");
                        Console.WriteLine($"{p.Location.Name}");
                        Console.WriteLine($"{p.Location.Pk}");
                        Console.WriteLine($"{p.Location.ShortName}");
                    }
                }
            }*/
        }
    }
}
