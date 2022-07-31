using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    public class CoinPurse
    {
        public IList<Item> Coins { get; set; } = new List<Item>();

        public decimal Weight
        {
            get
            {
                return Coins.Sum(c => c.Weight *c.Quantity);
            }

        }

        public decimal Value
        {
            get
            {
                return Coins.Sum(c => c.Value * c.Quantity);
            }
        }
        
    }
}
