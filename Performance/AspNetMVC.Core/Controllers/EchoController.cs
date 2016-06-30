using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetMVC.Core.Controllers
{
    public class EchoController : Controller
    {
        public IActionResult Hello(string name)
        {
            name = name ?? "Unknown";
            return Json($"Hello {name}");
        }
    }
}
