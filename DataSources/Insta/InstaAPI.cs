using DataSources.Models;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataSources
{
    public class InstaAPI:IDownloadable
    {
        private readonly string pass;
        private readonly string login;
        public List<string> Tags { get; set; }
        public InstaAPI(string login,string password)
        {
            this.login = login;
            pass = password;
        }

        public IInstaApi InstaApi { get; private set; }

        public async Task<List<Publication>> DownloadAsync(HttpClient httpClient, DataSource source)
        {
            
            var result = new List<Publication>();
            try
            {
                var login = await Login();
                if (login)
                {
                    var curPosts = new List<Task<IResult<InstaTagFeed>>>();
                    foreach (var tag in Tags)
                    {
                        curPosts.Add(GetPostsByTag(tag, 5));
                    }

                    while (curPosts.Any())
                    {
                        var tagPosts = await Task.WhenAny(curPosts);
                        curPosts.Remove(tagPosts);
                        var posts = await tagPosts;
                        foreach (var p in posts.Value.Medias)
                        {
                            if (p.Caption != null)
                            {
                                result.Add
                                    (
                                        new Publication
                                        {
                                            Text = p.Caption.Text,
                                            Date = p.Caption.CreatedAt,
                                            Geotag = p.Location == null ? string.Empty : $"City: {p.Location.City}\nAddress: {p.Location.Address}",
                                            URL = $"https://www.instagram.com/p/{p.Code}/",
                                            Source = source
                                        }
                                    );
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Insta: Login error");
                }
                return result;
            }
            catch(Exception e)
            {
                return result;
            }
        }

        public Task<IResult<InstaTagFeed>> GetPostsByTag(string tag, int maxPageCount)
        {
            var inf = InstaApi.GetTagFeedAsync(tag, PaginationParameters.MaxPagesToLoad(maxPageCount));
            
            //var posts = inf.Value.Medias.ToList().GetRange(0, postCount);
            //return posts;
            return inf;
        }

        public async Task<bool> Login()
        {
            var result = false;
            InstaApi = InstaApiBuilder.CreateBuilder().
                SetUser(new UserSessionData() { UserName = login, Password = pass }).
                UseLogger(new DebugLogger(LogLevel.Exceptions)).
                Build();

            var logReq = await InstaApi.LoginAsync();

            if (logReq.Succeeded)
            {
                Console.WriteLine("Log in");
                result = true;
            }
            else
                Console.WriteLine("Log error\n" + logReq.Info.Message);
            return result;
        }

        public DateTime GetTime(string time)
        {
            throw new NotImplementedException();
        }
    }
}
