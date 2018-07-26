using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SON_eStore.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Errror
        public ActionResult ErrorAlert()
        {
            return View();
        }
        public ActionResult NotFound404()
        {
            return View();
        }
    }
}