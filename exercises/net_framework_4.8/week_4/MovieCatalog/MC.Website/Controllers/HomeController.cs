using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace MC.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MyPage(int? id, string name)
        {
            ViewBag.Name = name;
            ViewBag.Count = id;

            return View();
        }

        public ActionResult Calculator(double? a, double? b)
        {
            ViewBag.Perimeter = a * 2 + b * 2;
            ViewBag.Aria = a * b;

            return View();
        }
    }
}