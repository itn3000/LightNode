using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightNode.Server;

namespace LightNode.Server.Tests.DNC.Contracts
{
    public class Hello : LightNodeContract
    {
        public string Echo(string name)
        {
            var callName = string.IsNullOrEmpty(name) ? "Unknown" : name;
            return $"Hello {callName}";
        }
        public async Task<string> EchoAsync(string name)
        {
            return await Task.Run(() =>
            {
                var callName = string.IsNullOrEmpty(name) ? "Unknown" : name;
                return $"Hello {callName}";
            });
        }
    }
}
