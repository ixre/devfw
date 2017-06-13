using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JR.DevFw.Web.Example.Controllers
{
    public class HomeController : Controller
    {
        public String Echo()
        {
            return "Hello devfw!";

        }
        public ActionResult Index()
        {
            return View();
        }
    }
}