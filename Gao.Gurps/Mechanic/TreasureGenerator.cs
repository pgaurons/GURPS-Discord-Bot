using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gao.Gurps.Mechanic
{
    /// <summary>
    /// Responsible for treasure generation logic.
    /// </summary>
    public static class TreasureGenerator
    {
        /// <summary>
        /// Generates treasure
        /// </summary>
        /// <returns>An item</returns>
        public static async Task<Treasure> Generate()
        {


            return await GenerateDungeonFantasyTreasure();
        }

        private static async Task<Treasure> GenerateDungeonFantasyTreasure()
        {
            Treasure returnValue = null;

            using (var client = new HttpClient())
            {
                var streamTask = client.GetStreamAsync("https://df-treasure-generator.herokuapp.com/v1/generate/");
                returnValue = (await JsonSerializer.DeserializeAsync<TreasureItems>(await streamTask)).Treasures[0];
            }


            return returnValue;
        }
    }
}
