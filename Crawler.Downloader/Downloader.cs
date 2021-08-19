using DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Downloader
{
    public class Downloader<T> where T: IDownloadable 
    {

        public List<T> Sources { get; set; }

        public async Task StartDownload()
        {
            var curSources = Sources.Select(x => x.DownloadAsync()).ToList();
            var ind = 1;
            while (curSources.Any())
            {
                var source = await Task.WhenAny(curSources);
                curSources.Remove(source);
                var s = await source;
                foreach (var p in s)
                {
                    Console.WriteLine(p);
                    Console.WriteLine($"-------------{ind}");
                    ind++;
                }
            }
        }
    }
}
