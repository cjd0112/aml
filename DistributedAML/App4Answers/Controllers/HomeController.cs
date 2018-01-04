using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using App4Answers.Models;
using App4Answers.Models.A4Amodels;
using App4Answers.Models.A4Amodels.Base;
using App4Answers.Models.A4Amodels.Login;
using App4Answers.Models.Outlook;
using As.Comms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using As.Email;
using As.Shared;
using Microsoft.AspNetCore.Hosting;

namespace App4Answers.Controllers
{
    public class HomeController : Controller
    {
        private A4AModel1 model;
        private IHostingEnvironment env;

        public HomeController(A4AModel1 model, IHostingEnvironment env)
        {
            this.model = model;
            this.env = env;
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
                HttpContext.Session.SetString(ModelNames.SessionStrings.UserEmail.ToString(), thisLogin.AuthenticationAccount.Email);
                HttpContext.Session.SetString(ModelNames.SessionStrings.UserType.ToString(),thisLogin.AuthenticationAccount.UserType.ToString());
                HttpContext.Session.SetString(ModelNames.SessionStrings.UserName.ToString(), thisLogin.AuthenticationAccount.Name);

                if (thisLogin.AuthenticationAccount.UserType == A4APartyType.Admin)
                {
                    return RedirectToAction(nameof(Administration), new { objecttype = ModelNames.ObjectTypes.Company, verb = ModelNames.Verb.List });
                }
                else if (thisLogin.AuthenticationAccount.UserType == A4APartyType.Expert)
                {
                    return RedirectToAction(nameof(EmailManager),new { objecttype = ModelNames.ObjectTypes.Message,verb = ModelNames.Verb.List,listtype= A4AMailboxType.Inbox});
                }
                else if (thisLogin.AuthenticationAccount.UserType == A4APartyType.User)
                {
                    return RedirectToAction(nameof(EmailManager), new {objecttype = ModelNames.ObjectTypes.Message, verb = ModelNames.Verb.List, listtype = A4AMailboxType.Inbox });
                    //return RedirectToAction(nameof(WebMail2),new {objecttype = ModelNames.ObjectTypes.Message});

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

        public IActionResult Topics()
        {
            var model = this.model.GetTopicView();
            ViewBag.datasource = model.LoadData;
            return View(model);
        }

        public IActionResult WebMail2(string objecttype)
        {
            return View();
        }

        Object GetAdministrationViewModel(string objecttype, string verb, string itemid, IFormCollection formCollection,A4AMailboxType listType)
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
                    else if (mi.GetParameters()[0].ParameterType == typeof(A4AMailboxType))
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

                var vm = viewModel as IViewModel;
                if (vm != null)
                {
                    vm.Verb = verb.ToEnum<ModelNames.Verb>();
                    vm.ObjectTypes = objecttype.ToEnum<ModelNames.ObjectTypes>();
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


                Object viewModel = GetAdministrationViewModel(objecttype, verb, itemid, Request.HasFormContentType? Request.Form:null, A4AMailboxType.None);

                var vm = viewModel as IViewModel;
                if (vm != null)
                {
                    vm.ActionNames = ModelNames.ActionNames.Administration;
                }
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

        

        public IActionResult EmailManager(string objecttype,string verb,string itemid, A4AMailboxType listtype)
        {
            try
            {
                objecttype = objecttype ?? "";
                verb = verb ?? "";
                itemid = itemid ?? "";

                Object viewModel = GetAdministrationViewModel(objecttype, verb, itemid, Request.HasFormContentType ? Request.Form : null,listtype);

                var vm = viewModel as IViewModel;
                if (vm != null)
                {
                    vm.ActionNames = ModelNames.ActionNames.EmailManager;
                }
                var model = this.model.GetTopicView();
                ViewBag.datasource = model.LoadData;
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
