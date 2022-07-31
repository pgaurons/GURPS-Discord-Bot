using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Gao.Gurps.Model
{
    public class TreasureItems
    {
        [JsonPropertyName("items")]
        public Treasure[] Treasures { get; set; }
    }
    public class Treasure
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("desc")]
        public string Description { get; set; }
        [JsonPropertyName("cost_per_item")]
        public string UnitValue { get; set; }
        [JsonPropertyName("curses")]
        public string[] Curses { get; set; }
        [JsonPropertyName("enchantments")]
        public Enchantment[] Enchantments { get; set; }
        [JsonPropertyName("number")]
        public int Quantity { get; set; }
        [JsonPropertyName("origin")]
        public string Origin { get; set; }
        [JsonPropertyName("supernatural_effects")]
        public string[] SupernaturalEffects { get; set; }
        [JsonPropertyName("treasure_type")]
        public string TreasureType { get; set; }
        [JsonPropertyName("unit")]
        public string Unit { get; set; }
        [JsonPropertyName("weight_per_item")]
        public string UnitWeight { get; set; }
        [JsonPropertyName("size")]
        public string Size { get; set; }
        [JsonPropertyName("size_modifier")]
        public int SizeModifier { get; set; }
    }


    public class Enchantment
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("skill")]
        public int SkillLevel { get; set; }
    }


}
