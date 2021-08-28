using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Crawler.Downloader;
using DataSources;
using DataSources.Models;
using DBLayer;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;

namespace Crawler
{

    public static class Program
    {
        public static List<IDownloadable> list = new List<IDownloadable>();
        public static readonly HttpClient HttpClient = new HttpClient();
        public static async Task Main(string[] args)
        {


            var source = new List<IDownloadable>
                {
                    new InstaAPI("double_k_v","abrakadabra77")
                    {
                        Tags = new List<string>{ "fire" }
                    },

                    new VKAPI{SourceGroup = new List<string> { "ria"}},

                    //какая-то шляпа с ответом сервера
                   /* new Website()
                    {
                        DomainForNext = "https://ria.ru",
                        URL = "https://ria.ru/incidents/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='list-item__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//a[@class='list-item__image']",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 100,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"},
                        DateElement = new HtmlElement{XPath=@".//div[@class='article__info-date']/a"}
                    },*/


                    new Website()
                    {

                        URL = "https://sakhalin.info/ecology",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='story-title-link']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']"},
                        StartElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='story-title-link']"},
                        NextElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']"},
                        //NewsCount = 100,
                        ContentElement = new HtmlElement{XPath = @".//p[@class='text-style-text']"},
                        DateElement = new HtmlElement{XPath = @".//div[@class='article-date']"}
                    },


                    new Website()
                    {
                        DomainForNext = "https://vnru.ru",
                        URL = "https://vnru.ru/news.html?start=0",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='article-card__image']"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='hasTooltip']"},
                        StartElementChildPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='article-card__image']"},
                        NextElementChildPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='hasTooltip']"},
                        //NewsCount = 100,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article-text']/p"},
                        DateElement = new HtmlElement{XPath=@".//div[@class='article__date uk-text-nowrap']"}
                    },


                    new Website()
                    {
                        DomainForStart ="https://1prime.ru",
                        DomainForNext = "https://1prime.ru",
                        URL = @"https://1prime.ru/News/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//p[@class='rubric-list__article-announce']/a"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='button button_inline button_rounded button_more']"},
                        StartElementChildPage=new HtmlElement{ AttributeName = "href", XPath = @".//p[@class='rubric-list__article-announce']/a"},
                        NextElementChildPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='button button_inline button_rounded button_more']"},
                        //NewsCount = 100,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article-body__content']/p"},
                        DateElement = new HtmlElement{XPath=@".//time[@class='article-header__datetime']"}
                    },

                    new Website()
                    {

                        URL = "http://neftianka.ru/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//h1[@class='entry-title']/a"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//div[@class='nav-previous']/a"},
                        StartElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//h1[@class='entry-title']/a"},
                        NextElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//div[@class='nav-previous']/a"},
                        //NewsCount = 100,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='entry-content']/p"},
                        DateElement = new HtmlElement{XPath=@".//span[@class='date']"}
                    },

                   /* new Website()
                    {
                        DomainForNext = "https://radiosputnik.ria.ru",
                        URL = "https://radiosputnik.ria.ru/russia/",
                        StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//div[@class='list-item__content']/a"},
                        NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more color-bg-hover']"},
                        StartElementChildPage= new HtmlElement{XPath = @".//div[@class='list-item__content']/a",AttributeName = "href"},
                        NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                        NewsCount = 100,
                        ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"},
                        DateElement = new HtmlElement{XPath=@".//div[@class='article__info-date']/a"}
                    }*/

                    /*new VKAPI{SourceGroup = new List<string> { "ecointegral", "russiauncensored", "botkin_news", "news.region", "otrussia", "world_weather", "news72ru", "mchs__russia" }},
                    new InstaAPI("double_k_v","abrakadabra77"),
                    */
            };


            var httpClient = new HttpClient();

            var a = new DBLayerContext();

            var sss = a.Sources.Include(x => x.Field).Where(x=>x.Id==1).ToList();
            var ind = 1;

            var sources = new List<Task<List<Publication>>>();

            foreach (var b in sss)
            {
                IDownloadable s;

                switch (b.SourceType)
                {
                    case DataSources.Models.Sources.VK:
                        s = JsonConvert.DeserializeObject<VKAPI>(b.Field.Properties);
                        break;
                    case DataSources.Models.Sources.Insta:
                        s = JsonConvert.DeserializeObject<InstaAPI>(b.Field.Properties);
                        break;
                    default:
                        s = JsonConvert.DeserializeObject<Website>(b.Field.Properties);
                        break;
                }

                sources.Add(s.DownloadAsync(httpClient, b));
                /* var resS = await s.DownloadAsync(httpClient, new HashSet<string>());*/

            }

            while (sources.Any())
            {
                var res = await Task.WhenAny(sources);
                sources.Remove(res);
                var resS = await res;
                var listTask = new List<Task>();
                foreach (var p in resS)
                {

                    listTask.Add(a.AddPublication(p));
                    Console.WriteLine($"{p.Date}\n{p.Text}\n{p.URL}");
                    Console.WriteLine($"-------------{ind}");
                    ind++;
                }

                while (listTask.Any())
                {
                    var cur = await Task.WhenAny(listTask);
                    listTask.Remove(cur);
                }

                a.SaveChanges();
            }
            /*var res = sources.Select(x => x.DownloadAsync(HttpClient)).ToList();

            var ind = 1;
            while (res.Any())
            {
                var source = await Task.WhenAny(res);
                res.Remove(source);
                var s = await source;
                foreach (var p in s)
                {
                    Console.WriteLine($"{p.Date}\n{p.Text}\n{p.URL}");
                    Console.WriteLine($"-------------{ind}");
                    ind++;
                }
            }*/

            //await downloader.Execute();


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
