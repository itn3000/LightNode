using LightNode.Server;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using LightNode.Diagnostics;
using System.Diagnostics;
using System.Linq;

namespace LightNode.Server
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class RegisteredHandlersInfo
    {
        public string EngineId { get; private set; }
        public ILightNodeOptions Options { get; private set; }
        public IReadOnlyCollection<KeyValuePair<string, OperationInfo>> RegisteredHandlers { get; private set; }

        public RegisteredHandlersInfo(string engineId, ILightNodeOptions options, IReadOnlyCollection<KeyValuePair<string, OperationInfo>> registeredHandlers)
        {
            this.EngineId = engineId;
            this.Options = options;
            this.RegisteredHandlers = registeredHandlers;
        }
    }

    public class LightNodeServerMiddleware
    {
        static readonly object runningHandlerLock = new object();
        static ILookup<string, RegisteredHandlersInfo> runningHandlers =
            Enumerable.Empty<RegisteredHandlersInfo>().ToLookup(x => "", x => x);

        /// <summary>
        /// Get all registered handlers. Key is ILightNodeOptions.ServerEngineId.
        /// </summary>
        public static ILookup<string, RegisteredHandlersInfo> GetRegisteredHandlersInfo()
        {
            return runningHandlers;
        }

        readonly LightNodeServer engine;
        readonly bool useOtherMiddleware;
        readonly AppFunc next;

        public LightNodeServerMiddleware(AppFunc next, ILightNodeOptions options)
            : this(next, options, Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(asm => Assembly.Load(asm)).ToArray())
        {
        }

        public LightNodeServerMiddleware(AppFunc next, ILightNodeOptions options, Assembly[] hostAssemblies)
        {
            this.next = next;
            this.useOtherMiddleware = options.UseOtherMiddleware;
            this.engine = new LightNodeServer(options);

            var sw = Stopwatch.StartNew();
            var registeredHandler = this.engine.RegisterHandler(hostAssemblies);
            options.Logger.InitializeComplete(sw.Elapsed.TotalMilliseconds);

            lock (runningHandlerLock)
            {
                runningHandlers = runningHandlers.SelectMany(g => g, (g, xs) => new { g.Key, xs })
                    .Concat(new[] { new { Key = options.ServerEngineId, xs = new RegisteredHandlersInfo(options.ServerEngineId, options, registeredHandler) } })
                    .ToLookup(x => x.Key, x => x.xs);
            }
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (useOtherMiddleware)
            {
                await engine.ProcessRequest(environment).ConfigureAwait(true); // keep context
                await next(environment).ConfigureAwait(false);
            }
            else
            {
                await engine.ProcessRequest(environment).ConfigureAwait(false);
            }
        }
    }
}

namespace Owin
{
    using IAppBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;
    using Microsoft.AspNetCore.Builder;
    public static class AppBuilderLightNodeMiddlewareExtensions
    {
        public static IAppBuilder UseLightNode(this IAppBuilder app)
        {
            return UseLightNode(app, new LightNodeOptions());
        }

        public static IAppBuilder UseLightNode(this IAppBuilder app, ILightNodeOptions options)
        {
            return app.UseLightNode(options, Assembly.GetEntryAssembly());
        }

        public static IAppBuilder UseLightNode(this IAppBuilder app, ILightNodeOptions options, params Assembly[] hostAssemblies)
        {
            return app.UseOwin(pipeline =>
            {
                pipeline(next =>
                {
                    return new LightNodeServerMiddleware(next, options, hostAssemblies).Invoke;
                });
            });
        }
    }
}