namespace MythRealFFSV2.Character
{
    /// <summary>
    /// The seven primary attributes in MythReal
    /// </summary>
    public enum AttributeType
    {
        Coordination,   // COR - Agility, balance, reflexes
        Faith,          // FAI - Divine power, innate magic
        Fortitude,      // FRT - Health, stamina, constitution
        Intelligence,   // INT - Learning, reasoning, memory
        Sociability,    // SOC - Communication, influence
        Strength,       // STR - Physical power
        Wits            // WIT - Awareness, intuition
    }

    /// <summary>
    /// The four ancestries available in the Alpha
    /// </summary>
    public enum AncestryType
    {
        Human,
        Elf,
        Orc,
        Drake
    }

    /// <summary>
    /// Character alignments
    /// </summary>
    public enum AlignmentType
    {
        LawfulGood,
        NeutralGood,
        ChaoticGood,
        LawfulNeutral,
        TrueNeutral,
        ChaoticNeutral,
        LawfulEvil,
        NeutralEvil,
        ChaoticEvil
    }

    /// <summary>
    /// Character sex
    /// </summary>
    public enum SexType
    {
        Male,
        Female,
        Other
    }

    /// <summary>
    /// Skill types
    /// </summary>
    public enum SkillType
    {
        Stealth,
        Athletics,
        Deception,
        Perception,
        Intimidation,
        Entertain,
        Speech,
        Insight,
        Thievery,
        Animals,
        Nature,
        Inspect,
        Essence,
        Encyclopedia,
        Survival,
        Religion,
        Medicine
    }

    /// <summary>
    /// Tech Tree types
    /// </summary>
    public enum TechTreeType
    {
        MartialArts,
        Warfare,
        Sly,
        Hunting,
        Wind,
        Water,
        Fire,
        Earth,
        Light,
        Dark,
        Arcane,
        Summoning,
        Morphology,
        Blood,
        Wild,
        OneHandedWeapons,
        TwoHandedWeapons,
        DualWielding,
        RangedWeapons,
        MagicWeapons
    }

    /// <summary>
    /// Action types for abilities
    /// </summary>
    public enum ActionType
    {
        Standard,
        Reaction,
        Regular,
        Summon,
        Enhancement,
        Debuff,
        Aura,
        Hide,
        UseItem
    }

    /// <summary>
    /// Damage types
    /// </summary>
    public enum DamageType
    {
        Physical,
        Slashing,
        Piercing,
        Blunt,
        Bludgeoning,
        Fire,
        Water,
        Cold,
        Wind,
        Lightning,
        Thunder,
        Earth,
        Light,
        Dark,
        Arcane,
        Poison,
        Acid,
        Burn,
        Healing,
        Energy,
        Force,
        Wild
    }

    /// <summary>
    /// Status conditions
    /// </summary>
    public enum StatusCondition
    {
        None,
        Poisoned,
        Stunned,
        Paralyzed,
        Blinded,
        Deafened,
        Charmed,
        Frightened,
        Petrified,
        Incapacitated,
        Grappled,
        Restrained,
        Exhausted,
        Confused,
        Invisible,
        Slowed,
        Burning,
        Frozen,
        Asleep,
        Weakened,
        Dazed,
        Prone,
        Crippled,
        Bleeding,
        Numb,
        Diseased,
        Marked,
        Encouraged,
        Rage,
        Fear,
        Doubt,
        Void
    }

    /// <summary>
    /// Equipment slot types
    /// </summary>
    public enum EquipmentSlot
    {
        MainHand,
        OffHand,
        TwoHanded,
        Head,
        Chest,
        Legs,
        Feet,
        Hands,
        Back,
        Accessory1,
        Accessory2
    }

    /// <summary>
    /// Armor types
    /// </summary>
    public enum ArmorType
    {
        None,
        Light,
        Medium,
        Heavy,
        Shield
    }

    /// <summary>
    /// Weapon types
    /// </summary>
    public enum WeaponType
    {
        None,
        Dagger,
        Sword,
        Axe,
        Mace,
        Spear,
        Staff,
        Bow,
        Crossbow,
        MagicWeapon,
        Unarmed
    }

    /// <summary>
    /// Languages in the MythReal universe
    /// </summary>
    public enum Language
    {
        Basic,          // Common/English
        Yanalian,
        Drakonic,
        Mythraic,
        Krandalian,
        Elvish,
        Orcish,
        Dwarvish
    }
}
