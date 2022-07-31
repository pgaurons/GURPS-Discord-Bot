using Microsoft.AspNetCore.Mvc;
using Gao.Gurps.Lookup;
using Gao.Gurps.Model;
using System.Collections.Generic;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gao.Web.Controllers.Api
{
    /// <summary>
    /// does a search through documents.
    /// </summary>
    [Route("api/[controller]")]
    public class LookupController : Controller
    {

        /// <summary>
        /// Given a Regex, returns matching index references
        /// </summary>
        /// <param name="expression">A regular expression search query.</param>
        /// <returns>a list of matching index references</returns>
        // GET api/<controller>/5
        [HttpGet("index/{expression}")]
        public IEnumerable<IndexReference> Get(string expression)
        {
            return Index.FindInIndex(expression);
        }




    }
}
