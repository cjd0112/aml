using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App4Answers.Models;
using App4Answers.Models.A4Amodels;
using App4Answers.Models.A4Amodels.Login;
using As.Comms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using As.Email;

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
        [AllowAnonymous]
        public IActionResult ReceiveMessage(EmailPostResponse response)
        {
            return new StatusCodeResult((int)HttpStatusCode.NoContent);
        }

        [HttpPost]
        public IActionResult Index(A4ALoginViewModel login)
        {
            var thisLogin = model.Login(login);
            if (thisLogin.Authenticated == A4ALoginViewModel.AuthenticationResult.Authenticated)
            {
                HttpContext.Session.SetString(ModelNames.SessionStrings.User.ToString(), thisLogin.Email);
                if (thisLogin.AuthenticationAccount.IsAdmin)
                {
                    HttpContext.Session.SetString(ModelNames.SessionStrings.Role.ToString(), ModelNames.Role.Administrator.ToString());
                    return RedirectToAction(nameof(Administration), new { objecttype = ModelNames.AdministrationNames.Company, verb = ModelNames.Verb.List });
                }
                else if (thisLogin.AuthenticationAccount.IsExpert)
                {
                    HttpContext.Session.SetString(ModelNames.SessionStrings.Role.ToString(), ModelNames.Role.Expert.ToString());
                    return RedirectToAction(nameof(EmailManager),new {verb=ModelNames.Verb.List,listtype= ModelNames.EmailList.Inbox});
                }
                else if (thisLogin.AuthenticationAccount.IsUser)
                {
                    HttpContext.Session.SetString(ModelNames.SessionStrings.Role.ToString(), ModelNames.Role.User.ToString());
                    return RedirectToAction(nameof(EmailManager), new { verb = ModelNames.Verb.List, listtype = ModelNames.EmailList.Inbox });
                }


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

        Object GetAdministrationViewModel(string objecttype, string verb, string itemid, IFormCollection formCollection,ModelNames.EmailList listType)
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
                        viewModel = mi.Invoke(model, new[] { itemid});
                    else if (mi.GetParameters()[0].ParameterType == typeof(ModelNames.EmailList))
                        viewModel = mi.Invoke(model, new[] {(object) listType});
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


                Object viewModel = GetAdministrationViewModel(objecttype, verb, itemid, Request.HasFormContentType? Request.Form:null,ModelNames.EmailList.None);
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

        

        public IActionResult EmailManager(string verb,string itemid,ModelNames.EmailList listtype)
        {
            try
            {
                verb = verb ?? "";
                itemid = itemid ?? "";

                Object viewModel = GetAdministrationViewModel(ModelNames.AdministrationNames.Message.ToString(), verb, itemid, Request.HasFormContentType ? Request.Form : null,listtype);
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
