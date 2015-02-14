﻿using Glimpse.LightNode;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glimpse.LightNode
{
    using Glimpse.Core.Framework;
    using System.Web;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class EnableGlimpseMiddleware
    {
        readonly AppFunc next;

        public EnableGlimpseMiddleware(AppFunc next)
        {
            this.next = next;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            return next(environment).ContinueWith((_, state) =>
            {
                ((state as HttpContext).Application["__GlimpseRuntime"] as IGlimpseRuntime).EndRequest();
            }, System.Web.HttpContext.Current);
        }
    }
}

namespace Owin
{
    public static class GlimpseLightNodeMiddlewareExtensions
    {
        public static IAppBuilder EnableGlimpse(this IAppBuilder app)
        {
            return app.Use(typeof(EnableGlimpseMiddleware));
        }
    }
}