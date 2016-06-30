using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LightNode.Server.DNC.Kestrel.Contracts
{
    using LightNode.Server;
    public class Echo : LightNodeContract
    {
        public string Hello(string name)
        {
            name = name ?? "Unknown";
            return $"Hello {name}";
        }
    }
}
