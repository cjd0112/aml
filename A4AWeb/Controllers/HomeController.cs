using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using A4AWeb.Models;

namespace A4AWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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

        public IActionResult Terms()
        {
            ViewData["Message"] = "Your terms page.";

            return View();
        }

        public IActionResult PrivacyPolicy()
        {
            ViewData["Message"] = "Your privacy policy page.";

            return View();
        }

        public IActionResult Support()
        {
            ViewData["Message"] = "Your support page.";

            return View();

        }

        public IActionResult Expert()
        {
            ViewData["Message"] = "Your support page.";

            return View();

        }

        public IActionResult Administration()
        {
            ViewData["Message"] = "Your support page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
