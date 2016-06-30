using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace LightNode.Server.DNC.Kestrel
{
    using Microsoft.AspNetCore.Builder;
    using Owin;
    public class Startup
    {
        public void Configure(IApplicationBuilder builder)
        {
            builder.UseLightNode(new LightNodeOptions(), typeof(Contracts.Echo).GetTypeInfo().Assembly);
        }
    }
}
