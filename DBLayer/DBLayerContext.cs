using DataSources;
using DataSources.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLayer
{
    public class DBLayerContext: DbContext
    {
        public DbSet<DataSource> Sources { get; set; }
        public DbSet<DataSourceField> SourcesFields { get; set; }
        public DbSet<Publication> Publications{ get; set; }
        public DBLayerContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataSource>()
           .HasOne(s => s.Field)
           .WithOne(f => f.Source)
           .HasForeignKey<DataSourceField>(x=>x.SourceId);

            modelBuilder.Entity<DataSource>()
            .HasMany(s => s.Publications)
            .WithOne(p => p.Source);
        }

        public async Task AddSource(DataSource dataSource)
        {
            await Sources.AddAsync(dataSource);
        }
        
        public async Task AddSourceField(DataSourceField field)
        {
            await SourcesFields.AddAsync(field);
        }
        
        public async Task AddPublication(Publication publication)
        {
            if(!Publications.Select(x=>x.URL).ToHashSet().Contains(publication.URL))
                await Publications.AddAsync(publication);
        }

        public async Task<DataSource> GetDataSourceById(int id)
        {
            var resList = await Sources.Include(x=>x.Publications).Include(x=>x.Field).ToListAsync();
            return resList.Where(x => x.Id == id).FirstOrDefault();
        }

        public async Task<Publication> GetPublicationById(int id)
        {
            var resList = await Publications.Include(x=>x.Source).ToListAsync();
            return resList.Where(x => x.Id == id).FirstOrDefault();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=udv_ecology;Username=postgres;Password=abrakadabra77");
        }
    }
    public static class DBInitializer
    {
        public static void Init(DBLayerContext context)
        {
            //context.Database.EnsureCreated();

            // Look for any students.
            if (context.Sources.Any())
            {
                return;   // DB has been seeded
            }


            var sources = new List<DataSource>
            {
               /* new DataSource{
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new InstaAPI("double_k_v", "abrakadabra77")
                            {
                                Tags = new List<string> { "fire" }
                            }
                        )
                    },
                    SourceType = Sources.Insta
                },*/

                new DataSource
                {
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new VKAPI
                            {
                                SourceGroup = new List<string> {  "russiauncensored", "botkin_news", "news.region", "otrussia", "world_weather", "news72ru", "mchs__russia" }
                            }
                        )
                    },
                    SourceType = Sources.VK
                },

                /*new DataSource{
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new Website()
                            {
                                DomainForNext = "https://ria.ru/",
                                URL = "https://ria.ru/incidents/",
                                StartElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='list-item__image']" },
                                NextElementFirstPage = new HtmlElement { AttributeName = "data-url", XPath = @".//div[@class='list-more']" },
                                StartElementChildPage = new HtmlElement { XPath = @".//a[@class='list-item__image']", AttributeName = "href" },
                                NextElementChildPage = new HtmlElement { XPath = @".//div[@class='list-items-loaded']", AttributeName = "data-next-url" },
                                //NewsCount = 200,
                                ContentElement = new HtmlElement { XPath = @".//div[@class='article__text']" },
                                DateElement = new HtmlElement { XPath = @".//div[@class='article__info-date']/a" }
                            }
                        )
                    },
                    SourceType = Sources.Site
                },
*/
                new DataSource
                {
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new Website()
                            {
                                URL = "https://sakhalin.info/ecology",
                                StartElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='story-title-link']" },
                                NextElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']" },
                                StartElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='story-title-link']" },
                                NextElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='pagination-more list-loader-status']" },
                                //NewsCount = 200,
                                ContentElement = new HtmlElement { XPath = @".//p[@class='text-style-text']" },
                                DateElement = new HtmlElement { XPath = @".//div[@class='article-date']" }
                            }
                        )
                    },
                    SourceType = Sources.Site
                },

                new DataSource
                {
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new Website()
                            {
                                DomainForNext = "https://vnru.ru",
                                URL = "https://vnru.ru/news.html?start=0",
                                StartElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='article-card__image']" },
                                NextElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='hasTooltip']" },
                                StartElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='article-card__image']" },
                                NextElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='hasTooltip']" },
                                //NewsCount = 200,
                                ContentElement = new HtmlElement { XPath = @".//div[@class='article-text']/p" },
                                DateElement = new HtmlElement { XPath = @".//div[@class='article__date uk-text-nowrap']" }
                            }
                        )
                    },
                    SourceType = Sources.Site
                },

                new DataSource
                {
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new Website()
                            {
                                DomainForStart = "https://1prime.ru",
                                DomainForNext = "https://1prime.ru",
                                URL = @"https://1prime.ru/News/",
                                StartElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//p[@class='rubric-list__article-announce']/a" },
                                NextElementFirstPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='button button_inline button_rounded button_more']" },
                                StartElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//p[@class='rubric-list__article-announce']/a" },
                                NextElementChildPage = new HtmlElement { AttributeName = "href", XPath = @".//a[@class='button button_inline button_rounded button_more']" },
                                //NewsCount = 5,
                                ContentElement = new HtmlElement { XPath = @".//div[@class='article-body__content']/p" },
                                DateElement = new HtmlElement { XPath = @".//time[@class='article-header__datetime']" }
                            }
                        )
                    },
                    SourceType = Sources.Site
                },

                new DataSource
                {
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new Website()
                            {
                                URL = "http://neftianka.ru/",
                                StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//h1[@class='entry-title']/a"},
                                NextElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//div[@class='nav-previous']/a"},
                                StartElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//h1[@class='entry-title']/a"},
                                NextElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//div[@class='nav-previous']/a"},
                                //NewsCount = 200,
                                ContentElement = new HtmlElement{XPath = @".//div[@class='entry-content']/p"},
                                DateElement = new HtmlElement{XPath=@".//span[@class='date']"}
                            }
                        )
                    },
                    SourceType = Sources.Site
                },

                /*new DataSource
                {
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new Website()
                            {
                                DomainForNext = "https://radiosputnik.ria.ru",
                                URL = "https://radiosputnik.ria.ru/russia/",
                                StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//div[@class='list-item__content']/a"},
                                NextElementFirstPage = new HtmlElement{ AttributeName = "data-url", XPath = @".//div[@class='list-more color-bg-hover']"},
                                StartElementChildPage= new HtmlElement{XPath = @".//div[@class='list-item__content']/a",AttributeName = "href"},
                                NextElementChildPage = new HtmlElement{XPath = @".//div[@class='list-items-loaded']",AttributeName = "data-next-url"},
                                //NewsCount = 200,
                                ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"},
                                DateElement = new HtmlElement{XPath=@".//div[@class='article__info-date']/a"}
                            }
                        )
                    },
                    SourceType = Sources.Site
                },*/
                
               

                new DataSource
                {
                    Field = new DataSourceField
                    {
                        Properties = JsonConvert.SerializeObject
                        (
                            new Website()
                            {
                                DomainForNext = "https://www.mk.ru",
                                URL = "https://www.mk.ru/incident/",
                                StartElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='listing-preview__content']"},
                                NextElementFirstPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='news-listing__arrow news-listing__arrow-right']"},
                                StartElementChildPage= new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='listing-preview__content']"},
                                NextElementChildPage = new HtmlElement{ AttributeName = "href", XPath = @".//a[@class='news-listing__arrow news-listing__arrow-right']"},
                                //NewsCount = 200,
                                ContentElement = new HtmlElement{XPath = @".//div[@class='article__text']"},
                                DateElement = new HtmlElement{XPath=@".//time[@class='meta__text']"}
                            }
                        )
                    },
                    SourceType = Sources.Site
                },
            };

            foreach (var s in sources)
                context.Sources.Add(s);

            context.SaveChanges();
           
        }
    }
}
