using DataSources;
using DataSources.Models;
using DBLayer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Downloader
{
    public class Downloader<T> : IJob
        where T: IDownloadable 
    {

        public List<T> Sources { get; set; }

        public Downloader()
        {
            //this.httpClient = httpClient;
        }

       

        public async Task Execute(IJobExecutionContext context)
        {
            var httpClient = new HttpClient();

            var a = new DBLayerContext();

            var sss = a.Sources.Include(x => x.Field).ToList();
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

                sources.Add(s.DownloadAsync(httpClient,b));
               /* var resS = await s.DownloadAsync(httpClient, new HashSet<string>());*/

            }

            while(sources.Any())
            {
                var res = await Task.WhenAny(sources);
                sources.Remove(res);
                var resS = await res;
                var listTask = new List<Task>();
                foreach (var p in resS)
                {
                    
                    listTask.Add(a.AddPublication(p));
                    Debug.Print($"{p.Date}\n{p.Text}\n{p.URL}");
                    Debug.Print($"-------------{ind}");
                    ind++;
                }

                while (listTask.Any())
                {
                    var cur = await Task.WhenAny(listTask);
                    listTask.Remove(cur);
                }
            }
            a.SaveChanges();
            /*var httpClient = new HttpClient();
            var db = new DBLayerContext();
            var soueces = await db.Sources.Include(x => x.Field).ToListAsync();*/

            /*var fields = new List<Task<List<Publication>>>();

            foreach(var s in soueces)
            {
                IDownloadable downloader;
                switch (s.SourceType)
                {
                    case DataSources.Models.Sources.VK:
                        downloader = JsonConvert.DeserializeObject<VKAPI>(s.Field.Properties);
                        break;
                    case DataSources.Models.Sources.Insta:
                        downloader = JsonConvert.DeserializeObject<InstaAPI>(s.Field.Properties);
                        break;
                    default:
                        downloader = JsonConvert.DeserializeObject<Website>(s.Field.Properties);
                        break;
                }

                var a = downloader.DownloadAsync(httpClient, s.Publications.Select(p => p.URL).ToHashSet());
                fields.Add(a);
            }*/

            /*var ind = 1;
            foreach (var b in soueces)
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

                var resS = await s.DownloadAsync(httpClient, new HashSet<string>());

                var listTask = new List<Task>();
                foreach (var p in resS)
                {
                    //listTask.Add(db.AddPublication(new Publication { Text = p.Text, URL = p.URL, Source = b, Date = p.Date }));
                    Console.WriteLine($"{p.Date}\n{p.Text}\n{p.URL}");
                    Console.WriteLine($"-------------{ind}");
                    ind++;
                }*/

            /*while (listTask.Any())
            {
                var cur = await Task.WhenAny(listTask);
                listTask.Remove(cur);
            }

            db.SaveChanges();*/



            /*var field = soueces.Select
                (
                    x =>
                    {
                        IDownloadable res;
                        switch (x.SourceType)
                        {
                            case DataSources.Models.Sources.VK:
                                res = JsonConvert.DeserializeObject<VKAPI>(x.Field.Properties);
                                break;
                            case DataSources.Models.Sources.Insta:
                                res = JsonConvert.DeserializeObject<InstaAPI>(x.Field.Properties);
                                break;
                            default:
                                res = JsonConvert.DeserializeObject<Website>(x.Field.Properties);
                                break;
                        }
                        return res;
                    }
                ).Select(x => x.DownloadAsync(httpClient,new HashSet<string>())).ToList();*/

            /*var ind = 1;
            while (fields.Any())
            {
                var source = await Task.WhenAny(fields);
                fields.Remove(source);
                var s = await source;
                foreach (var p in s)
                {
                    Debug.Print($"{p.Date}\n{p.Text}\n{p.URL}");
                    Debug.Print($"-------------{ind}");
                    ind++;
                }
            }*/
        }
    }
}
