using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dashboard.Models;
using System.IdentityModel.Tokens;
using Microsoft.Identity.Client;
using Codit.AspNetCore.Microsoft.Graph;

namespace Dashboard.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        GraphClient _msgraph;
        public HomeController(GraphClient msgraph)
        {
            _msgraph = msgraph;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var profile = await _msgraph.GetUserJson();

                var photo = await _msgraph.GetPictureBase64();

                var mail = await _msgraph.ReadMail();

            }

            return View();
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
