using DataSources.Models;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataSources
{
    public interface IDownloadable
    {
        Task<List<Publication>> DownloadAsync(HttpClient httpClient, DataSource source);

        DateTime GetTime(string time);
    }
}
