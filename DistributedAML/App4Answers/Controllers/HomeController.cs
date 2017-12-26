using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App4Answers.Models;
using App4Answers.Models.A4Amodels;
using As.Comms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace App4Answers.Controllers
{
    public class HomeController : Controller
    {
        private A4AModel1 model;
        public HomeController(A4AModel1 model)
        {
            this.model = model;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(new A4ALoginViewModel());
        }

        [HttpPost]
        public IActionResult Index(A4ALoginViewModel login)
        {
            var thisLogin = model.Login(login);
            if (thisLogin.Authenticated == A4ALoginViewModel.AuthenticationResult.Authenticated)
            {
                HttpContext.Session.SetString("User", thisLogin.Email);
                if (thisLogin.AuthenticationAccount.IsAdmin)
                    return RedirectToAction(nameof(Administration),new { objecttype = ObjectTypesAndVerbsAndRoles.ObjectType.Company, verb = ObjectTypesAndVerbsAndRoles.Verb.List });
            }
            return View(thisLogin);
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

        Object GetViewModel(string objecttype, string verb, string itemid, IFormCollection formCollection)
        {
            Object viewModel = "";
            if (objecttype != "" && !String.IsNullOrEmpty(verb))
            {
                var mi = model.GetType().GetMethod(verb + objecttype);
                if (mi == null)
                    throw new Exception(
                        $"Method - {verb}{objecttype} is not found on Model object of type - {model.GetType().Name}");

                if (mi.GetParameters().Length == 0)
                    viewModel = mi.Invoke(model, null);
                else if (mi.GetParameters().Length == 1)
                {
                    if (mi.GetParameters()[0].ParameterType == typeof(IFormCollection))
                        viewModel = mi.Invoke(model, new[] { Request.Form });
                    else if (mi.GetParameters()[0].ParameterType == typeof(string))
                        viewModel = mi.Invoke(model, new[] { itemid });
                    else
                        throw new Exception(
                            $"Method - {verb}{objecttype} takes an unexpected parameter-type {mi.GetParameters()[0].ParameterType.Name}... ");
                }
                else
                {
                    throw new Exception(
                        $"Method - {verb}{objecttype} takes more than one parameters - we are currently only able to deal with one... ");
                }

            }
            return viewModel;
        }

        public IActionResult Administration(String objecttype,String verb,String itemid)
        {
            try
            {
                objecttype = objecttype ?? "";
                verb = verb ?? "";
                itemid = itemid ?? "";


                Object viewModel = GetViewModel(objecttype, verb, itemid, Request.HasFormContentType? Request.Form:null);
                return View(viewModel);

            }
            catch (Exception e)
            {
                if (e.InnerException == null)
                    throw new ApplicationException($"{e.Message} - {e.StackTrace.ToString()}");
                else
                    throw new ApplicationException($"{e.InnerException.Message} - {e.InnerException.StackTrace.ToString()}");

            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
