using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gao.Gurps.Mechanic;
using Gao.Gurps.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gao.Web.Controllers.Api
{
    [Route("api/[controller]")]
    public class MoneyController : Controller
    {
        // GET: api/<controller>
        [HttpGet("dollarsToDungeonFantasyCoins/{amount}")]

        public CoinPurse Get(decimal amount)
        {
            return Money.ConvertDollarAmountToCoins(amount);
        }


    }
}
