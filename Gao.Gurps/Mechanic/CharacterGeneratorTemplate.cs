using Gao.Gurps.Dice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gao.Gurps.Mechanic
{
    /// <summary>
    /// A template that generates a character, does so randomly.
    /// </summary>
    public class CharacterGeneratorTemplate
    {
        
        public static CharacterGeneratorTemplate PointlessSlayingAndLooting
        {
            get
            {
                var cgt = new CharacterGeneratorTemplate() { Points = 250 };
                cgt.SelectionGroups.Add(
                    new SelectionGroup
                    {
                        Name = "Archetype",
                        PointBased = true,
                        Limit = 100,
                        Options = new List<Option>
                        {
                        new Option {Name = "Agile", PointValue = 100},
                        new Option {Name = "Brainy", PointValue = 100},
                        new Option {Name = "Brawny", PointValue = 100},
                        new Option {Name = "Versatile", PointValue = 100},
                        }
                    }
                );

                cgt.SelectionGroups.Add(
                    new SelectionGroup
                    {
                        Name = "Abilities",
                        PointBased = true,
                        Limit = 80,
                        Options = new List<Option>
                        {
                        new Option {Name = "Beast Whisperer", PointValue = 20, Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry","Animal"), new Option("Wizardry")}},
                        new Option {Name = "Chi Mastery", PointValue = 20},
                        new Option {Name = "Clinging", PointValue = 20, Prerequisites = new List<Option>{new Option("Chi Mastery")}},
                        new Option {Name = "Conjured Companion", PointValue = 20, Prerequisites = new List<Option>{new Option("Demonologist"), new Option("Druidism"), new Option("Minor Wizardry"), new Option("Wizardry")} },
                        new Option("Intuitive") { PointValue = 20},
                        new Option("Coordinated 1") {PointValue = 20},
                        new Option("Coordinated 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Coordinated 1") } },
                        new Option("Coordinated 3") {PointValue = 20, Prerequisites = new List<Option>{new Option("Coordinated 2") } },
                        new Option("Daring") {PointValue = 20},
                        new Option("Defender") {PointValue = 20, Prerequisites = new List<Option>{new Option("Warrior Training") } },
                        new Option("Demonologist") {PointValue = 20},
                        new Option("Detect Evil") {PointValue = 20, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Druidic Energy 1") {PointValue = 20, Prerequisites = new List<Option>{new Option("Druidism") } },
                        new Option("Druidic Energy 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Druidic Energy 1") } },
                        new Option("Druidic Energy 3") {PointValue = 20, Prerequisites = new List<Option>{new Option("Druidic Energy 2") } },
                        new Option("Druidism") {PointValue = 20, Disqualifiers = new List<Option>{new Option("Magic-Resistant")} },
                        new Option("Energetic") {PointValue = 20},
                        new Option("Evasion 1") {PointValue = 20},
                        new Option("Evasion 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Evasion 1") } },
                        new Option("Evasion 3") {PointValue = 20, Prerequisites = new List<Option>{new Option("Evasion 2"), new Option( "Chi Mastery") }, AnyOrAllPrerequisites = false },
                        new Option("Fearless") {PointValue = 20},
                        new Option("Heroic Archer") {PointValue = 20},
                        new Option("Intuitive") {PointValue = 20},
                        new Option("Lay on Hands 1") {PointValue = 20, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Lay on Hands 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Lay on Hands 1") } },
                        new Option("Linguist") {PointValue = 20},
                        new Option {Name = "Lucky 1", PointValue = 20},
                        new Option {Name = "Lucky 2", PointValue = 20, Prerequisites = new List<Option>{ new Option("Lucky 1")} },
                        new Option("Mental Fortress") {PointValue = 20},
                        new Option("Negotiator") {PointValue = 20},
                        new Option {Name = "Nimble 1", PointValue = 20},
                        new Option {Name = "Nimble 2", PointValue = 20, Prerequisites = new List<Option>{ new Option("Nimble 1")} },
                        new Option {Name = "Resistant Caster", PointValue = 20, Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry"), new Option("Theurgy"), new Option("Wizardry")} },
                        new Option {Name = "Run and Hit", PointValue = 20, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Scent Tracker") {PointValue = 20},
                        new Option("Serendipitous 1") {PointValue = 20},
                        new Option("Serendipitous 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Serendipitous 1") } },
                        new Option("Serendipitous 3") {PointValue = 20, Prerequisites = new List<Option>{new Option("Serendipitous 2") } },
                        new Option("Serendipitous 4") {PointValue = 20, Prerequisites = new List<Option>{new Option("Serendipitous 3") } },
                        new Option("Shoulder Checker") {PointValue = 20},
                        new Option("Situational Awareness") {PointValue = 20},
                        new Option("Sixth Sense") {PointValue = 20},
                        new Option {Name = "Smart 1", PointValue = 20},
                        new Option {Name = "Smart 2", PointValue = 20, Prerequisites = new List<Option>{ new Option("Smart 1")} },
                        new Option {Name = "Smart 3", PointValue = 20, Prerequisites = new List<Option>{ new Option("Smart 2")} },
                        new Option("Swift Strike 1") {PointValue = 20, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Swift Strike 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Swift Strike 1") } },
                        new Option {Name = "Thanatologist", PointValue = 20},
                        new Option("Theurgic Energy 1") {PointValue = 20, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Theurgic Energy 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Theurgic Energy 1") } },
                        new Option("Theurgic Energy 3") {PointValue = 20, Prerequisites = new List<Option>{new Option("Theurgic Energy 2") } },
                        new Option("Theurgy") {PointValue = 20, Disqualifiers = new List<Option>{new Option("Magic-Resistant")}},
                        new Option("Turn Undead") {PointValue = 20, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Two-Weapon Fighter") {PointValue = 20, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Unarmed Master") {PointValue = 20, Prerequisites = new List<Option>{ new Option("Chi Mastery")} },
                        new Option("Uninterrupted Flurry") {PointValue = 20, Prerequisites = new List<Option>{ new Option("Chi Mastery")} },
                        new Option {Name = "Wild Talent 1", PointValue = 20},
                        new Option {Name = "Wild Talent 2", PointValue = 20, Prerequisites = new List<Option>{ new Option("Wild Talent 1")} },
                        new Option {Name = "Wild Talent 3", PointValue = 20, Prerequisites = new List<Option>{ new Option("Wild Talent 2")} },
                        new Option {Name = "Wild Talent 4", PointValue = 20, Prerequisites = new List<Option>{ new Option("Wild Talent 3")} },
                        new Option("Wizardly Energy 1") {PointValue = 20, Prerequisites = new List<Option>{ new Option("Minor Wizardry"),new Option("Wizardry") } },
                        new Option("Wizardly Energy 2") {PointValue = 20, Prerequisites = new List<Option>{new Option("Wizardly Energy 1") } },
                        new Option("Wizardly Energy 3") {PointValue = 20, Prerequisites = new List<Option>{new Option("Wizardly Energy 2") } },
                        new Option("Wizardry") {PointValue = 20, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Minor Wizardry")}},
                        new Option {Name = "Alertness 1", PointValue = 10},
                        new Option("Alertness 2") {PointValue = 10, Prerequisites = new List<Option>{new Option("Alertness 1") } },
                        new Option {Name = "Animal Companion", PointValue = 10},
                        new Option {Name = "Arctic Survivor", PointValue = 10},
                        new Option {Name = "Backstabber 1", PointValue = 10},
                        new Option {Name = "Backstabber 2", PointValue = 10, Prerequisites = new List<Option>{ new Option("Backstabber 1")}},
                        new Option("Blessed Agility 1") {PointValue = 10, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Blessed Agility 2") {PointValue = 10, Prerequisites = new List<Option>{new Option("Blessed Agility 1") } },
                        new Option("Blessed Hardiness 1") {PointValue = 10, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Blessed Hardiness 2") {PointValue = 10, Prerequisites = new List<Option>{new Option("Blessed Hardiness 1") } },
                        new Option("Blessed Might 1") {PointValue = 10, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Blessed Might 2") {PointValue = 10, Prerequisites = new List<Option>{new Option("Blessed Might 1") } },
                        new Option("Blessing") {PointValue = 10, Prerequisites = new List<Option>{new Option("Theurgy") } },
                        new Option("Bow Fencer") {PointValue = 10, Prerequisites = new List<Option>{new Option("Heroic Archer") } },
                        new Option {Name = "Cat's Eyes", PointValue = 10},
                        new Option {Name = "Catfall", PointValue = 10},
                        new Option ("Channeling"){ PointValue = 10, Prerequisites = new List<Option>{new Option("Demonologist"), new Option("Minor Wizardry", "Necromantic"), new Option("Thanatologist"), new Option("Theurgy"), new Option("Wizardry") } },
                        new Option("Charm 1"){ PointValue = 10},
                        new Option("Charm 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Charm 1")}},
                        new Option("Combat Medic"){ PointValue = 10},
                        new Option("Desert Survivor"){ PointValue = 10},
                        new Option("Dismissive Wave"){ PointValue = 10, Prerequisites = new List<Option>{new Option("Theurgy") }},
                        new Option("Eagle Eyes"){ PointValue = 10},
                        new Option("Empath 1"){ PointValue = 10},
                        new Option("Empath 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Empath 1")}},
                        new Option("Fitness 1"){ PointValue = 10},
                        new Option("Fitness 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fitness 1")}},
                        new Option {Name = "Foe of Evil 1", PointValue = 10, Prerequisites = new List<Option>{ new Option("Theurgy")}},
                        new Option("Foes of Evil 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Foes of Evil 1")}},
                        new Option("Foes of Evil 3"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Foes of Evil 2")}},
                        new Option("Forceful Strike") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Fortunate 1"){ PointValue = 10},
                        new Option("Fortunate 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fortunate 1")}},
                        new Option("Fortunate 3"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fortunate 2")}},
                        new Option("Fortunate 4"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fortunate 3")}},
                        new Option("Fortunate 5"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fortunate 4")}},
                        new Option("Fortunate 6"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fortunate 5")}},
                        new Option("Fortunate 7"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fortunate 6")}},
                        new Option("Fortunate 8"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Fortunate 7")}},
                        new Option("Gear 1"){ PointValue = 10},
                        new Option("Gear 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Gear 1")}},
                        new Option("Gear 3"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Gear 2")}},
                        new Option("Gear 4"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Gear 3")}},
                        new Option("Gear 5"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Gear 4")}},
                        new Option("Gear 6"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Gear 5")}},
                        new Option("Gear 7"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Gear 6")}},
                        new Option("Gear 8"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Gear 7")}},
                        new Option("High Pain Threshold"){ PointValue = 10},
                        new Option("Iron Will 1"){ PointValue = 10},
                        new Option("Iron Will 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Iron Will 1")}},
                        new Option("Jack of All Trades 1"){ PointValue = 10},
                        new Option("Jack of All Trades 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Jack of All Trades 1")}},
                        new Option("Jack of All Trades 3"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Jack of All Trades 2")}},
                        new Option("Learned 1"){ PointValue = 10},
                        new Option("Learned 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Learned 1")}},
                        new Option("Loyal Henchmen"){ PointValue = 10},
                        new Option("Magic-Resistant"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry"), new Option("Theurgy"), new Option("Wizardry")}},
                        new Option("Massive 1"){ PointValue = 10},
                        new Option("Massive 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Massive 1")}},
                        new Option ("Medium"){ PointValue = 10, Prerequisites = new List<Option>{new Option("Minor Wizardry", "Necromantic"), new Option("Theurgy"), new Option("Wizardry") } },
                        new Option("Mighty Leap 1") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Mighty Leap 2") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Mighty Leap 1") }, AnyOrAllPrerequisites=false },
                        new Option("Mimicry"){ PointValue = 10},
                        new Option("Minor Wizardry", "Spellsmith"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Demonology"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Shamanism"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Thanatology"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Fire"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Food"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Gate"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Healing"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Illusion and Creation"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Knowledge"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Light and Darkness"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Making and Breaking"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Meta"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Mind Control"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Movement"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Necromantic"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Plant"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Protection and Warning"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Sound"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Technological"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Water"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Minor Wizardry", "Weather"){ PointValue = 10, Disqualifiers = new List<Option>{new Option("Magic-Resistant"), new Option("Wizardry")}},
                        new Option("Pack Mule"){ PointValue = 10},
                        new Option("Perfect Recovery"){ PointValue = 10},
                        new Option("Photographic Memory"){ PointValue = 10},
                        new Option("Prepared"){ PointValue = 10},
                        new Option("Rage") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Recovery"){ PointValue = 10},
                        new Option("Resistant"){ PointValue = 10},
                        new Option("Strong 1"){ PointValue = 10},
                        new Option("Strong 2"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Strong 1")}},
                        new Option("Strong 3"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Strong 2")}},
                        new Option("Strong 4"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Strong 3")}},
                        new Option("Strong 5"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Strong 4")}},
                        new Option("Strong 6"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Strong 5")}},
                        new Option("Strong 7"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Strong 6")}},
                        new Option("Strong 8"){ PointValue = 10, Prerequisites = new List<Option>{ new Option("Strong 7")}},
                        new Option("Strong-Arm"){ PointValue = 10},
                        new Option("Swift Sprint 1"){ PointValue = 10,Prerequisites = new List<Option>{ new Option("Chi Mastery") } },
                        new Option("Swift Sprint 2"){ PointValue = 10,Prerequisites = new List<Option>{ new Option("Swift Sprint 1") } },
                        new Option("Toughness 1") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Toughness 2") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Toughness 1")} },
                        new Option("Warrior Training"){ PointValue = 10},
                        new Option("Weapon-and-Shield Fighter 1"){ PointValue = 10,Prerequisites = new List<Option>{ new Option("Warrior Training") } },
                        new Option("Weapon-and-Shield Fighter 2"){ PointValue = 10,Prerequisites = new List<Option>{ new Option("Weapon-and-Shield Fighter 1") } },
                        new Option("Weapon Specialist 1") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Chi Mastery"), new Option("Warrior Training") } },
                        new Option("Weapon Specialist 2") {PointValue = 10, Prerequisites = new List<Option>{ new Option("Weapon Specialist 1")} },

                        }
                    }
                );

                cgt.SelectionGroups.Add(
                    new SelectionGroup
                    {
                        Name = "Heroic Flaws",
                        PointBased = true,
                        Limit = -50,
                        Options = new List<Option>
                        {
                        new Option {Name = "Avaricious", PointValue = -10},
                        new Option {Name = "Cantankerous", PointValue = -10},
                        new Option {Name = "Close-Minded", PointValue = -10},
                        new Option {Name = "Clumsy", PointValue = -10},
                        new Option {Name = "Cursed", PointValue = -10},
                        new Option {Name = "Dark Side", PointValue = -10},
                        new Option {Name = "Foreigner", PointValue = -10},
                        new Option("Gung Ho"){ PointValue = -10},
                        new Option("Hardened"){ PointValue = -10},
                        new Option("Honorable"){ PointValue = -10},
                        new Option("Inquisitive"){ PointValue = -10},
                        new Option("Ivory-Tower Academic"){ PointValue = -10},
                        new Option("Loyal"){ PointValue = -10},
                        new Option("Man-Mountain"){ PointValue = -10},
                        new Option("Mean"){ PointValue = -10},
                        new Option("Murderous"){ PointValue = -10},
                        new Option("Mystical"){ PointValue = -10},
                        new Option("Naive"){ PointValue = -10},
                        new Option("Nature Boy"){ PointValue = -10},
                        new Option("Nervous"){ PointValue = -10},
                        new Option("Nutter"){ PointValue = -10},
                        new Option("Obsessed"){ PointValue = -10},
                        new Option("Old"){ PointValue = -10},
                        new Option("Primitive"){ PointValue = -10},
                        new Option("Religious"){ PointValue = -10},
                        new Option("Saintly"){ PointValue = -10},
                        new Option("Scummy"){ PointValue = -10},
                        new Option("Self-Important"){ PointValue = -10},
                        new Option("Self-Indulgent"){ PointValue = -10},
                        new Option("Troublemaker"){ PointValue = -10},
                        new Option("True Believer"){ PointValue = -10},
                        new Option("Untrusting"){ PointValue = -10},

                        }
                    }
                );


                cgt.SelectionGroups.Add(
                    new SelectionGroup
                    {
                        Name = "Wildcards",
                        PointBased = true,
                        Limit = 120,
                        AllowDuplicates = true,
                        Options = new List<Option>
                        {
                        new Option {Name = "Arcane Lore!", PointValue = 12},
                        new Option {Name = "Archery!", PointValue = 12},
                        new Option {Name = "Artificing!", PointValue = 12},
                        new Option ("Assassination!"){PointValue = 12},
                        new Option ("Bardic Arts!"){PointValue = 12},
                        new Option {Name = "Basic Combat!", PointValue = 12},
                        new Option ("Battlefield Weapons!"){PointValue = 12},
                        new Option ("Beastmaster!"){PointValue = 12},
                        new Option ("Burglar!"){PointValue = 12},
                        new Option ("Chi Control!"){PointValue = 12},
                        new Option ("Chivalry!"){PointValue = 12},
                        new Option {Name = "Con-Man!", PointValue = 12},
                        new Option ("Dungeoneering!"){PointValue = 12},
                        new Option ("Faith!"){PointValue = 12},
                        new Option ("Fast Hands!"){PointValue = 12},
                        new Option ("Fixer!"){PointValue = 12},
                        new Option ("Green Thumb!"){PointValue = 12},
                        new Option ("Healer!"){PointValue = 12},
                        new Option ("Huntsman!"){PointValue = 12},
                        new Option ("Hurled!"){PointValue = 12},
                        new Option ("Investigator!"){PointValue = 12},
                        new Option ("Leader!"){PointValue = 12},
                        new Option("Demonology!"){ PointValue = 12, Prerequisites = new List<Option>{new Option("Demonologist"), new Option("Minor Wizardry", "Demonology"), new Option("Wizardry") } },
                        new Option("Shamanism!"){ PointValue = 12, Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Shamanism"), new Option("Theurgy"), new Option("Wizardry") } },
                        new Option("Thanatology!"){ PointValue = 12, Prerequisites = new List<Option>{new Option("Minor Wizardry", "Thanatology"), new Option("Thanatologist"), new Option("Wizardry") } },
                        new Option("Fire!"){ PointValue = 12, Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Fire"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Food!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Food"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Gate!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Gate"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Healing!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Healing"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Illusion and Creation!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Illusion and Creation"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Knowledge!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Knowledge"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Light and Darkness!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Light and Darkness"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Making and Breaking!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Making and Breaking"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Meta!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Meta"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Mind Control!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Mind Control"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Movement!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Movement"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Necromantic!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Necromantic"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Plant!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Plant"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Protection and Warning!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Protection and Warning"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Sound!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Sound"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Technological!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Technological"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Water!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Water"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option("Weather!"){ PointValue = 12,  Prerequisites = new List<Option>{new Option("Druidism"), new Option("Minor Wizardry", "Weather"), new Option("Theurgy"), new Option("Wizardry") }},
                        new Option ("Martial-Arts Weapons!"){PointValue = 12},
                        new Option ("Mobility!"){PointValue = 12},
                        new Option ("Monsters!"){PointValue = 12},
                        new Option ("Outdoorsman!"){PointValue = 12},
                        new Option ("Sage!"){PointValue = 12},
                        new Option ("Scouting!"){PointValue = 12},
                        new Option ("Sea Dog!"){PointValue = 12},
                        new Option ("Socialize!"){PointValue = 12},
                        new Option ("Spellsmith!"){PointValue = 12, Prerequisites = new List<Option>{new Option("Wizardry"), new Option("Minor Wizardry", "Spellsmith")} },
                        new Option ("Street-Savvy!"){PointValue = 12},
                        new Option ("Swordplay!"){PointValue = 12},
                        new Option ("Teamwork!"){PointValue = 12},
                        new Option ("Unarmed!"){PointValue = 12},


                        }
                    }
                );

                return cgt;
            }
        }
        public int Points { get; set; }
        public List<SelectionGroup> SelectionGroups { get; set; } = new List<SelectionGroup>();

        public List<SelectionGroup> MandatorySelections { get; set; } = new List<SelectionGroup>();

        public List<SelectionGroup> ProduceCharacter()
        {
            
            var remainingBudget = Points;
            var selectionGroups = new List<SelectionGroup>();
            foreach(var group in SelectionGroups)
            {
                var destinationGroup = new SelectionGroup { Name = group.Name, Options = new List<Option>() };
                selectionGroups.Add(destinationGroup);
                var currentLimit = group.Limit;
                var negative = currentLimit < 0;
                while ((currentLimit < 0 && negative) || (currentLimit > 0 && !negative))
                {
                    var allCurrentlySelectedOptions = selectionGroups.SelectMany(g => g.Options).ToList();
                    var allowedOptions = group.Options.
                        //Filter out duplicates if not allowed.
                        Where(o => group.AllowDuplicates || !allCurrentlySelectedOptions.Any(dgo => dgo == o)).
                        //Get all the items with no prerequisites, or the ones for which we meet the prerequisites.
                        Where(o => o.Prerequisites == null || o.Prerequisites.Count == 0 ||
                        (
                            (o.AnyOrAllPrerequisites && o.Prerequisites.Any(p => allCurrentlySelectedOptions.Contains(p))) ||
                            (!o.AnyOrAllPrerequisites && o.Prerequisites.All(p => allCurrentlySelectedOptions.Contains(p)))
                        )).
                        //Hide the ones that we are disqualified from choosing.
                        Where(o => o.Disqualifiers == null || o.Disqualifiers.Count == 0 ||
                        (
                            (!o.Disqualifiers.Any(d => allCurrentlySelectedOptions.Contains(d)))
                        )).
                        //Hide the options we can't afford.
                        Where(o => (o.PointValue <= currentLimit && !negative) || (currentLimit <= o.PointValue && negative)).
                        ToList();
                    Option selection;
                    if (allowedOptions.Count == 0)
                    {
                        selection = new Option { Name = "Leftover Points", PointValue = currentLimit };
                    }
                    else
                    {
                        selection = allowedOptions[Roller.NumberGenerator.Next(0, allowedOptions.Count)];
                    }
                    AddSelection(destinationGroup, selection);
                    currentLimit -= selection.PointValue;
                    remainingBudget -= selection.PointValue;

                }

            }
            selectionGroups.AddRange(MandatorySelections);
            foreach(var mandatorySelectionGroup in MandatorySelections)
            {
                foreach(var option in mandatorySelectionGroup.Options)
                {
                    AddSelection(selectionGroups.First(sg => sg.Name == mandatorySelectionGroup.Name), option);
                }
            }
            return selectionGroups;
        }

        private static void AddSelection(SelectionGroup destinationGroup, Option selection)
        {
            destinationGroup.Options.Add(selection);
        }
    }

    public class SelectionGroup
    {
        /// <summary>
        /// If true, Limit is the amount of points to spend in this category; If False, it is the number of options to select.
        /// </summary>
        public bool PointBased { get; set; } = true;
        public int Limit { get; set; }
        public List<Option> Options { get; set; }
        /// <summary>
        /// Most groups don't allow duplicate selections, but some do.
        /// </summary>
        public bool AllowDuplicates { get; set; } = false;
        /// <summary>
        /// Set to true to make duplicates act as extra levels instead of showing multiple times.
        /// </summary>
        public string Name { get; internal set; }
    }

    public class Option
    {
        public Option(string name, string specialty) 
        {
            Name = name;
            Specialty = specialty;
        }

        public Option(string name) : this(name, "Any") { }
        public Option() : this(string.Empty) { }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public int PointValue { get; set; }
        /// <summary>
        /// If true (default), than any prerequisite needs to match; if false, all must match.
        /// </summary>
        public bool AnyOrAllPrerequisites { get; set; } = true;
        public List<Option> Prerequisites { get; set; } = new List<Option>();
        public List<Option> Disqualifiers { get; set; } = new List<Option>();



        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(Option))
                return base.Equals(obj);
            var casted = (Option)obj;
            return
                Name == casted.Name &&
                (
                    Specialty == casted.Specialty || 
                    (Specialty.ToUpperInvariant() == "ANY" || casted.Specialty.ToUpperInvariant() == "ANY")
                );
        }

        public override string ToString()
        {
            var specialtyString = Specialty.ToUpperInvariant() != "ANY" ? " (" + Specialty + ")" : string.Empty;
            return $"{Name}{specialtyString}[{PointValue}]";
        }
    }

}
