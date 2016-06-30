using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightNode.Server.DNC.Kestrel
{
    using Microsoft.AspNetCore.Hosting;
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:10004")
                .UseStartup<Startup>()
                .Build()
                ;
            host.Run();
        }
    }
}
