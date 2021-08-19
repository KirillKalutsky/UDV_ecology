using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler.Downloader
{
    public class Program
    {
        public static void Main()
        {
            var str = "16:37&nbsp;18 августа 2021";
            var reg = new Regex("[^0-9А-Яа-я: .]");
            var d = reg.Replace(str, " ");
            string iDate = "19 августа 2021";
            DateTime oDate = DateTime.Parse(d);
            Console.WriteLine(oDate.ToString());
        }
    }
}
