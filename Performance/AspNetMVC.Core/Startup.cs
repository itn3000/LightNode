using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetMVC.Core
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
        public void Configure(IApplicationBuilder builder)
        {
            builder.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Test}/{action=Echo}/{name?}");
            });
        }
    }
}
