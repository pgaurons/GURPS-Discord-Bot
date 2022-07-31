using Gao.Gurps.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{
    [XmlRoot(Namespace = "http://gao.gurps.discord/", IsNullable = false)]
    [XmlType(AnonymousType = true, TypeName = "HeroicBackground", Namespace = "http://gao.gurps.discord/")]
    public class HeroicBackground
    {
        [XmlAttribute]
        public BackgroundBirthplace Birthplace { get; set; }
        [XmlAttribute]
        public BackgroundRegion Region { get; set; }
        [XmlElement]
        public BackgroundParentage Parentage { get; set; }
        [XmlElement]
        public BackgroundCharacter Mentor { get; set; }
        [XmlElement(ElementName = "Sibling")]
        public List<BackgroundCharacter> Siblings { get; set; }
        [XmlElement]
        public BackgroundOmen Omen { get; set; }
        [XmlElement]
        public BackgroundDarkness Darkness {get;set;}
        [XmlElement]
        public BackgroundLegacy Legacy { get; set; }
        [XmlElement]
        public BackgroundInheritance Inheritance { get; set; }
        [XmlElement]
        public BackgroundBurden Burden { get; set; }
        [XmlAttribute]
        public Feature DistinguishingFeature { get; set; }
        [XmlAttribute]
        public Experience ExperienceLevel { get; set; }
    }

    [Serializable]
    public enum Experience
    {
        None,
        [EnumHumanReadableText("services to a Professional Organization or guild")]
        ProfessionalOrganization,
        [EnumHumanReadableText("services to a noble or royal family")]
        Noble,
        [EnumHumanReadableText("life as a Wanderer")]
        Wanderer,
        [EnumHumanReadableText("harsh and difficult life")]
        HarshLife,
        [EnumHumanReadableText("already established career as a practiced Dungeon Adventurer")]
        DungeonAdventurer
    }

    [Serializable]
    public enum Feature
    {
        None,
        Birthmark,
        [EnumHumanReadableText("extensive tattoos")]
        Tattoo,
        Heterochromia,
        [EnumHumanReadableText("Witches' Mark, the streak of white in your hair")]
        WitchesMark,
        UnusualSize,
        Scars
    }

    [XmlType(AnonymousType = true, TypeName = "Burden", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundBurden
    {
        [XmlAttribute]
        public BurdenType Type { get; set; }
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }
        [XmlElement]
        public MarriageObstacleInformation MarriageObstacle { get; set; }
        [XmlAttribute]
        public OutlawType OutlawReason { get; set; }
    }
    [XmlType(AnonymousType = true, TypeName = "MarriageObstacleInformation", Namespace = "http://gao.gurps.discord/")]
    public class MarriageObstacleInformation
    {
        [XmlAttribute]
        public MarriageObstacle Type { get; set; }
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }
    }

    [Serializable]
    public enum OutlawType
    {
        None,
        [EnumHumanReadableText("being accused of a crime")]
        Accusal,
        [EnumHumanReadableText("you accidentally killing or maiming a loved one")]
        AccidentalDeath,
        [EnumHumanReadableText("refusing a request from a royal or noble family")]
        RefusedRuler,
        [EnumHumanReadableText("you were framed for a crime you didn't commit")]
        Framed
    }

    [Serializable]
    public enum MarriageObstacle
    {
        None,
        [EnumHumanReadableText("you are not in love with your promised")]
        YouDontLike,
        [EnumHumanReadableText("the one engaged to you is not in love with you")]
        DoesNotLikeYou,
        [EnumHumanReadableText("the parents demand an unbelievable Dowry")]
        Dowry,
        [EnumHumanReadableText("your love is being held captive")]
        Captive,
        [EnumHumanReadableText("your parents do not approve")]
        Parents,
        [EnumHumanReadableText("your fiance(e) is far too young")]
        Age
    }

    [Serializable]
    public enum BurdenType
    {
        None,
        [EnumHumanReadableText("caring for a Child")]
        Child,
        [EnumHumanReadableText("being Betrothed")]
        Betrothed,
        [EnumHumanReadableText("discovering you are a Lost Heir")]
        LostHeir,
        [EnumHumanReadableText("dealing with being an Outlaw")]
        Outlaw,
        [EnumHumanReadableText("caring for a Dependant")]
        Dependant,
        [EnumHumanReadableText("discovering you are to be a Guardian of a sacred or magical place or rite")]
        Guardian
    }

    [XmlType(AnonymousType = true, TypeName = "Inheritance", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundInheritance
    {
        [XmlAttribute]
        public InheritanceType Type { get; set; }
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }

    }

    [Serializable]
    public enum InheritanceType
    {
        None,
        Tools,
        Deed,
        Pet,
        Companion,
        Transportation
    }

    [XmlType(AnonymousType = true, TypeName = "Legacy", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundLegacy
    {
        [XmlAttribute]
        public LegacyType Type { get; set; }
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }
        [XmlAttribute]
        public BackgroundGrimFate FateOfActorSaved { get; set; }
        [XmlAttribute]
        public MagicItem BrokenMagicItem { get; set; }
    }

    [Serializable]
    public enum MagicItem
    {
        None,
        Weapon,
        MagicalTool,
        Garment,
        Jewelry,
        Media,
        UnusualItem
    }

    [Serializable]
    public enum LegacyType
    {
        None,
        Favor,
        InheritedLand,
        Heir,
        InheritedBook,
        InheritedClue,
        InheritedBrokenMagicItem
    }

    [XmlType(AnonymousType = true, TypeName = "Darkness", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundDarkness
    {
        [XmlAttribute]
        public DarknessType Type {get;set;}
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }
        [XmlElement]
        public BackgroundDarkDeed DarkDeed { get; set; }
        [XmlAttribute]
        public DarkProphecy Prophecy { get; set; }
        [XmlAttribute]
        public Disaster Disaster { get; set; }
    }

    [Serializable]
    public enum DarkProphecy
    {
        None,
        [EnumHumanReadableText("Thou shalt suffer murder at the hands of those that thou loved")]
        KillLovedOnes,
        [EnumHumanReadableText("Thou shalt snuff out the life of one you hold dear")]
        KilledByLovedOnes,
        [EnumHumanReadableText("Thou shalt be always remembered as a traitor to thine land and thine god")]
        Betrayer,
        [EnumHumanReadableText("Thou art doomed to die young")]
        DieYoung,
        [EnumHumanReadableText("Thou shalt become ruler of all; and thou shalt live long enough to see it all slip from thine fingers")]
        GainWorldLoseEverything,
        [EnumHumanReadableText("Thou shalt come to be as that which you despiseth most")]
        BecomeWhatYouHate
    }

    [XmlType(AnonymousType = true, TypeName = "LovedOneData", Namespace = "http://gao.gurps.discord/")]
    public class LovedOne
    {
        [XmlAttribute]
        public LovedOneType Type { get; set; }
        [XmlAttribute]
        public bool StepRelationship { get; set; }
        /// <summary>
        /// True is male, false is female.
        /// </summary>
        [XmlAttribute]
        public bool Gender { get; set; }
        [XmlAttribute]
        public RelativeAge RelativeAge { get; set; }
        [XmlAttribute]
        public TwinType Twin { get; set; }
        [XmlAttribute]
        public int ActualAge { get; set; }
    }

    [Serializable]
    public enum TwinType
    {
        NotTwins,
        [EnumHumanReadableText("Identical twin")]
        Identical,
        [EnumHumanReadableText("Fraternal twin")]
        Fraternal
    }

    [Serializable]
    public enum RelativeAge
    {
        None,
        Younger,
            Older,
            SameAge
    }

    [Serializable]
    public enum LovedOneType
    {
        None,
        Sibling,
        Lover,
        Cousin,
        Grandparent,
        Aunt,
        Parent
    }
    [Serializable]
    public enum Disaster
    {
        None,
        [EnumHumanReadableText("were in a horrible accident")]
        Accident,
        [EnumHumanReadableText("succumbed to the plague")]
        Plague,
        [EnumHumanReadableText("were driven into Poverty")]
        Poverty,
        [EnumHumanReadableText("were caught in an earthquake")]
        Earthquake,
        [EnumHumanReadableText("were drowned")]
        Drowned,
        [EnumHumanReadableText("suddenly vanished")]
        Vanished
    }

    [XmlType(AnonymousType = true, TypeName = "DarkDeed", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundDarkDeed
    {
        [XmlAttribute]
        public DarkDeedType Type { get; set; }
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }
        [XmlAttribute]
        public BackgroundGrimFate GrimFate { get; set; }
        [XmlAttribute]
        public EvildoerMotive Motive { get; set; }
    }

    [Serializable]
    public enum EvildoerMotive
    {
        None,
        Pride,
        AttemptedToStop,
        Lust,
        InTheWay,
        Possession,
        Sacrifice
    }

    [Serializable]
    public enum BackgroundGrimFate
    {
        None,
        [EnumHumanReadableText("Enslavement")]
        Enslaved,
        [EnumHumanReadableText("Imprisonment")]
        Imprisoned,
        [EnumHumanReadableText("a Scarred Face")]
        ScarredFace,
        [EnumHumanReadableText("a Broken Nose")]
        BrokenNose,
        [EnumHumanReadableText("losing an Eye")]
        MissingEye,
        [EnumHumanReadableText("removal of a hand")]
        MissingHand,
        [EnumHumanReadableText("injury leading to a limp")]
        Limp,
        [EnumHumanReadableText("removal of an ear")]
        MissingEar,
        UnspeakableTorment,
        [EnumHumanReadableText("exile")]
        Exiled,
        [EnumHumanReadableText("death (or if that doesn't make sense, being beaten to near-death)")]
        Killed

    }
    [Serializable]
    public enum DarkDeedType
    {
        None,
        Relative,
        Ravage,
        Revenge,
        Duped,
        Attacked
    }
    [Serializable]
    public enum DarknessType
    {
        DarknessFree,
        [EnumHumanReadableText("an Evildoer")]
        Evildoer,
        [EnumHumanReadableText("a Tragic Death")]
        TragicDeath,
        [EnumHumanReadableText("a Dark Prophecy")]
        DarkProphecy
    }

    [XmlType(AnonymousType = true, TypeName = "Omen", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundOmen
    {
        [XmlAttribute]
        public OmenType Type { get; set; }
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }
    }
    [Serializable]
    public enum OmenType
    {
        
        None,
        [EnumHumanReadableText("a Natural Disaster")]
        NaturalDisaster,
        [EnumHumanReadableText("a gathering of Animals")]
        Animals,
        [EnumHumanReadableText("an uprising of Monsters")]
        Monsters,
        [EnumHumanReadableText("a visitation by a Mysterious Stranger")]
        MysteriousStranger,
        [EnumHumanReadableText("a Great Battle")]
        GreatBattle,
        [EnumHumanReadableText("a Celestial Event")]
        CelestialEvent
    }
    [Serializable]
    public enum BackgroundOrphanState
    {
        NotOrphan,
        Abandoned,
        RaisedByAnimals,
        Adopted,
        Stolen,
    }

    [Serializable]
    public enum SupportingCharacterType
    {
        None,
        Captor,
        Evildoer
    }

    [XmlType(AnonymousType = true, TypeName = "Character", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundCharacter
    {
        public BackgroundCharacter() : this(BackgroundCharacterType.Undefined) { }
        public BackgroundCharacter(BackgroundCharacterType type) { Type = type; }
        [XmlAttribute]
        public BackgroundCharacterType Type { get; set; }
        [XmlAttribute]
        public bool IsAlive { get; set; } = true;
        [XmlAttribute]
        public bool IsLover { get; set; }
        [XmlAttribute]
        public bool IsEvildoer { get; set; }
        [XmlAttribute]
        public bool IsShapeShifter { get; set; }
        [XmlAttribute]
        public RelationshipState Attitude { get; set; }
        /// <summary>
        /// The relationship type this character has if they are a lover.
        /// </summary>
        [XmlAttribute]
        public SupportingCharacterType SupportingCharacterType { get; set; }
        /// <summary>
        /// Lovers might have a special related character.
        /// </summary>
        [XmlElement]
        public BackgroundCharacter SupportingActor { get; set; }
        [XmlElement]
        public LovedOne LovedOneInformation { get; set; }
        [XmlAttribute]
        public bool IsGhost { get; set; }
        [XmlAttribute]
        public bool IsMentor { get; set; }
        [XmlAttribute]
        public bool IsApprentice { get; set; }
        [XmlAttribute]
        public HumbleFolkSpecialFeature SpecialFeature { get; set; }
    }

    [Serializable]
    public enum RelationshipState
    {
        None,
        Love,
        Estranged,
        Hate
    }

    [Serializable]
    public enum BackgroundCharacterType
    {
        Undefined,
        Evildoer,
        Noble,
        Crafter,
        Entertainer,
        Rural,
        Merchant,
        Outcast,
        Scholar,
        Faerie,
        CatFolk,
        Dwarf,
        Elf,
        Halfling,
        Gnome,
        Mixed,
        HalfElf,
        HalfOgre,
        HalfOrc,
        Goblin,
        Hobgoblin,
        Orc,
        Bugbear,
        Dinoman,
        Gargoyle,
        HordePygmy,
        LizardMan,
        Minotaur,
        Ogre,
        RockMite,
        SiegeBeast,
        Throttler,
        Troll,
        Wildman,
        Elemental,
        DivineServitor,
        BronzeSpider,
        CorpseGolem,
        GolemArmorSwordsman,
        ObsidianJaguar,
        StoneGolem,
        SwordSpirit,
        AsSharak,
        DemonOfOld,
        DoomChild,
        Hellhound,
        Peshkali,
        Toxifier,
        Cultist,
        DemonFromBeyondTheStars,
        EyeOfDeath,
        Mindwarper,
        SphereOfMadness,
        WatcherAtTheEdgeOfTime,
        Draug,
        FlamingSkull,
        FormerMentor,
        Vampire,
        Lich,
        Skeleton,
        Barbarian,
        Knight,
        HolyWarrior,
        MartialArtist,
        SharpshooterScout,
        Swashbuckler,
        Cleric,
        Druid,
        Bard,
        RangerScout,
        BountyHunterScout,
        BurglarThief,
        MastermindThief,
        AssassinThief,
        ArtilleryWizard,
        ControllerWizard,
        ThaumatologistWizard,
        Bat,
        Snake,
        Cat,
        Bird,
        Wolf,
        Sheep,
        Goat,
        Dog,
        Chicken,
        Cow,
        Horse,
        AcidSpider,
        DireWolf,
        ElectricJelly,
        FleshEatingApe,
        FoulBat,
        FrostSnake,
        GladiatorApe,
        IceWeasel,
        IceWyrm,
        Slorn,
        SlugBeast,
        Triger,
        GiantApe,
        GiantRat,
        GiantSnake,
        GiantSpider,
        Gryphon,
        Dragon,
        Prisoner,
        LovedOne,
        Lover,
        Tyrant,
        Brigand,
        Soldier,
        Pirate,
        Slaver
    }

    [Serializable]
    public enum BackgroundRegion
    {
        Desert,
        Pastoral,
        Highland,
        Island,
        Forest,
        Beyond,
        Hell,
        SeaDepths,
        ElementalRealm,
        FaerieRealm,
        Moon,
        SpiritWorld

    }

    [Serializable]
    public enum BackgroundBirthplace
    {
        Castle,
        SmallVillage,
        Town,
        SacredPlace,
        LonelyPlace,
        Cave
    }

    [Serializable]
    public enum HumbleFolkSpecialFeature
    {
        None,
        Master,
        VeryAttractive,
        VeryWise
    }
}
