using GeniiApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AppRoles()
        {
            return View();
        }

        public async Task<bool> Create3Roles()
        {
            bool result = false;

            bool roleFoundAdmin = await _roleManager.RoleExistsAsync("Admin");
            bool roleFoundManager = await _roleManager.RoleExistsAsync("Manager");
            bool roleFoundUser = await _roleManager.RoleExistsAsync("User");

            if (!roleFoundAdmin)
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                await _roleManager.CreateAsync(role);
                result = true;
            }

            if (!roleFoundManager)
            {
                var role = new IdentityRole();
                role.Name = "Manager";
                await _roleManager.CreateAsync(role);
                result = true;
            }

            if (!roleFoundUser)
            {
                var role = new IdentityRole();
                role.Name = "User";
                await _roleManager.CreateAsync(role);
                result = true;
            }

            return result;
        }
    }
}
