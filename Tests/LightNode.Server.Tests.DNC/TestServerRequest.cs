using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LightNode.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System.Threading;
using System.Net.Http;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using System.Reflection;

namespace LightNode.Server.Tests.DNC
{
    using Owin;
    public class Startup
    {
        public void Configure(IApplicationBuilder builder, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();
            loggerFactory.AddConsole();
            builder.UseLightNode(new LightNodeOptions(), typeof(Contracts.Hello).GetTypeInfo().Assembly);
        }
    }
    public class TestServerRequest
    {
        private readonly ITestOutputHelper m_OutputHelper;
        public TestServerRequest(ITestOutputHelper outputHelper)
        {
            m_OutputHelper = outputHelper;
        }
        [Fact]
        public void TestRequest()
        {
            using (var csrc = new CancellationTokenSource())
            using (var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:10000")
                .UseStartup<Startup>()
                
                .Build())
            {
                Task.WhenAll(
                    Task.Run(() =>
                    {
                        host.Run(csrc.Token);
                    })
                    ,
                    Task.Run(async () =>
                    {
                        // wait for kestrel server
                        await Task.Delay(500).ConfigureAwait(false);
                        try
                        {
                            for (int i = 0; i < 1000; i++)
                            {
                                using (var requestor = new HttpClient())
                                {
                                    using (var res = await requestor.GetAsync("http://localhost:10000/Hello/Echo?name=abc").ConfigureAwait(false))
                                    {
                                        res.EnsureSuccessStatusCode();
                                        var str = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                                        Assert.False(string.IsNullOrEmpty(str));
                                        m_OutputHelper.WriteLine(str);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            csrc.Cancel();
                        }
                    })
                    ).Wait();
            }

        }
    }
}
