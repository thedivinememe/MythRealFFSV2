using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MythRealFFSV2.Character;
using System.Collections.Generic;

namespace MythRealFFSV2.Data
{
    /// <summary>
    /// Generates all game data as ScriptableObjects
    /// Run this in Unity Editor via Tools > MythReal > Generate Game Data
    /// </summary>
    public class DataGenerator
    {
#if UNITY_EDITOR
        [MenuItem("Tools/MythReal/Generate Game Data")]
        public static void GenerateAllData()
        {
            Debug.Log("=== Starting Game Data Generation ===");

            GenerateAncestries();
            GenerateBackgrounds();
            GenerateAbilities();
            GenerateTalents();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("=== Game Data Generation Complete ===");
        }

        static void GenerateAncestries()
        {
            Debug.Log("Generating Ancestries...");

            // Human
            var human = CreateAncestry("Human", AncestryType.Human,
                "Humans are versatile and adaptable, found in all corners of the world.",
                adulthoodAge: 18, maxAge: 100, minHeight: 5.0f, maxHeight: 6.5f, startingHP: 8);
            human.attributeIncreases.Add(new AttributeModifier { attributeType = AttributeType.Sociability, bonus = 1 });
            human.attributeIncreases.Add(new AttributeModifier { attributeType = AttributeType.Wits, bonus = 1 });
            human.knownLanguages.Add(Language.Basic);
            human.tags.Add("Versatile");
            human.startingTalentSlots = 1;

            // Elf
            var elf = CreateAncestry("Elf", AncestryType.Elf,
                "Elves are graceful and long-lived, with innate magical abilities.",
                adulthoodAge: 110, maxAge: -1, minHeight: 5.5f, maxHeight: 6.2f, startingHP: 6);
            elf.attributeIncreases.Add(new AttributeModifier { attributeType = AttributeType.Coordination, bonus = 1 });
            elf.attributeIncreases.Add(new AttributeModifier { attributeType = AttributeType.Intelligence, bonus = 1 });
            elf.attributeFlaws.Add(AttributeType.Fortitude);
            elf.knownLanguages.Add(Language.Basic);
            elf.knownLanguages.Add(Language.Elvish);
            elf.tags.Add("LowLightVision");
            elf.tags.Add("Agile");
            elf.startingTalentSlots = 1;

            // Orc
            var orc = CreateAncestry("Orc", AncestryType.Orc,
                "Orcs are powerful warriors with unmatched physical prowess.",
                adulthoodAge: 14, maxAge: 70, minHeight: 6.0f, maxHeight: 7.5f, startingHP: 10);
            orc.attributeIncreases.Add(new AttributeModifier { attributeType = AttributeType.Strength, bonus = 2 });
            orc.attributeFlaws.Add(AttributeType.Intelligence);
            orc.knownLanguages.Add(Language.Basic);
            orc.knownLanguages.Add(Language.Orcish);
            orc.tags.Add("Powerful");
            orc.tags.Add("Intimidating");
            orc.startingTalentSlots = 1;

            // Drake
            var drake = CreateAncestry("Drake", AncestryType.Drake,
                "Drakes are dragon-blooded humanoids with elemental affinities.",
                adulthoodAge: 15, maxAge: 120, minHeight: 5.8f, maxHeight: 7.0f, startingHP: 9);
            drake.attributeIncreases.Add(new AttributeModifier { attributeType = AttributeType.Fortitude, bonus = 1 });
            drake.attributeIncreases.Add(new AttributeModifier { attributeType = AttributeType.Faith, bonus = 1 });
            drake.knownLanguages.Add(Language.Basic);
            drake.knownLanguages.Add(Language.Drakonic);
            drake.tags.Add("DragonBlood");
            drake.tags.Add("ElementalResistance");
            drake.startingTalentSlots = 1;

            Debug.Log("Created 4 Ancestries");
        }

        static void GenerateBackgrounds()
        {
            Debug.Log("Generating Backgrounds...");

            // All 26 backgrounds from the official list
            CreateBackground("Acrobat", 1,
                "You perform daring feats of agility and balance for entertainment and profit.",
                AttributeType.Strength, AttributeType.Coordination,
                SkillType.Athletics, SkillType.Entertain, 14, 0, 50);

            CreateBackground("Entertainer", 2,
                "You make your living performing for crowds, whether through music, theater, or storytelling.",
                AttributeType.Sociability, AttributeType.Wits,
                SkillType.Entertain, SkillType.Speech, 17, 0, 40);

            CreateBackground("Artist", 3,
                "You create works of beauty and meaning through paint, sculpture, or other media.",
                AttributeType.Coordination, AttributeType.Intelligence,
                SkillType.Entertain, SkillType.Insight, 22, 0, 45);

            CreateBackground("Barkeep", 4,
                "You've spent years serving drinks and hearing the stories of countless patrons.",
                AttributeType.Fortitude, AttributeType.Sociability,
                SkillType.Entertain, SkillType.Insight, 19, 0, 60);

            CreateBackground("Bodyguard", 5,
                "You protected others for a living, putting your life on the line for those who hired you.",
                AttributeType.Strength, AttributeType.Fortitude,
                SkillType.Intimidation, SkillType.Survival, 23, 0, 50);

            CreateBackground("Artisan", 6,
                "You are a skilled craftsperson, creating useful and beautiful items.",
                AttributeType.Strength, AttributeType.Intelligence,
                SkillType.Insight, SkillType.Encyclopedia, 22, 0, 75);

            CreateBackground("Criminal", 7,
                "You have a history of breaking the law and operating in the shadows.",
                AttributeType.Coordination, AttributeType.Intelligence,
                SkillType.Deception, SkillType.Stealth, 24, 0, 25);

            CreateBackground("Detective", 8,
                "You investigate crimes and mysteries, piecing together clues to find the truth.",
                AttributeType.Wits, AttributeType.Intelligence,
                SkillType.Inspect, SkillType.Insight, 25, 1, 60);

            CreateBackground("Medic", 9,
                "You have training in treating wounds and illnesses, saving lives through medical knowledge.",
                AttributeType.Fortitude, AttributeType.Wits,
                SkillType.Medicine, SkillType.Encyclopedia, 26, 0, 50);

            CreateBackground("Drifter", 10,
                "You wander from place to place, never settling down, picking up odd jobs and skills.",
                AttributeType.Intelligence, AttributeType.Sociability,
                SkillType.Thievery, SkillType.Deception, 27, 1, 30);

            CreateBackground("Emissary", 11,
                "You served as a diplomatic representative, bridging cultures and negotiating agreements.",
                AttributeType.Intelligence, AttributeType.Sociability,
                SkillType.Religion, SkillType.Speech, 20, 3, 100);

            CreateBackground("Farmer", 12,
                "You worked the land, growing crops and raising livestock to sustain your community.",
                AttributeType.Fortitude, AttributeType.Wits,
                SkillType.Animals, SkillType.Nature, 15, 0, 20);

            CreateBackground("Gambler", 13,
                "You make your living by games of chance, reading people and taking calculated risks.",
                AttributeType.Coordination, AttributeType.Sociability,
                SkillType.Speech, SkillType.Insight, 28, 0, 35);

            CreateBackground("Hermit", 14,
                "You lived in seclusion, away from society, seeking wisdom or avoiding the world.",
                AttributeType.Fortitude, AttributeType.Intelligence,
                SkillType.Medicine, SkillType.Religion, 29, 0, 15);

            CreateBackground("Inventor", 15,
                "You create new devices and mechanisms, pushing the boundaries of what's possible.",
                AttributeType.Coordination, AttributeType.Intelligence,
                SkillType.Insight, SkillType.Encyclopedia, 22, 0, 60);

            CreateBackground("Merchant", 16,
                "You understand trade, commerce, and the value of coin.",
                AttributeType.Intelligence, AttributeType.Sociability,
                SkillType.Speech, SkillType.Insight, 21, 1, 100);

            CreateBackground("Miner", 17,
                "You extract valuable ore and gems from deep beneath the earth.",
                AttributeType.Strength, AttributeType.Wits,
                SkillType.Athletics, SkillType.Survival, 35, 0, 40);

            CreateBackground("Nomad", 18,
                "You traveled with a wandering group, never staying in one place for long.",
                AttributeType.Fortitude, AttributeType.Wits,
                SkillType.Survival, SkillType.Nature, 31, 0, 25);

            CreateBackground("Sailor", 19,
                "You sailed the seas, working aboard ships and facing the dangers of the ocean.",
                AttributeType.Strength, AttributeType.Coordination,
                SkillType.Athletics, SkillType.Survival, 16, 0, 35);

            CreateBackground("Scholar", 20,
                "You devoted years to studying ancient texts and pursuing knowledge.",
                AttributeType.Intelligence, AttributeType.Wits,
                SkillType.Encyclopedia, SkillType.Essence, 32, 2, 75);

            CreateBackground("Shepherd", 21,
                "You tended flocks of animals, protecting them from predators and the elements.",
                AttributeType.Fortitude, AttributeType.Wits,
                SkillType.Insight, SkillType.Animals, 33, 0, 20);

            CreateBackground("Prisoner", 22,
                "You spent time imprisoned, whether justly or not, learning to survive harsh conditions.",
                AttributeType.Strength, AttributeType.Fortitude,
                SkillType.Intimidation, SkillType.Survival, 24, 0, 10);

            CreateBackground("Soldier", 23,
                "You served in an organized military force, learning discipline and combat tactics.",
                AttributeType.Strength, AttributeType.Fortitude,
                SkillType.Athletics, SkillType.Intimidation, 23, 0, 50);

            CreateBackground("Street Rat", 24,
                "You grew up on the streets, surviving through cunning and quick reflexes.",
                AttributeType.Coordination, AttributeType.Fortitude,
                SkillType.Thievery, SkillType.Stealth, 34, 0, 15);

            CreateBackground("Teacher", 25,
                "You educated others, sharing knowledge and shaping young minds.",
                AttributeType.Intelligence, AttributeType.Sociability,
                SkillType.Encyclopedia, SkillType.Insight, 30, 2, 50);

            CreateBackground("Zealot", 26,
                "You are devoted to a cause or philosophy with unwavering fervor.",
                AttributeType.Faith, AttributeType.Strength,
                SkillType.Religion, SkillType.Insight, 13, 0, 30);

            Debug.Log("Created 26 Backgrounds");
        }

        static void GenerateAbilities()
        {
            Debug.Log("Generating Abilities...");

            int abilityCount = 0;

            // === 1H WEAPONS ===
            abilityCount += Create1HWeaponAbilities();

            // === 2H WEAPONS ===
            abilityCount += Create2HWeaponAbilities();

            // === DUAL WIELDING ===
            abilityCount += CreateDualWieldingAbilities();

            // === MORPHOLOGY ===
            abilityCount += CreateMorphologyAbilities();

            // === MAGIC WEAPONS ===
            abilityCount += CreateMagicWeaponAbilities();

            // === RANGED WEAPONS ===
            abilityCount += CreateRangedWeaponAbilities();

            // === LIGHT ===
            abilityCount += CreateLightAbilities();

            // === ARCANE ===
            abilityCount += CreateArcaneAbilities();

            // === BLOOD ===
            abilityCount += CreateBloodAbilities();

            // === FIRE ===
            abilityCount += CreateFireAbilities();

            // === DARK ===
            abilityCount += CreateDarkAbilities();

            // === EARTH ===
            abilityCount += CreateEarthAbilities();

            // === HUNTING ===
            abilityCount += CreateHuntingAbilities();

            // === MARTIAL ARTS ===
            abilityCount += CreateMartialArtsAbilities();

            // === SLY ===
            abilityCount += CreateSlyAbilities();

            // === SUMMONING ===
            abilityCount += CreateSummoningAbilities();

            // === WARFARE ===
            abilityCount += CreateWarfareAbilities();

            // === WATER ===
            abilityCount += CreateWaterAbilities();

            // === WILD ===
            abilityCount += CreateWildAbilities();

            // === WIND ===
            abilityCount += CreateWindAbilities();

            Debug.Log($"Created {abilityCount} Abilities");
        }

        static int Create1HWeaponAbilities()
        {
            CreateAbility("Slash", 1, "Slash enemies in front of you using your 1h weapon", 2, 1, ActionType.Standard,
                1.5f, "3x1m line in front", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.OneHandedWeapons, 1);

            CreateAbility("Parry", 1, "Parry an enemy's physical attack", 2, 2, ActionType.Reaction,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.OneHandedWeapons, 1);

            CreateAbility("Provoke", 2, "Insult your enemy with a disrespectful display, goading them into attacking you", 1, 1, ActionType.Standard,
                4f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.OneHandedWeapons, 2);

            CreateAbility("Flurry", 3, "Deal 3 attacks for the cost of 2", 4, 2, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 3, diceSize = 6, modifier = 3 },
                DamageType.Piercing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.OneHandedWeapons, 3);

            CreateAbility("Knock Back", 4, "Bash an enemy with your shield, knocking them back 2m", 2, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Strength, true, AttributeType.Fortitude,
                TechTreeType.OneHandedWeapons, 4);

            CreateAbility("Boomerang Shield", 5, "Hit multiple enemies with your shield", 3, 2, ActionType.Standard,
                4f, "Up to 3 targets within 4m", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 2 },
                DamageType.Bludgeoning, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.OneHandedWeapons, 5);

            CreateAbility("Riposte", 5, "On any missed melee attack from an enemy, hit them for free", 0, 2, ActionType.Reaction,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.OneHandedWeapons, 5);

            return 7;
        }

        static int Create2HWeaponAbilities()
        {
            CreateAbility("2 Handed Slash", 1, "A powerful slash with a two-handed weapon", 2, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 10, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.TwoHandedWeapons, 1);

            CreateAbility("Intimidate", 1, "Debuff enemies to -1 to attack rolls for duration", 2, 1, ActionType.Standard,
                9f, "Enemies within range", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.TwoHandedWeapons, 1);

            CreateAbility("2H Parry", 2, "Parry an enemy's attack with a two-handed weapon", 2, 2, ActionType.Reaction,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.TwoHandedWeapons, 2);

            CreateAbility("Pushing Strike", 3, "Push target back 1.5m and knock them down on failed save", 3, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 10, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, true, AttributeType.Coordination,
                TechTreeType.TwoHandedWeapons, 3);

            CreateAbility("Surefooted", 4, "Buff teammates +1 attack, you gain +1 attack and double reach", 5, 2, ActionType.Standard,
                6f, "Double reach", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.TwoHandedWeapons, 4);

            CreateAbility("Sweeping Attack", 5, "Attack multiple targets in succession", 2, 2, ActionType.Regular,
                1.5f, "Multiple targets", new DiceRoll { numberOfDice = 1, diceSize = 10, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.TwoHandedWeapons, 5);

            CreateAbility("Ground Strike", 5, "Target is Concussed and Knocked Over on failed save", 3, 2, ActionType.Regular,
                9f, "9m radius", new DiceRoll { numberOfDice = 1, diceSize = 10, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Strength, true, AttributeType.Coordination,
                TechTreeType.TwoHandedWeapons, 5);

            return 7;
        }

        static int CreateDualWieldingAbilities()
        {
            CreateAbility("Twin Strike", 1, "Strike using both weapons without an offhand penalty", 2, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.DualWielding, 1);

            CreateAbility("Swing 'n Block", 1, "Swing with mainhand and ready other for +1 DEF", 3, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.DualWielding, 1);

            CreateAbility("Twin Flurry", 2, "Attack 6 times with disadvantage", 4, 2, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 6, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.DualWielding, 2);

            CreateAbility("Twin Parry", 3, "Parry any physical attacks as a reaction", 2, 2, ActionType.Reaction,
                1.5f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.DualWielding, 3);

            CreateAbility("Side Swipe", 4, "Swipe to either side dealing damage", 3, 2, ActionType.Standard,
                1.5f, "2 targets on either side", new DiceRoll { numberOfDice = 4, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.DualWielding, 4);

            CreateAbility("Vengeful Blades", 5, "Deal double damage back if hit in melee", 2, 3, ActionType.Aura,
                1.5f, "Self", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.DualWielding, 5);

            CreateAbility("Eviscerate", 5, "Deal massive damage to a single enemy", 4, 3, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 10, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.DualWielding, 5);

            return 7;
        }

        static int CreateMorphologyAbilities()
        {
            CreateAbility("Unstable Transformation", 1, "Turn into random basic medium size animal", 2, 1, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Morphology, 1);

            CreateAbility("Mimic", 1, "Take the form of a creature or object you have knowledge of", 2, 1, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Fortitude, false, AttributeType.Fortitude,
                TechTreeType.Morphology, 1);

            CreateAbility("Morphing Missiles", 1, "Throw 3 rocks that morph into various attacks", 2, 1, ActionType.Standard,
                20f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Morphology, 1);

            CreateAbility("Transmogrify", 2, "Turn items of one material type into another", 2, 2, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Morphology, 2);

            CreateAbility("Camouflage", 2, "Turn invisible while you don't move", 1, 1, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Morphology, 2);

            CreateAbility("Adapt to Overcome", 3, "Use utility effect to improve climbing, swimming, running", 1, 3, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Morphology, 3);

            CreateAbility("Embiggen or Ensmallen", 3, "Make something big or small", 3, 2, ActionType.Standard,
                6f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Morphology, 3);

            CreateAbility("Terramorph", 4, "Change the terrain in a 3x3 square", 3, 4, ActionType.Standard,
                18f, "3x3 square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Morphology, 4);

            CreateAbility("Web", 4, "Create a sticky terrain", 2, 2, ActionType.Standard,
                30f, "3x3 Square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Morphology, 4);

            CreateAbility("Polymorph", 5, "Change an enemy into a harmless object", 3, 2, ActionType.Standard,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Morphology, 5);

            CreateAbility("Devolve", 5, "Reduce an enemy's INT majorly", 2, 2, ActionType.Standard,
                6f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Morphology, 5);

            CreateAbility("Mega Form", 5, "Take your Ancestry's ultimate form", 3, 3, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Morphology, 5);

            return 12;
        }

        // Continuing with more ability groups...
        // Due to character limits, I'll create a condensed version for the remaining categories

        static int CreateMagicWeaponAbilities()
        {
            CreateAbility("Essence Armor", 1, "Encase yourself in magical armor, gaining +2 DEF", 2, 1, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MagicWeapons, 1);

            CreateAbility("Zap", 1, "Deal ranged damage to a single target", 3, 1, ActionType.Standard,
                20f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 2 },
                DamageType.Lightning, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MagicWeapons, 1);

            CreateAbility("Essence Missiles", 2, "Deal ranged damage to up to 3 targets", 4, 2, ActionType.Standard,
                20f, "Up to 3 targets", new DiceRoll { numberOfDice = 3, diceSize = 4, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MagicWeapons, 2);

            CreateAbility("Replenish", 3, "Heal one target within 8m", 3, 1, ActionType.Reaction,
                8f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Healing, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MagicWeapons, 3);

            CreateAbility("Quake", 4, "Attempt to knock down creatures in a 4m radius", 4, 2, ActionType.Standard,
                4f, "4m radius", new DiceRoll { numberOfDice = 2, diceSize = 4, modifier = 0 },
                DamageType.Earth, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.MagicWeapons, 4);

            CreateAbility("Magic Weapon Polymorph", 5, "Turn a target into a harmless small creature", 3, 3, ActionType.Aura,
                8f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.MagicWeapons, 5);

            CreateAbility("Twin Cast", 5, "Double the effect of the next ability you use this turn", 2, 4, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MagicWeapons, 5);

            return 7;
        }

        static int CreateRangedWeaponAbilities()
        {
            CreateAbility("Trick Shot", 1, "Able to hit targets when you or the target have a partial view", 2, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.RangedWeapons, 1);

            CreateAbility("Reflexive Shot", 1, "First 3 enemies that move in AOE range get hit with regular attack", 2, 1, ActionType.Reaction,
                30f, "4.5m x 4.5m square", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.RangedWeapons, 1);

            CreateAbility("Double Shot", 2, "Fire two ranged shots", 3, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.RangedWeapons, 2);

            CreateAbility("Close Combat Shot", 3, "Attack with an arrow at close range without disadvantages", 3, 2, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.RangedWeapons, 3);

            CreateAbility("Cover Fire", 4, "Provide cover fire, any attack on teammates is at disadvantage", 2, 1, ActionType.Regular,
                30f, "9m radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.RangedWeapons, 4);

            CreateAbility("Rangers Mark", 5, "Focus on one target, all attack and damage rolls doubled", 2, 2, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Slashing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.RangedWeapons, 5);

            CreateAbility("Rain Fire", 5, "Rain down flaming arrows dealing damage and burn", 5, 2, ActionType.Regular,
                30f, "9m cone", new DiceRoll { numberOfDice = 3, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.RangedWeapons, 5);

            return 7;
        }

        static int CreateLightAbilities()
        {
            CreateAbility("Encourage", 1, "Gives allies Encouraged status or gain 2 AP", 1, 1, ActionType.Standard,
                9f, "9m radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Light, AttributeType.Faith, false, AttributeType.Faith,
                TechTreeType.Light, 1);

            CreateAbility("Minor Healing Light", 1, "Heal a single target", 2, 1, ActionType.Standard,
                20f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Healing, AttributeType.Faith, false, AttributeType.Faith,
                TechTreeType.Light, 1);

            CreateAbility("Burning Light", 1, "Deal light damage to a single target", 2, 1, ActionType.Standard,
                20f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Light, AttributeType.Faith, true, AttributeType.Faith,
                TechTreeType.Light, 1);

            CreateAbility("Dancing Lights", 2, "Gives enemies Blinded status for 3 turns on failed save", 1, 1, ActionType.Standard,
                20f, "Surrounding tiles", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Light, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Light, 2);

            CreateAbility("Light Blade", 2, "A blade of light appears in your hand, any attack is 2d6", 2, 1, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Light, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Light, 2);

            CreateAbility("Aurora", 3, "Target has aura of light, all attacks against them have disadvantage", 3, 3, ActionType.Standard,
                20f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Light, AttributeType.Faith, false, AttributeType.Faith,
                TechTreeType.Light, 3);

            CreateAbility("Blinding Radiance", 4, "Blind enemies and deal damage", 3, 3, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Light, AttributeType.Faith, true, AttributeType.Wits,
                TechTreeType.Light, 4);

            CreateAbility("Dawn", 4, "You and allies gain full AP and wake from sleep", 3, 3, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Light, AttributeType.Faith, false, AttributeType.Faith,
                TechTreeType.Light, 4);

            CreateAbility("Enchant", 5, "Selected targets are Charmed", 3, 2, ActionType.Standard,
                20f, "Multiple targets", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Light, AttributeType.Faith, true, AttributeType.Wits,
                TechTreeType.Light, 5);

            CreateAbility("Light Beam", 5, "A beam of light shines from above dealing massive damage", 3, 3, ActionType.Standard,
                30f, "9m radius", new DiceRoll { numberOfDice = 6, diceSize = 6, modifier = 0 },
                DamageType.Light, AttributeType.Faith, true, AttributeType.Coordination,
                TechTreeType.Light, 5);

            CreateAbility("Divine Healing", 5, "Heal all targeted creatures, gain full AP, cure all negative effects", 3, 3, ActionType.Standard,
                20f, "5 targets", new DiceRoll { numberOfDice = 4, diceSize = 8, modifier = 0 },
                DamageType.Healing, AttributeType.Faith, false, AttributeType.Faith,
                TechTreeType.Light, 5);

            return 11;
        }

        static int CreateArcaneAbilities()
        {
            CreateAbility("Arcane Essence Missiles", 1, "Fire magical missiles at a target", 3, 1, ActionType.Regular,
                30f, "Single target each missile", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Arcane, 1);

            CreateAbility("Charm", 1, "Target gains Charmed effect on fail", 2, 1, ActionType.Regular,
                4.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Arcane, 1);

            CreateAbility("Piercing Whisper", 1, "Communicate with someone far away without alerting anyone else", 1, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Arcane, 1);

            CreateAbility("Illusion", 2, "Create an illusion to fool enemies", 2, 1, ActionType.Regular,
                4.5f, "Area", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, true, AttributeType.Wits,
                TechTreeType.Arcane, 2);

            CreateAbility("Arcane Essence Weapon", 2, "Create an Essence Weapon to fight for you", 2, 1, ActionType.Regular,
                4.5f, "Summon", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Arcane, 2);

            CreateAbility("Sleep", 3, "Target gains Asleep effect on fail", 2, 1, ActionType.Regular,
                9f, "1.5m radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Arcane, 3);

            CreateAbility("Blink", 3, "Teleport to a random location as a reaction to an attack", 1, 1, ActionType.Reaction,
                30f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Arcane, 3);

            CreateAbility("Arcane Counterspell", 4, "Cancel a spell within range as a reaction", 2, 2, ActionType.Reaction,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, true, AttributeType.Intelligence,
                TechTreeType.Arcane, 4);

            CreateAbility("Invisibility", 4, "Turn target invisible for 5 rounds or until action", 2, 2, ActionType.Regular,
                15f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Arcane, 4);

            CreateAbility("Arcane Essence Picks", 5, "Attempt to pick locks using an INT check", 2, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, true, AttributeType.Intelligence,
                TechTreeType.Arcane, 5);

            CreateAbility("Teleport", 5, "Teleport a target from one location to another", 2, 2, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Arcane, 5);

            CreateAbility("Mass Confusion", 5, "Inflict Confused status on all targets within range", 3, 4, ActionType.Regular,
                30f, "5x5 square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Arcane, 5);

            return 12;
        }

        static int CreateBloodAbilities()
        {
            CreateAbility("Animate Dead", 1, "Bring back a creature as a zombie or skeleton", 2, 2, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Blood, 1);

            CreateAbility("False Life", 1, "Add extra HP for the duration of combat", 1, 1, ActionType.Enhancement,
                30f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Blood, 1);

            CreateAbility("Offering", 1, "Deal damage to yourself to roll with advantage", 1, 1, ActionType.Enhancement,
                0f, "Self", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Blood, 1);

            CreateAbility("Blood Charm", 2, "Enemy target cannot attack you or allies while Charmed", 2, 2, ActionType.Standard,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Blood, 2);

            CreateAbility("Feign Death", 2, "Pretend to be dead, enemies make WIT checks", 1, 1, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Blood, 2);

            CreateAbility("Blood Lust", 3, "Creatures in area attack closest creature regardless of ally status", 2, 2, ActionType.Standard,
                30f, "6m x 6m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Blood, 3);

            CreateAbility("Life Transfer", 3, "Transfer life between two friendly targets", 1, 2, ActionType.Standard,
                30f, "2 friendly targets", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Healing, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Blood, 3);

            CreateAbility("Dominate", 4, "Enemy target becomes dominated and charmed", 2, 2, ActionType.Standard,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Blood, 4);

            CreateAbility("Infestation", 4, "Creatures in area receive Diseased status", 3, 2, ActionType.Standard,
                30f, "6m x 6m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Poison, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Blood, 4);

            CreateAbility("Siphon", 5, "Deal damage to enemies and gain that much HP", 2, 2, ActionType.Enhancement,
                18f, "6m radius", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Dark, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Blood, 5);

            CreateAbility("Blood Curse", 5, "Target enemy becomes a monstrosity under your control", 3, 2, ActionType.Standard,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Blood, 5);

            CreateAbility("Summon Demon", 5, "Summon a demon to do your bidding", 3, 3, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Blood, 5);

            return 12;
        }

        static int CreateFireAbilities()
        {
            CreateAbility("Fire Bolt", 1, "Fire a bolt of flame at a target", 2, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Fire, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Fire, 1);

            CreateAbility("Burning Hands", 1, "Deal fire damage in a cone, target becomes burned", 3, 1, ActionType.Regular,
                1.5f, "Touch", new DiceRoll { numberOfDice = 3, diceSize = 6, modifier = 0 },
                DamageType.Fire, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Fire, 1);

            CreateAbility("Smokey Escape", 1, "Move with advantage up to 4.5m away", 1, 1, ActionType.Regular,
                4.5f, "3m radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Fire, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Fire, 1);

            CreateAbility("Fireball", 2, "Hurl a ball of fire at a target", 2, 2, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Fire, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Fire, 2);

            CreateAbility("Flame Shield", 2, "Parry an attack with a shield of fire", 2, 2, ActionType.Reaction,
                1.5f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Fire, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Fire, 2);

            CreateAbility("Fire Arrows", 3, "Enchant arrows with fire, target becomes burned", 2, 2, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 8, modifier = 0 },
                DamageType.Fire, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Fire, 3);

            CreateAbility("Create Ethereal Fire", 3, "Create a versatile ethereal flame", 4, 4, ActionType.Regular,
                30f, "Self or target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Fire, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Fire, 3);

            CreateAbility("Wall of Flames", 4, "Create a wall of flames", 3, 3, ActionType.Regular,
                18f, "6m wall", new DiceRoll { numberOfDice = 2, diceSize = 8, modifier = 0 },
                DamageType.Fire, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Fire, 4);

            CreateAbility("Melt Metal", 4, "Heat metal objects to melting or red-hot", 3, 3, ActionType.Regular,
                30f, "Single object", new DiceRoll { numberOfDice = 3, diceSize = 6, modifier = 0 },
                DamageType.Fire, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Fire, 4);

            CreateAbility("Scorching Ray", 5, "Fire scorching rays at multiple targets", 2, 2, ActionType.Regular,
                30f, "3 targets", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Fire, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Fire, 5);

            CreateAbility("Flame Strike", 5, "Make a flaming melee attack", 3, 2, ActionType.Reaction,
                1.5f, "Single target", new DiceRoll { numberOfDice = 3, diceSize = 8, modifier = 0 },
                DamageType.Fire, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Fire, 5);

            CreateAbility("Flaming Sphere", 5, "Create a sphere of flame that engulfs an area", 5, 4, ActionType.Regular,
                27f, "9m circle", new DiceRoll { numberOfDice = 3, diceSize = 8, modifier = 0 },
                DamageType.Fire, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Fire, 5);

            return 12;
        }

        static int CreateDarkAbilities()
        {
            CreateAbility("Void Touch", 1, "Targeted enemies receive Void status", 2, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Dark, 1);

            CreateAbility("Doubt", 1, "Gives enemies Doubt (Bane) status", 1, 1, ActionType.Debuff,
                9f, "9m radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Faith, true, AttributeType.Faith,
                TechTreeType.Dark, 1);

            CreateAbility("Void Step", 1, "Move up to 9m on a failed melee attack", 1, 1, ActionType.Reaction,
                30f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Dark, 1);

            CreateAbility("Banishment", 2, "Remove creature from battlefield for 3 turns", 2, 2, ActionType.Standard,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Dark, 2);

            CreateAbility("Blind", 2, "Give enemies Blind status", 2, 2, ActionType.Debuff,
                30f, "6m x 6m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Dark, 2);

            CreateAbility("Dark Counterspell", 3, "Cause an enemy ability not to happen", 2, 2, ActionType.Reaction,
                20f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, true, AttributeType.Intelligence,
                TechTreeType.Dark, 3);

            CreateAbility("Shadow Blade", 3, "Add 1d6 Dark damage to all attack abilities", 2, 2, ActionType.Enhancement,
                0f, "Self", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Dark, 3);

            CreateAbility("Curse", 4, "Give targets random debuff status", 3, 2, ActionType.Debuff,
                30f, "6m x 6m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Dark, 4);

            CreateAbility("Dark Sleep", 4, "Give targets Sleep status", 3, 2, ActionType.Debuff,
                30f, "6m x 6m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Dark, 4);

            CreateAbility("Fear", 5, "Give targets Fear status", 3, 2, ActionType.Debuff,
                30f, "10m radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Dark, 5);

            CreateAbility("Dark Beam", 5, "Deal massive damage to all enemies in a line, lose all AP next turn", 1, 3, ActionType.Standard,
                100f, "Straight line", new DiceRoll { numberOfDice = 10, diceSize = 6, modifier = 7 },
                DamageType.Dark, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Dark, 5);

            CreateAbility("Eclipse", 5, "Cause the battlefield to lose all sunlight", 2, 3, ActionType.Standard,
                100f, "Battlefield", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Dark, 5);

            return 12;
        }

        static int CreateEarthAbilities()
        {
            CreateAbility("Launch Rock", 1, "Launch a rock at a target", 2, 1, ActionType.Regular,
                20f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Earth, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Earth, 1);

            CreateAbility("Fortify", 1, "Add +5 temporary health", 1, 1, ActionType.Regular,
                18f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 5 },
                DamageType.Earth, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Earth, 1);

            CreateAbility("Uneven Ground", 1, "Create difficult terrain that knocks enemies prone", 2, 1, ActionType.Regular,
                18f, "4m x 4m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Earth, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Earth, 1);

            CreateAbility("Pocket Sand", 2, "Blind a target for one turn", 1, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Earth, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Earth, 2);

            CreateAbility("Rock Fist", 2, "Add +6 to unarmed damage rolls", 1, 2, ActionType.Enhancement,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 6 },
                DamageType.Earth, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Earth, 2);

            CreateAbility("Ground Pound", 3, "Attempt to knock down enemies in 4m radius", 3, 2, ActionType.Regular,
                12f, "4m radius", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Earth, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Earth, 3);

            CreateAbility("Quick Sand", 3, "Conjure quicksand that traps enemies", 3, 2, ActionType.Regular,
                18f, "4m x 4m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Earth, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Earth, 3);

            CreateAbility("Rock Wall", 4, "Conjure a wall of rock", 3, 2, ActionType.Regular,
                18f, "6m x 2m line", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Earth, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Earth, 4);

            CreateAbility("Earth Catapult", 4, "Launch a target to another location", 2, 2, ActionType.Regular,
                20f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Earth, 4);

            CreateAbility("Earth Quake", 5, "Shake entire battlefield, dealing damage and knocking prone", 4, 3, ActionType.Regular,
                100f, "Entire battlefield", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Earth, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Earth, 5);

            CreateAbility("Vibrate Metal", 5, "Vibrate metal, heating it and causing damage", 2, 2, ActionType.Enhancement,
                24f, "Single target", new DiceRoll { numberOfDice = 3, diceSize = 6, modifier = 0 },
                DamageType.Earth, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Earth, 5);

            CreateAbility("Fissure", 5, "Create a fissure that swallows creatures", 4, 3, ActionType.Regular,
                24f, "6m x 4m rectangle", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Earth, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Earth, 5);

            return 12;
        }

        static int CreateHuntingAbilities()
        {
            CreateAbility("First Aid", 1, "Target receives healing", 2, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Healing, AttributeType.Wits, false, AttributeType.Wits,
                TechTreeType.Hunting, 1);

            CreateAbility("Hunting Reflexive Shot", 1, "First 3 enemies that move in AOE get hit", 2, 1, ActionType.Reaction,
                30f, "4.5m x 4.5m square", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Hunting, 1);

            CreateAbility("Targeted Attack", 1, "Target gets crippled status for one turn", 2, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Hunting, 1);

            CreateAbility("Tracking", 2, "Creatures in full cover become partially visible", 2, 1, ActionType.Regular,
                27f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Hunting, 2);

            CreateAbility("Acid Arrow", 2, "Target gets Acid status", 3, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Hunting, 2);

            CreateAbility("Apply Poison", 3, "Apply poison to weapons or arrows", 2, 1, ActionType.UseItem,
                1.5f, "Single object", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Poison, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Hunting, 3);

            CreateAbility("Animal Friend Attack", 3, "Command your animal friend to attack", 2, 2, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Hunting, 3);

            CreateAbility("Lay of the Land", 4, "Find herbs and move quickly through the area", 3, 3, ActionType.Regular,
                27f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Hunting, 4);

            CreateAbility("Hunting Double Shot", 4, "Fire two shots at a target", 3, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 8, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Hunting, 4);

            CreateAbility("Cure Illness", 5, "Heal target and cure all illness", 2, 2, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Healing, AttributeType.Wits, false, AttributeType.Wits,
                TechTreeType.Hunting, 5);

            CreateAbility("Camouflage", 5, "Blend into surroundings for full cover", 3, 2, ActionType.Hide,
                27f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, false, AttributeType.Wits,
                TechTreeType.Hunting, 5);

            CreateAbility("Deadly Cut", 5, "Target gets crippled status for three turns", 2, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 10, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Hunting, 5);

            return 12;
        }

        static int CreateMartialArtsAbilities()
        {
            CreateAbility("Flurry of Fists", 1, "Make 2 unarmed attacks", 2, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.MartialArts, 1);

            CreateAbility("Falcon Dive", 1, "Move up to 9m, do 1d4 damage to all surrounding enemies", 2, 1, ActionType.Standard,
                18f, "Surrounding enemies", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.MartialArts, 1);

            CreateAbility("Roundhouse Kick", 1, "Enemy gets pushed back and knocked down on failed save", 2, 1, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.MartialArts, 1);

            CreateAbility("Deflect", 2, "Redirect physical attacks", 3, 2, ActionType.Reaction,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.MartialArts, 2);

            CreateAbility("Spirit Weapon", 2, "Conjure a spirit weapon, +2 damage to unarmed", 2, 1, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 2 },
                DamageType.Energy, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MartialArts, 2);

            CreateAbility("Elemental Fist", 3, "Use elemental attacks with your fists", 1, 3, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Energy, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MartialArts, 3);

            CreateAbility("Energy Blast", 3, "Do 5 damage to yourself, fire blast of energy", 2, 2, ActionType.Standard,
                18f, "Single target", new DiceRoll { numberOfDice = 8, diceSize = 4, modifier = 0 },
                DamageType.Energy, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.MartialArts, 3);

            CreateAbility("Unconscious Reflexes", 4, "Add +10 to DEF and Save rolls for 2 rounds", 3, 2, ActionType.Standard,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Energy, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MartialArts, 4);

            CreateAbility("Flying Bicycle Kick", 4, "Deal damage to all creatures in a line", 2, 2, ActionType.Standard,
                24f, "4x1 line", new DiceRoll { numberOfDice = 3, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.MartialArts, 4);

            CreateAbility("Self-Projection", 5, "Project another version of yourself for 3 rounds", 5, 3, ActionType.Standard,
                12f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Energy, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MartialArts, 5);

            CreateAbility("Energy Redirection", 5, "Redirect a non-physical ability that targets you", 3, 2, ActionType.Reaction,
                0f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Energy, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.MartialArts, 5);

            CreateAbility("Suns Uppercut", 5, "Deal massive damage, ignore armor", 3, 2, ActionType.Standard,
                1.5f, "Single target", new DiceRoll { numberOfDice = 12, diceSize = 4, modifier = 8 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.MartialArts, 5);

            return 12;
        }

        static int CreateSlyAbilities()
        {
            CreateAbility("Adrenaline Rush", 1, "Gain 2 AP, lose 2 AP next turn", 0, 1, ActionType.Regular,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Fortitude, false, AttributeType.Fortitude,
                TechTreeType.Sly, 1);

            CreateAbility("Precision Throw", 1, "Throw a knife with possibility of backstab", 2, 1, ActionType.Regular,
                9f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Sly, 1);

            CreateAbility("Sucker Punch", 1, "Target has dazed status for one turn on failed save", 1, 1, ActionType.Reaction,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Sly, 1);

            CreateAbility("Expeditious Retreat", 2, "Move up to 12m", 1, 2, ActionType.Regular,
                36f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Sly, 2);

            CreateAbility("Ether", 2, "Target has asleep status on failed save", 2, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Sly, 2);

            CreateAbility("Hamstring", 3, "Target receives crippled status on failed save", 2, 2, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Sly, 3);

            CreateAbility("Smoke Bomb", 3, "Drop smoke bomb, obfuscate vision and move", 2, 2, ActionType.Reaction,
                12f, "4m radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Sly, 3);

            CreateAbility("Serrated Blade", 4, "Deal damage with bleeding status", 2, 2, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Sly, 4);

            CreateAbility("Numbing Blow", 4, "Prevent enemy from holding a weapon", 2, 2, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Sly, 4);

            CreateAbility("Catch", 5, "Catch a physical projectile and add to inventory", 1, 2, ActionType.Reaction,
                1.5f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Sly, 5);

            CreateAbility("Down They Go", 5, "Instantly kill a target with under 50% health", 4, 3, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Sly, 5);

            CreateAbility("Knife Storm", 5, "Fling knives at surrounding enemies", 5, 3, ActionType.Regular,
                12f, "4m radius", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Piercing, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Sly, 5);

            return 12;
        }

        static int CreateSummoningAbilities()
        {
            CreateAbility("Living Weapon", 1, "Conjure a weapon with varying effects", 1, 1, ActionType.Enhancement,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 1);

            CreateAbility("Conjure Familiar", 1, "Conjure a familiar with varying effects", 1, 1, ActionType.Enhancement,
                0f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 1);

            CreateAbility("Summon Animal", 1, "Summon an animal to assist you", 3, 2, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 1);

            CreateAbility("Underworld Tentacle", 2, "Summon tentacle to fight for you", 2, 2, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Dark, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 2);

            CreateAbility("Summon Spirit", 2, "Summon a spirit with buff/debuff effects", 2, 2, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 2);

            CreateAbility("Conjure Elemental", 3, "Summon an elemental to assist you", 3, 2, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 3);

            CreateAbility("Portable Eye", 3, "Summon a portable eye to sneak on things", 2, 1, ActionType.Summon,
                60f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 3);

            CreateAbility("Summon Golem", 4, "Summon a golem to assist you", 3, 2, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Earth, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 4);

            CreateAbility("Absorb", 4, "Add damage to HP instead of subtracting on success", 3, 2, ActionType.Reaction,
                0f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Summoning, 4);

            CreateAbility("Summon Ritual", 5, "Conjure a pre-selected target on your location", 2, 3, ActionType.Summon,
                0f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 5);

            CreateAbility("Portal", 5, "Create 2 portals that only allies can move through", 3, 2, ActionType.Summon,
                60f, "2 hexes within 20m", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Arcane, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 5);

            CreateAbility("Summon Dragon", 5, "Summon a dragon to assist you", 4, 3, ActionType.Summon,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Fire, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Summoning, 5);

            return 12;
        }

        static int CreateWarfareAbilities()
        {
            CreateAbility("Charge", 1, "Knock over target on failed save", 2, 2, ActionType.Regular,
                18f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Warfare, 1);

            CreateAbility("Battle Cry", 1, "Buffed teammates gain +1 to damage rolls", 1, 2, ActionType.Regular,
                30f, "Allied targets", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 1);

            CreateAbility("Warfare Shield Bash", 1, "Target is dazed on failed save", 1, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Warfare, 1);

            CreateAbility("Warfare Slash", 1, "Basic slash attack", 2, 2, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 1);

            CreateAbility("Cleave", 1, "Attack up to 3 targets", 3, 2, ActionType.Regular,
                1.5f, "Up to 3 targets", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 1);

            CreateAbility("Shove", 1, "Push target back 1.5m, knock over on failed save", 2, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Warfare, 1);

            CreateAbility("Leap into Action", 3, "Jump large distances", 2, 1, ActionType.Regular,
                30f, "Surrounding melee range", new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 2 },
                DamageType.Force, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 3);

            CreateAbility("Warrior Rage", 2, "Gives rage status for 5 turns", 2, 1, ActionType.Regular,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 2);

            CreateAbility("Spin Attack", 4, "Hit all targets in melee range", 3, 2, ActionType.Regular,
                1.5f, "All in melee range", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 4);

            CreateAbility("Shield Wall", 5, "Add +2 DEF, reduce all damage by half", 3, 2, ActionType.Regular,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 5);

            CreateAbility("Protective Strike", 5, "Attack enemies who attack you or allies in melee", 4, 2, ActionType.Reaction,
                1.5f, "Melee range", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Slashing, AttributeType.Strength, false, AttributeType.Strength,
                TechTreeType.Warfare, 5);

            return 11;
        }

        static int CreateWaterAbilities()
        {
            CreateAbility("Frost Armor", 1, "Target gains +2 DEF", 1, 1, ActionType.Regular,
                12f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Cold, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Water, 1);

            CreateAbility("Ray of Frost", 1, "Target gains slowed for one turn", 2, 1, ActionType.Regular,
                24f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Cold, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Water, 1);

            CreateAbility("Water Whip", 1, "Target gains prone status on failed save", 2, 1, ActionType.Regular,
                12f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 12, modifier = 0 },
                DamageType.Water, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Water, 1);

            CreateAbility("Ice Knife", 2, "Melee attack or throw a conjured ice knife", 2, 2, ActionType.Regular,
                18f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 3 },
                DamageType.Cold, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Water, 2);

            CreateAbility("Frostbite", 2, "Cause a target to take cold damage", 2, 1, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Cold, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Water, 2);

            CreateAbility("Freezing Sphere", 3, "Summon a freezing sphere with a freezing aura", 3, 2, ActionType.Summon,
                24f, "Summon", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Cold, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Water, 3);

            CreateAbility("Cone of Cold", 3, "Deal cold damage in a cone in front", 3, 2, ActionType.Regular,
                12f, "4m cone", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Cold, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Water, 3);

            CreateAbility("Rain", 4, "Conjure rain to flood the battlefield", 4, 3, ActionType.Regular,
                30f, "8m x 8m square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Water, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Water, 4);

            CreateAbility("Wall of Ice", 4, "Conjure a wall of ice to block enemies", 3, 2, ActionType.Regular,
                24f, "6m x 2m wall", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Cold, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Water, 4);

            CreateAbility("Binding Ice", 5, "Freeze an enemy in place on failed save", 2, 2, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Cold, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Water, 5);

            CreateAbility("Ice Storm", 5, "Conjure an ice storm to bludgeon enemies for 3 turns", 2, 2, ActionType.Regular,
                30f, "8m x 8m square", new DiceRoll { numberOfDice = 3, diceSize = 6, modifier = 0 },
                DamageType.Cold, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Water, 5);

            CreateAbility("Hydraulic Force", 5, "Conjure powerful beam of water, knocks down on failed save", 3, 3, ActionType.Regular,
                36f, "12m x 2m line", new DiceRoll { numberOfDice = 5, diceSize = 6, modifier = 0 },
                DamageType.Water, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Water, 5);

            return 12;
        }

        static int CreateWildAbilities()
        {
            CreateAbility("Tame Animal", 1, "Attempt to persuade an animal to be your companion", 2, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Sociability, true, AttributeType.Sociability,
                TechTreeType.Wild, 1);

            CreateAbility("Snare", 1, "Snare up to 3 enemies in place for 3 turns", 2, 1, ActionType.Regular,
                30f, "Up to 3 targets", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Wild, 1);

            CreateAbility("Bark Skin", 1, "Turn your skin hard, natural +2 armor", 1, 1, ActionType.Regular,
                0f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Wild, 1);

            CreateAbility("Acid Splash", 2, "Splash acid in front, dealing burn damage", 2, 1, ActionType.Regular,
                6f, "2x3 rectangle", new DiceRoll { numberOfDice = 3, diceSize = 4, modifier = 0 },
                DamageType.Burn, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Wild, 2);

            CreateAbility("Charm Monster", 2, "Charm a creature, making it friendly", 2, 2, ActionType.Regular,
                12f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Wild, 2);

            CreateAbility("Spore Cloud", 3, "Create a cloud of spores that control enemies", 3, 1, ActionType.Regular,
                30f, "2x2 square", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Poison, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Wild, 3);

            CreateAbility("Hold Monster", 3, "Keep creature from taking actions for 5 turns", 2, 2, ActionType.Regular,
                30f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Wild, 3);

            CreateAbility("Mending", 4, "Restore health to any target", 2, 1, ActionType.Regular,
                6f, "Single target", new DiceRoll { numberOfDice = 3, diceSize = 6, modifier = 0 },
                DamageType.Healing, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Wild, 4);

            CreateAbility("Primal Roar", 4, "Give all enemies frightened status within range", 1, 1, ActionType.Regular,
                15f, "5 unit radius", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Wits, true, AttributeType.Wits,
                TechTreeType.Wild, 4);

            CreateAbility("Summon Shroomer", 5, "Summon an ally to fight for you", 3, 2, ActionType.Regular,
                18f, "Single target", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Wild, 5);

            CreateAbility("Stampede", 5, "Summon a stampede across the battlefield", 3, 2, ActionType.Regular,
                100f, "Straight line", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Physical, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Wild, 5);

            return 11;
        }

        static int CreateWindAbilities()
        {
            CreateAbility("Shocking Touch", 1, "Touch attack dealing lightning damage", 2, 1, ActionType.Regular,
                1.5f, "Single target", new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 },
                DamageType.Lightning, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Wind, 1);

            CreateAbility("Bolt", 1, "Fire a lightning bolt at a target", 3, 2, ActionType.Regular,
                27f, "Single target", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Lightning, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Wind, 1);

            CreateAbility("Air Shield", 1, "Parry an enemy's attack with a shield of wind", 3, 1, ActionType.Reaction,
                1.5f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Wind, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Wind, 1);

            CreateAbility("Wind-Aided Jump", 2, "Move anywhere within 9m without provoking opportunity", 3, 1, ActionType.Regular,
                27f, "Self", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Wind, AttributeType.Coordination, false, AttributeType.Coordination,
                TechTreeType.Wind, 2);

            CreateAbility("Gust", 2, "Push target back 1.5m, knock over on failed save", 3, 2, ActionType.Regular,
                9f, "3m wall", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Wind, 2);

            CreateAbility("Air Bubble", 3, "Cast air bubble, any attack deals half damage", 2, 1, ActionType.Regular,
                30f, "Self or ally", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Wind, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Wind, 3);

            CreateAbility("Wind Whip", 3, "Create a 4m whip out of wind, knock over on failed save", 3, 2, ActionType.Regular,
                12f, "4m line", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Wind, 3);

            CreateAbility("Downdraft", 4, "Create a downdraft dealing bludgeoning damage", 3, 2, ActionType.Regular,
                27f, "9m circle", new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 },
                DamageType.Bludgeoning, AttributeType.Coordination, true, AttributeType.Coordination,
                TechTreeType.Wind, 4);

            CreateAbility("Thunderwave", 4, "A thunderous cloud appears", 3, 2, ActionType.Regular,
                27f, "9m circle", new DiceRoll { numberOfDice = 3, diceSize = 6, modifier = 0 },
                DamageType.Thunder, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Wind, 4);

            CreateAbility("Fly", 5, "Touch a creature and grant it the ability to fly", 2, 2, ActionType.Regular,
                1.5f, "Touch", new DiceRoll { numberOfDice = 0, diceSize = 0, modifier = 0 },
                DamageType.Wind, AttributeType.Intelligence, false, AttributeType.Intelligence,
                TechTreeType.Wind, 5);

            CreateAbility("Wind Lightning Bolt", 5, "Target receives Stunned status on failed save", 3, 3, ActionType.Regular,
                27f, "9m line", new DiceRoll { numberOfDice = 5, diceSize = 6, modifier = 0 },
                DamageType.Lightning, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Wind, 5);

            CreateAbility("Poison Cloud", 5, "Target receives poisoned status on failed save", 3, 2, ActionType.Regular,
                27f, "9m circle", new DiceRoll { numberOfDice = 2, diceSize = 10, modifier = 0 },
                DamageType.Poison, AttributeType.Fortitude, true, AttributeType.Fortitude,
                TechTreeType.Wind, 5);

            return 12;
        }

        static void GenerateTalents()
        {
            Debug.Log("Generating Talents...");

            // ANCESTRY TALENTS - Elf
            CreateTalent(1, "Elvish Weapon Training",
                "You favor elegant weapons that require precision and coordination.",
                "You have L1 proficiency with longbows, shortbows, and dexterity-based swords.",
                TalentSource.Ancestry, 1);

            CreateTalent(2, "Otherworldly Essence",
                "Your elven blood courses with magic.",
                "Pick a level 1 arcane ability to add to your memory.",
                TalentSource.Ancestry, 1);

            CreateTalent(3, "Lifetime of Experience",
                "You have lived longer than other mortal races and are able to recall your experience to prepare for any upcoming challenge you may face.",
                "Be at least 100 years old. Gain advantage on knowledge checks.",
                TalentSource.Ancestry, 1);

            // ANCESTRY TALENTS - Orc
            CreateTalent(4, "Beast Tamer",
                "You have an innate ability to deal with animals.",
                "Gain advantage on Animal Handling checks.",
                TalentSource.Ancestry, 1);

            CreateTalent(5, "Specimen",
                "You're made to be an orc.",
                "Your physical prowess is legendary. Gain +2 to Strength-based checks.",
                TalentSource.Ancestry, 1);

            CreateTalent(6, "Tusk Weapon",
                "You have tusks that are dangerous weapons.",
                "Your tusks deal 1d6 piercing damage and can be used as a natural weapon.",
                TalentSource.Ancestry, 1);

            // ANCESTRY TALENTS - Human
            CreateTalent(7, "General Training",
                "You're adaptable and you get more general talents as a result.",
                "Gain one additional general talent slot.",
                TalentSource.Ancestry, 1);

            CreateTalent(8, "Ambitious Nature",
                "You're ambitious so you excel in your fields of study.",
                "Gain +1 to all skill proficiencies.",
                TalentSource.Ancestry, 1);

            CreateTalent(9, "Skilled",
                "You're skilled so you get more skill proficiency.",
                "Gain proficiency in one additional skill of your choice.",
                TalentSource.Ancestry, 1);

            // ANCESTRY TALENTS - Drake
            CreateTalent(10, "Drakonic Breath",
                "You're able to breathe fire as an action.",
                "Breathe fire in a 15ft cone dealing 2d6 fire damage. Usable once per rest.",
                TalentSource.Ancestry, 1);

            CreateTalent(11, "Drakonic Claw",
                "You have sharp claws.",
                "Your claws deal 1d6 slashing damage and can be used as natural weapons.",
                TalentSource.Ancestry, 1);

            CreateTalent(12, "Drakonic Wisdom",
                "You know a lot about the world because of your ancestry.",
                "Gain proficiency in History and Arcana.",
                TalentSource.Ancestry, 1);

            // SKILL TALENTS - Religion
            CreateTalent(13, "One of the Flock",
                "You have an established understanding with other followers of your religion.",
                "Gain advantage on social checks with members of your faith.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Athletics
            CreateTalent(14, "Supreme Balance",
                "You have a great sense of balance due to your training.",
                "Gain advantage on balance and acrobatics checks.",
                TalentSource.Skill, 1);

            CreateTalent(15, "Natural Athlete",
                "You're a natural athlete.",
                "Gain +2 to Athletics checks.",
                TalentSource.Skill, 1);

            CreateTalent(16, "Amphibious Combat",
                "You know how to fight in the water.",
                "No penalty for fighting underwater or in difficult aquatic terrain.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Entertain
            CreateTalent(17, "Routine Performance",
                "You've got a routine for your performance down to the point you can do it in your sleep.",
                "Entertainment roll minimum is 10 + proficiency.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Speech
            CreateTalent(18, "Connections",
                "Anywhere you stay put long enough, you're bound to make connections and be able to find what you need.",
                "Gain contacts in any settlement you spend more than a week in.",
                TalentSource.Skill, 1);

            CreateTalent(19, "Windspitter",
                "You're good at making small talk.",
                "Gain advantage on casual social interactions.",
                TalentSource.Skill, 1);

            CreateTalent(20, "Polylingual",
                "You know many languages.",
                "Learn two additional languages.",
                TalentSource.Skill, 1);

            CreateTalent(21, "Guild Member",
                "You're a member of a guild which gives you some perks.",
                "Gain access to guild resources and discounts.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Crafting
            CreateTalent(22, "Rare Crafting Idea",
                "You're inspired to create something magnificent.",
                "Once per campaign, craft an item of rare quality.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Intimidation
            CreateTalent(23, "Imposing Figure",
                "You're scary.",
                "Gain advantage on Intimidation checks.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Stealth
            CreateTalent(24, "Smuggler",
                "You can smuggle things.",
                "Gain advantage on checks to conceal items on your person.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Inspect
            CreateTalent(25, "Unravel",
                "Spending some time to think about the matter at hand, you gain perspective.",
                "Spend 10 minutes to gain advantage on Investigation checks.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Medicine
            CreateTalent(26, "Combat Medic",
                "You have combat medic experience.",
                "Can stabilize dying creatures as a bonus action.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Deception
            CreateTalent(27, "Charming Deceiver",
                "You're a charming liar.",
                "Gain advantage on Deception checks when lying to someone who finds you attractive.",
                TalentSource.Skill, 1);

            CreateTalent(28, "Takes One to Know One",
                "You can suss out liars.",
                "Gain advantage on Insight checks to detect lies.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Encyclopedia
            CreateTalent(29, "Unreliable Source",
                "You know a lot of things but you're wrong from time to time.",
                "Roll two d20s for knowledge checks, use higher roll but DM may give false information on a 1.",
                TalentSource.Skill, 1);

            CreateTalent(30, "Foundational Knowledge",
                "You have a strong baseline of knowledge.",
                "Gain +2 to Encyclopedia checks.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Survival
            CreateTalent(31, "Foundational Survivor",
                "You know a lot about survival.",
                "Gain +2 to Survival checks.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Essence
            CreateTalent(32, "Essential Familiarity",
                "You are very familiar with the presence of shaped essence.",
                "Detect magic within 30ft and identify spell effects.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Animals
            CreateTalent(33, "Animal Friend",
                "You have a way with animals.",
                "Animals are naturally friendly towards you unless provoked.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Thievery
            CreateTalent(34, "Pickpocket",
                "You know how to pick peoples pockets.",
                "Gain advantage on Sleight of Hand checks to steal from others.",
                TalentSource.Skill, 1);

            // SKILL TALENTS - Investigation
            CreateTalent(35, "Miner's Sight",
                "You have experience in the deep caves.",
                "Detect structural weaknesses and valuable minerals in stone.",
                TalentSource.Skill, 1);

            // TECH TREE TALENTS - Arcane
            CreateTalent(36, "Counterspell",
                "Ability to counterspell.",
                "Use your reaction to attempt to counter an enemy spell.",
                TalentSource.TechTree, 1);

            CreateTalent(37, "Familiar",
                "Choose one of the available familiar options; it gives you bonuses that accumulate as you level.",
                "Summon a magical familiar that aids you in various ways.",
                TalentSource.TechTree, 1);

            CreateTalent(38, "Farsight Spells",
                "Increase range of spells.",
                "Double the range of all your spells.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Dark
            CreateTalent(39, "Sap Life",
                "Void touch heals you for half the damage you dealt.",
                "When you deal dark damage, heal for 50% of damage dealt.",
                TalentSource.TechTree, 1);

            CreateTalent(40, "Hard to See",
                "Enemies have disadvantage on perception when you're hiding.",
                "Gain advantage on Stealth checks in dim light or darkness.",
                TalentSource.TechTree, 1);

            CreateTalent(41, "Shadow Swept",
                "Double your movement in darkness.",
                "Your movement speed doubles when in darkness or dim light.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Earth
            CreateTalent(42, "Rock Steady",
                "Gain advantage on attempts to knock you down.",
                "Advantage on saves against being knocked prone or moved.",
                TalentSource.TechTree, 1);

            CreateTalent(43, "Stoneskin",
                "Gain +1 to DEF.",
                "Your skin hardens like stone, granting +1 to defense.",
                TalentSource.TechTree, 1);

            CreateTalent(44, "Shape Mineral",
                "Manipulate 'earth' you can touch.",
                "Reshape stone and earth within touch range.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Fire
            CreateTalent(45, "Heat Resistance",
                "Provide resistance to Fire and heat damage.",
                "Gain resistance to fire damage (half damage).",
                TalentSource.TechTree, 1);

            CreateTalent(46, "Heat",
                "Heats up a material you can touch.",
                "Heat objects to extreme temperatures with a touch.",
                TalentSource.TechTree, 1);

            CreateTalent(47, "Burn",
                "Your fire spells gain a burn effect.",
                "Fire spells inflict burning (1d4 damage per turn for 3 turns).",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Light
            CreateTalent(48, "Light",
                "You can light up dark spaces.",
                "Create magical light in a 30ft radius at will.",
                TalentSource.TechTree, 1);

            CreateTalent(49, "Punishing Heals",
                "Your healing spells do damage to certain enemies.",
                "Healing spells deal damage to undead and dark creatures.",
                TalentSource.TechTree, 1);

            CreateTalent(50, "Nourishing Aura",
                "Allies within 3m of you receive +1 health at the beginning of your turn.",
                "Emit an aura that slowly heals nearby allies.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Hunting
            CreateTalent(51, "Mark Prey",
                "Target gets 'Marked' status.",
                "Mark a target, granting advantage on attacks against them.",
                TalentSource.TechTree, 1);

            CreateTalent(52, "Track",
                "You gain additional proficiency in Nature and Survival.",
                "Expert tracker, can follow tracks up to a week old.",
                TalentSource.TechTree, 1);

            CreateTalent(53, "Animal Friendship",
                "Additional proficiency in Animals, capacity for an animal companion.",
                "Befriend and train an animal companion.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Martial Arts
            CreateTalent(54, "Dodge",
                "Once per rest, dodge an incoming melee attack by expending this ability.",
                "Use your reaction to dodge one melee attack per rest.",
                TalentSource.TechTree, 1);

            CreateTalent(55, "Athletic Mastery",
                "Improves athletics proficiency.",
                "Gain expertise in Athletics (double proficiency bonus).",
                TalentSource.TechTree, 1);

            CreateTalent(56, "Forceful Hits",
                "Regular attacks attempt to knock down opponents.",
                "Melee attacks can knock prone on failed STR save.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Morphology
            CreateTalent(57, "Shapeshifter",
                "Learn to change shape to one creature of choice.",
                "Transform into one chosen creature form once per rest.",
                TalentSource.TechTree, 1);

            CreateTalent(58, "Mutation",
                "Add 2 proficiencies of your choice.",
                "Mutate your body to gain two skill proficiencies.",
                TalentSource.TechTree, 1);

            CreateTalent(59, "Hardened Carapace",
                "Gain +1 DEF naturally.",
                "Develop a hardened exterior granting +1 defense.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Sly
            CreateTalent(60, "Sneak Attack",
                "When you are behind an enemy or they are flanked, you deal an additional 1d6 physical damage to them on any physical move.",
                "Deal extra damage when attacking from advantage position.",
                TalentSource.TechTree, 1);

            CreateTalent(61, "Cunning Action",
                "You gain certain bonus actions you can use once per turn.",
                "Take Dash, Disengage, or Hide as a bonus action.",
                TalentSource.TechTree, 1);

            CreateTalent(62, "Counter",
                "You attack enemies when they miss you.",
                "Use reaction to counterattack when enemy misses.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Warfare
            CreateTalent(63, "Combat Training",
                "Choose 2 weapon proficiencies and one armor proficiency.",
                "Gain proficiency with two weapons and one armor type.",
                TalentSource.TechTree, 1);

            CreateTalent(64, "Shake it Off",
                "Regain health once per rest.",
                "Use bonus action to regain 1d10 + level HP once per rest.",
                TalentSource.TechTree, 1);

            CreateTalent(65, "Stances",
                "Gain the ability to change stance.",
                "Switch between combat stances (offensive/defensive/balanced).",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Water
            CreateTalent(66, "Shape Water",
                "Manipulate the shape and temperature of water and liquids.",
                "Control and reshape water within 30ft.",
                TalentSource.TechTree, 1);

            CreateTalent(67, "Aquatic Combat",
                "Allows you to breathe and fight underwater.",
                "Breathe underwater and suffer no combat penalties.",
                TalentSource.TechTree, 1);

            CreateTalent(68, "Conjure Water",
                "Create 1 gallon of water once per rest.",
                "Summon clean drinking water from nothing.",
                TalentSource.TechTree, 1);

            // TECH TREE TALENTS - Wind
            CreateTalent(69, "Wind at your Back",
                "Adds +1 to your speed.",
                "Increase movement speed by 5ft.",
                TalentSource.TechTree, 1);

            CreateTalent(70, "Wind Walking",
                "Add to your athletics and jumping ability.",
                "Jump twice as far and high with wind assistance.",
                TalentSource.TechTree, 1);

            CreateTalent(71, "Dandelion in the Wind",
                "Fall great distances without taking damage.",
                "Take no falling damage, land gently like a feather.",
                TalentSource.TechTree, 1);

            Debug.Log("Created 71 Talents");
        }

        // Helper Methods

        static AncestryData CreateAncestry(string name, AncestryType type, string description,
            int adulthoodAge, int maxAge, float minHeight, float maxHeight, int startingHP)
        {
            var ancestry = ScriptableObject.CreateInstance<AncestryData>();
            ancestry.ancestryType = type;
            ancestry.ancestryName = name;
            ancestry.description = description;
            ancestry.adulthoodAge = adulthoodAge;
            ancestry.maxAge = maxAge;
            ancestry.minHeight = minHeight;
            ancestry.maxHeight = maxHeight;
            ancestry.startingHP = startingHP;

            string path = $"Assets/Resources/Data/Ancestries/{name}.asset";
            AssetDatabase.CreateAsset(ancestry, path);
            Debug.Log($"Created {name} ancestry at {path}");
            return ancestry;
        }

        static BackgroundData CreateBackground(string name, int id, string description,
            AttributeType attr1, AttributeType attr2, SkillType skill1, SkillType skill2,
            int profTalentId, int additionalLanguages, int startingNotes)
        {
            var background = ScriptableObject.CreateInstance<BackgroundData>();
            background.backgroundId = id;
            background.backgroundName = name;
            background.description = description;
            background.attributeIncreaseOption1 = attr1;
            background.attributeIncreaseOption2 = attr2;
            background.skillProficiency1 = skill1;
            background.skillProficiency2 = skill2;
            background.additionalLanguages = additionalLanguages;
            background.startingNotes = startingNotes;
            background.typicalBackstory = description; // Using description as backstory for now

            // Note: profTalent will need to be linked later when talents are created
            // Store the talent ID in a comment for reference

            string path = $"Assets/Resources/Data/Backgrounds/{name}.asset";
            AssetDatabase.CreateAsset(background, path);
            Debug.Log($"Created {name} background (Talent ID: {profTalentId}) at {path}");
            return background;
        }

        static AbilityData CreateAbility(string name, int level, string description,
            int actionCost, int memoryCost, ActionType actionType,
            float range, string areaOfEffect,
            DiceRoll damageRoll, DamageType damageType, AttributeType damageAttribute,
            bool requiresSave, AttributeType saveType,
            TechTreeType requiredTechTree, int requiredLevel)
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = name;
            ability.abilityLevel = level;
            ability.description = description;
            ability.actionPointCost = actionCost;
            ability.memoryCost = memoryCost;
            ability.actionType = actionType;
            ability.range = range;
            ability.areaOfEffect = areaOfEffect;
            ability.damageRoll = damageRoll;
            ability.damageType = damageType;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = damageAttribute;
            ability.requiresSave = requiresSave;
            ability.saveType = saveType;
            ability.requiredTechTree = requiredTechTree;
            ability.requiredTechTreeLevel = requiredLevel;

            string path = $"Assets/Resources/Data/Abilities/{name}.asset";
            AssetDatabase.CreateAsset(ability, path);
            Debug.Log($"Created {name} ability at {path}");
            return ability;
        }

        static TalentData CreateTalent(int id, string name, string description, string effect, TalentSource source, int minLevel = 1)
        {
            var talent = ScriptableObject.CreateInstance<TalentData>();
            talent.talentId = id;
            talent.talentName = name;
            talent.description = description;
            talent.effect = effect;
            talent.source = source;
            talent.minimumLevel = minLevel;

            string path = $"Assets/Resources/Data/Talents/{name}.asset";
            AssetDatabase.CreateAsset(talent, path);
            Debug.Log($"Created {name} talent at {path}");
            return talent;
        }
#endif
    }
}
