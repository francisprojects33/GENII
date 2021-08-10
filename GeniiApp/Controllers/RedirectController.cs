using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Controllers
{
    public class RedirectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
