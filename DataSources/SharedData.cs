using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataSources
{
    public static class SharedData
    {
        public static readonly HttpClient HttpClient = new HttpClient();

        public static readonly string instLogin = "double_k_v";
        public static readonly string instPass = "abrakadabra77";
    }
}
