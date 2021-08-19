using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
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
            this.pass = password;
            Tags = new List<string> {"пожар" };
        }

        public IInstaApi InstaApi { get; private set; }

        public async Task<List<string>> DownloadAsync()
        {
            var result = new List<string>();
            var login = await Login();
            if (login)
            {
                var curPosts = new List<Task<IResult<InstaTagFeed>>>();
                foreach(var tag in Tags)
                {
                    curPosts.Add(GetPostsByTag(tag, 1));
                }

                while (curPosts.Any())
                {
                    var tagPosts = await Task.WhenAny(curPosts);
                    curPosts.Remove(tagPosts);
                    var posts = await tagPosts;
                    foreach(var p in posts.Value.Medias)
                    {
                        result.Add(p.Caption.Text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Insta: Login error");
            }
            return result;
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
                /*SetRequestDelay().*/
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
    }
}
