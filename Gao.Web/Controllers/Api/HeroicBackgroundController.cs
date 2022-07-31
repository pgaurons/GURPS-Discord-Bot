using System.Collections.Generic;
using Gao.Gurps;
using Gao.Gurps.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gao.Web.Controllers.Api
{
    [Route("api/[controller]")]
    public class HeroicBackgroundController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public HeroicBackground Get()
        {
            return LookupTables.GenerateHeroicBackground();
        }

    }
}
