using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetMVC.Core
{
    using Microsoft.AspNetCore.Server.Kestrel;
    using Microsoft.AspNetCore.Hosting;
    using System.IO;
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseUrls("http://localhost:10003")
                .UseStartup<Startup>()
                .Build()
                ;
            host.Run();
        }
    }
}
