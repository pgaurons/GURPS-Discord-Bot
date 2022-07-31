using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Mechanic
{
    public static class Money
    {

        public static CoinPurse ConvertDollarAmountToCoins(decimal amount)
        {
            var coinWeight = 1m / 50m;
            var value = 800m;
            var coinPurse = new CoinPurse();
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Platinum Coin", Value = value, Weight = coinWeight, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 400m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Gold Coin", Value = value, Weight = coinWeight, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 200m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Halved Gold Coin", Value = value, Weight = coinWeight/2m, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 200m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Electrum Coin", Value = value, Weight = coinWeight, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 100m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Quartered Gold Coin", Value = value, Weight = coinWeight / 4m, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 60m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Tumbaga Coin", Value = value, Weight = coinWeight, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 50m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "1/8 Gold Coin", Value = value, Weight = coinWeight / 8m, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 20m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Silver Coin", Value = value, Weight = coinWeight, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 10m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Billon Coin", Value = value, Weight = coinWeight, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            value = 1m;
            if (amount >= value)
            {
                coinPurse.Coins.Add(new Item { Name = "Copper Coin", Value = value, Weight = coinWeight, Quantity = (int)Math.Floor(amount / value) });
                amount %= value;
            }
            if (amount > 0)
            {
                coinPurse.Coins.Add(new Item { Name = "Remainder", Value = amount, Weight = coinWeight * amount, Quantity = 1 });
            }

            return coinPurse;
        }
    }
}

