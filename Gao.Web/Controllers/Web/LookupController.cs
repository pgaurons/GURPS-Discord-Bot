using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Gao.Web.Controllers.Web
{
    /// <summary>
    /// The controller for web lookup functionality.
    /// </summary>
    public class LookupController : Controller
    {
        /// <summary>
        /// Clean form.
        /// </summary>
        /// <returns>Empty View.</returns>
        public IActionResult Index()
        {
            return View(new Models.LookupModel());
        }
    }
}