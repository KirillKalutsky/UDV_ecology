using System;
using System.Net.Http;
using System.Threading.Tasks;
using DataSources;
using DataSources.Models;
using Newtonsoft.Json;

namespace DBLayer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var db = new DBLayerContext();

            var client = new HttpClient();

            var source = await db.GetDataSourceById(1);

            var p = source.Field.Properties;

            var webP = JsonConvert.DeserializeObject<Website>(p);

            Console.WriteLine(source.SourceType == Sources.Site);
            var fieldA = new DataSourceField
            {
                Properties = JsonConvert.SerializeObject
                (
                     new Website()
                     {
                         DomainForNext = "",
                         URL = "https://sakhalin.info/ecology",
                         StartElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='story-title-link']" },
                         NextElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']" },
                         StartElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='story-title-link']" },
                         NextElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']" },
                         //NewsCount = 5,
                         ContentElement = new HtmlElement { XPath = @".//p[@class='text-style-text']" },
                         DateElement = new HtmlElement { XPath = @".//div[@class='article-date']" }
                     }
                )
            };

            var a = db.AddSource
                (
                    new DataSource
                    {
                        Field = fieldA,
                        SourceType = Sources.Site
                    }
                );
            //db.Sources.Remove(await db.GetDataSourceById(1));
            foreach (var e in db.SourcesFields)
                Console.WriteLine(e);
            //db.SaveChanges();
        }
    }
}
