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

namespace App4Answers.Controllers
{
    public class HomeController : Controller
    {
        private A4AModel1 model;
        public HomeController(A4AModel1 model)
        {
            this.model = model;
        }

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

        Object GetViewModel(string category, string verb, string itemid, IFormCollection formCollection)
        {
            Object viewModel = "";
            if (category != "" && !String.IsNullOrEmpty(verb))
            {
                var mi = model.GetType().GetMethod(verb + category);
                if (mi == null)
                    throw new Exception(
                        $"Method - {verb}{category} is not found on Model object of type - {model.GetType().Name}");

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
                            $"Method - {verb}{category} takes an unexpected parameter-type {mi.GetParameters()[0].ParameterType.Name}... ");
                }
                else
                {
                    throw new Exception(
                        $"Method - {verb}{category} takes more than one parameters - we are currently only able to deal with one... ");
                }

            }
            return viewModel;
        }

        public IActionResult Administration(String category,String verb,String itemid)
        {
            try
            {
                category = category ?? "";
                verb = verb ?? "";
                itemid = itemid ?? "";


                Object viewModel = GetViewModel(category, verb, itemid, Request.HasFormContentType? Request.Form:null);

                /*

                if (verb == "New" && category == "Companies")
                {
                    var nm = this.model.AddCompany();
                    viewModel = new ViewModelListBase(typeof(A4ACompanySummaryViewModel), model.GetCompanies());
                }
                else if (verb == "List" && category == "Companies")
                {
                    viewModel = new ViewModelListBase(typeof(A4ACompanySummaryViewModel), model.GetCompanies());
                }
                else if (verb == "Edit" && category == "Companies")
                {
                    viewModel = model.GetCompany(itemid);
                }
                else if (verb == "Delete" && category == "Companies")
                {
                    model.DeleteCompany(itemid);
                    viewModel = new ViewModelListBase(typeof(A4ACompanySummaryViewModel), model.GetCompanies());
                }
                else if (verb == "Save" && category == "Companies")
                {
                    var updatedCompany = new A4ACompanyDetailViewModel(Request.Form);
                    model.SaveCompany(updatedCompany);
                    viewModel = new ViewModelListBase(typeof(A4ACompanySummaryViewModel), model.GetCompanies());

                }
                */
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
