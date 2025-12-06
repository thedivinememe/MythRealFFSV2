using UnityEngine;
using UnityEditor;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;
using System.Collections.Generic;

namespace MythRealFFSV2.Editor
{
    /// <summary>
    /// Editor utility to populate ScriptableObject assets from the MythReal rulebook
    /// Run this from the Unity menu: MythReal → Populate All Data
    /// </summary>
    public class DataPopulator : EditorWindow
    {
        [MenuItem("MythReal/Populate All Data")]
        public static void PopulateAllData()
        {
            if (EditorUtility.DisplayDialog("Populate Data",
                "This will create all ScriptableObject assets for MythReal. Continue?",
                "Yes", "Cancel"))
            {
                CreateAncestries();
                CreateBackgrounds();
                CreateTalents();
                CreateAbilities();
                CreateItems();
                CreateTechTrees();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Complete",
                    "All data assets have been created successfully!", "OK");
            }
        }

        #region Ancestries
        [MenuItem("MythReal/Create Data/Ancestries")]
        static void CreateAncestries()
        {
            Debug.Log("Creating Ancestry assets...");

            // Human
            CreateHumanAncestry();
            // Elf
            CreateElfAncestry();
            // Orc
            CreateOrcAncestry();
            // Drake
            CreateDrakeAncestry();

            Debug.Log("Ancestry assets created!");
        }

        static void CreateHumanAncestry()
        {
            var human = ScriptableObject.CreateInstance<AncestryData>();
            human.ancestryType = AncestryType.Human;
            human.ancestryName = "Human";
            human.description = "Humans are generally the most adaptable and resilient beings in the Mythoverse. They're able to thrive in almost any setting.";
            human.adulthoodAge = 16;
            human.maxAge = 95;
            human.minHeight = 4.9f;
            human.maxHeight = 6.6f;
            human.startingHP = 8;

            // Humans get +2 to any attributes (handled at character creation)
            human.attributeIncreases = new List<AttributeModifier>();
            human.attributeFlaws = new List<AttributeType>();

            human.knownLanguages = new List<Language> { Language.Basic, Language.Yanalian };
            human.tags = new List<string> { "Human", "Humanoid" };
            human.startingTalentSlots = 1;

            CreateAsset(human, "Assets/ScriptableObjects/Ancestries/Human.asset");
        }

        static void CreateElfAncestry()
        {
            var elf = ScriptableObject.CreateInstance<AncestryData>();
            elf.ancestryType = AncestryType.Elf;
            elf.ancestryName = "Elf";
            elf.description = "Elves are graceful beings who can live forever. This changes their perspective on things that other beings find important.";
            elf.adulthoodAge = 70;
            elf.maxAge = -1; // Immortal
            elf.minHeight = 4.9f;
            elf.maxHeight = 6.6f;
            elf.startingHP = 6;

            elf.attributeIncreases = new List<AttributeModifier>
            {
                new AttributeModifier { attributeType = AttributeType.Coordination, bonus = 1 },
                new AttributeModifier { attributeType = AttributeType.Intelligence, bonus = 1 },
                new AttributeModifier { attributeType = AttributeType.Faith, bonus = 1 }
            };
            elf.attributeFlaws = new List<AttributeType> { AttributeType.Fortitude };

            elf.knownLanguages = new List<Language> { Language.Basic, Language.Yanalian, Language.Mythraic };
            elf.tags = new List<string> { "Elf", "Humanoid" };
            elf.startingTalentSlots = 1;

            CreateAsset(elf, "Assets/ScriptableObjects/Ancestries/Elf.asset");
        }

        static void CreateOrcAncestry()
        {
            var orc = ScriptableObject.CreateInstance<AncestryData>();
            orc.ancestryType = AncestryType.Orc;
            orc.ancestryName = "Orc";
            orc.description = "Orcs are a large, strong race who historically have engaged in war for sport.";
            orc.adulthoodAge = 13;
            orc.maxAge = 50;
            orc.minHeight = 6.0f;
            orc.maxHeight = 7.4f;
            orc.startingHP = 10;

            orc.attributeIncreases = new List<AttributeModifier>
            {
                new AttributeModifier { attributeType = AttributeType.Strength, bonus = 1 },
                new AttributeModifier { attributeType = AttributeType.Fortitude, bonus = 1 }
            };
            orc.attributeFlaws = new List<AttributeType>();

            orc.knownLanguages = new List<Language> { Language.Basic, Language.Yanalian, Language.Krandalian };
            orc.tags = new List<string> { "Orc", "Humanoid" };
            orc.startingTalentSlots = 1;

            CreateAsset(orc, "Assets/ScriptableObjects/Ancestries/Orc.asset");
        }

        static void CreateDrakeAncestry()
        {
            var drake = ScriptableObject.CreateInstance<AncestryData>();
            drake.ancestryType = AncestryType.Drake;
            drake.ancestryName = "Drake";
            drake.description = "The Drakes are humanoids descended from Dragons. They are a proud race who value strength and order.";
            drake.adulthoodAge = 10;
            drake.maxAge = 150;
            drake.minHeight = 6.5f;
            drake.maxHeight = 7.5f;
            drake.startingHP = 10;

            drake.attributeIncreases = new List<AttributeModifier>
            {
                new AttributeModifier { attributeType = AttributeType.Strength, bonus = 1 },
                new AttributeModifier { attributeType = AttributeType.Wits, bonus = 1 }
            };
            drake.attributeFlaws = new List<AttributeType>();

            drake.knownLanguages = new List<Language> { Language.Basic, Language.Drakonic, Language.Yanalian };
            drake.tags = new List<string> { "Drake", "Humanoid" };
            drake.startingTalentSlots = 1;

            CreateAsset(drake, "Assets/ScriptableObjects/Ancestries/Drake.asset");
        }
        #endregion

        #region Backgrounds
        [MenuItem("MythReal/Create Data/Backgrounds")]
        static void CreateBackgrounds()
        {
            Debug.Log("Creating Background assets...");

            CreateDetectiveBackground();
            CreateSoldierBackground();
            CreateScholarBackground();
            CreateCriminalBackground();
            CreateMedicBackground();
            CreateMerchantBackground();

            Debug.Log("Background assets created!");
        }

        static void CreateDetectiveBackground()
        {
            var detective = ScriptableObject.CreateInstance<BackgroundData>();
            detective.backgroundId = 8;
            detective.backgroundName = "Detective";
            detective.description = "Nothing escapes your keen eye. You piece together mysteries and unravel the truth, no matter how deeply it's buried.";

            detective.attributeIncreaseOption1 = AttributeType.Wits;
            detective.attributeIncreaseOption2 = AttributeType.Intelligence;

            detective.skillProficiency1 = SkillType.Inspect;
            detective.skillProficiency2 = SkillType.Insight;

            detective.additionalLanguages = 1;
            detective.startingNotes = 50;

            detective.typicalBackstory = "You've spent years solving mysteries and uncovering truths. Your keen eye for detail has helped many, but there's always one case that haunts you.";
            detective.suggestedFlaws = new List<string> { "Obsessive about unsolved cases", "Paranoid", "Difficulty trusting others" };
            detective.suggestedIdeals = new List<string> { "Justice must be served", "Truth above all", "Protect the innocent" };
            detective.suggestedBonds = new List<string> { "An unsolved case", "A grateful client", "A former partner" };

            CreateAsset(detective, "Assets/ScriptableObjects/Backgrounds/Detective.asset");
        }

        static void CreateSoldierBackground()
        {
            var soldier = ScriptableObject.CreateInstance<BackgroundData>();
            soldier.backgroundId = 23;
            soldier.backgroundName = "Soldier";
            soldier.description = "Trained for war, you've seen battlefields and bloodshed. Discipline and strategy are ingrained in you, and you stand ever-ready for conflict.";

            soldier.attributeIncreaseOption1 = AttributeType.Strength;
            soldier.attributeIncreaseOption2 = AttributeType.Fortitude;

            soldier.skillProficiency1 = SkillType.Athletics;
            soldier.skillProficiency2 = SkillType.Intimidation;

            soldier.startingNotes = 50;

            soldier.typicalBackstory = "You served in the military or as a mercenary. Combat is second nature to you.";
            soldier.suggestedFlaws = new List<string> { "PTSD from battle", "Overly aggressive", "Follows orders blindly" };
            soldier.suggestedIdeals = new List<string> { "Honor in battle", "Protect the weak", "Follow the chain of command" };
            soldier.suggestedBonds = new List<string> { "Former squad members", "A fallen comrade", "Your commanding officer" };

            CreateAsset(soldier, "Assets/ScriptableObjects/Backgrounds/Soldier.asset");
        }

        static void CreateScholarBackground()
        {
            var scholar = ScriptableObject.CreateInstance<BackgroundData>();
            scholar.backgroundId = 20;
            scholar.backgroundName = "Scholar";
            scholar.description = "Your life is devoted to knowledge. Books and scrolls are your tools, and you seek to uncover the truths hidden in the past.";

            scholar.attributeIncreaseOption1 = AttributeType.Intelligence;
            scholar.attributeIncreaseOption2 = AttributeType.Wits;

            scholar.skillProficiency1 = SkillType.Encyclopedia;
            scholar.skillProficiency2 = SkillType.Essence;

            scholar.additionalLanguages = 2;
            scholar.startingNotes = 50;

            scholar.typicalBackstory = "Years of study have made you an expert in various fields of knowledge.";
            scholar.suggestedFlaws = new List<string> { "Socially awkward", "Lost in thought", "Arrogant about intelligence" };
            scholar.suggestedIdeals = new List<string> { "Knowledge is power", "Share wisdom with others", "Preserve ancient texts" };
            scholar.suggestedBonds = new List<string> { "A mentor", "A rare book", "An academic institution" };

            CreateAsset(scholar, "Assets/ScriptableObjects/Backgrounds/Scholar.asset");
        }

        static void CreateCriminalBackground()
        {
            var criminal = ScriptableObject.CreateInstance<BackgroundData>();
            criminal.backgroundId = 7;
            criminal.backgroundName = "Criminal";
            criminal.description = "Born in the shadows, you live by your own rules. Theft, deception, and cunning are your tools in a world that refuses to play fair.";

            criminal.attributeIncreaseOption1 = AttributeType.Coordination;
            criminal.attributeIncreaseOption2 = AttributeType.Intelligence;

            criminal.skillProficiency1 = SkillType.Deception;
            criminal.skillProficiency2 = SkillType.Stealth;

            criminal.startingNotes = 50;

            criminal.typicalBackstory = "You've lived on the wrong side of the law, using stealth and cunning to survive.";
            criminal.suggestedFlaws = new List<string> { "Compulsive liar", "Greedy", "Paranoid of authorities" };
            criminal.suggestedIdeals = new List<string> { "Freedom above all", "Survival of the fittest", "Look out for yourself" };
            criminal.suggestedBonds = new List<string> { "A criminal gang", "A fence", "Someone you betrayed" };

            CreateAsset(criminal, "Assets/ScriptableObjects/Backgrounds/Criminal.asset");
        }

        static void CreateMedicBackground()
        {
            var medic = ScriptableObject.CreateInstance<BackgroundData>();
            medic.backgroundId = 9;
            medic.backgroundName = "Medic";
            medic.description = "Your knowledge of healing can save lives, even in the most dire of circumstances. You've seen enough suffering to know the value of a steady hand.";

            medic.attributeIncreaseOption1 = AttributeType.Fortitude;
            medic.attributeIncreaseOption2 = AttributeType.Wits;

            medic.skillProficiency1 = SkillType.Medicine;
            medic.skillProficiency2 = SkillType.Encyclopedia;

            medic.startingNotes = 50;

            medic.typicalBackstory = "You've trained in the healing arts and have saved many lives.";
            medic.suggestedFlaws = new List<string> { "Haunted by those you couldn't save", "Overprotective", "Squeamish about violence" };
            medic.suggestedIdeals = new List<string> { "Do no harm", "All life is precious", "Heal the sick" };
            medic.suggestedBonds = new List<string> { "A patient you saved", "Your medical mentor", "A hospital or clinic" };

            CreateAsset(medic, "Assets/ScriptableObjects/Backgrounds/Medic.asset");
        }

        static void CreateMerchantBackground()
        {
            var merchant = ScriptableObject.CreateInstance<BackgroundData>();
            merchant.backgroundId = 16;
            merchant.backgroundName = "Merchant";
            merchant.description = "You've traveled the roads, trading goods and gathering stories. You know the value of a fair deal and the art of negotiation.";

            merchant.attributeIncreaseOption1 = AttributeType.Intelligence;
            merchant.attributeIncreaseOption2 = AttributeType.Sociability;

            merchant.skillProficiency1 = SkillType.Speech;
            merchant.skillProficiency2 = SkillType.Insight;

            merchant.additionalLanguages = 1;
            merchant.startingNotes = 75; // Merchants start with more money

            merchant.typicalBackstory = "You've made your living buying and selling goods across the realm.";
            merchant.suggestedFlaws = new List<string> { "Greedy", "Dishonest in dealings", "Obsessed with profit" };
            merchant.suggestedIdeals = new List<string> { "Fair trade", "Wealth is power", "Build connections" };
            merchant.suggestedBonds = new List<string> { "A trade caravan", "A valuable item", "A business rival" };

            CreateAsset(merchant, "Assets/ScriptableObjects/Backgrounds/Merchant.asset");
        }
        #endregion

        #region Talents
        [MenuItem("MythReal/Create Data/Talents")]
        static void CreateTalents()
        {
            Debug.Log("Creating Talent assets...");

            // Create some key talents
            CreateCounterTalent();
            CreateArmorProficiencyTalent();
            CreateUnravelTalent();
            CreateDodgeTalent();
            CreateSneakAttackFeature();

            Debug.Log("Talent assets created!");
        }

        static void CreateCounterTalent()
        {
            var counter = ScriptableObject.CreateInstance<TalentData>();
            counter.talentId = 62;
            counter.talentName = "Counter";
            counter.description = "You know how to counter enemy attacks";
            counter.effect = "You attack enemies when they miss you";
            counter.minimumLevel = 1;
            counter.source = TalentSource.TechTree;

            CreateAsset(counter, "Assets/ScriptableObjects/Talents/Counter.asset");
        }

        static void CreateArmorProficiencyTalent()
        {
            var armorProf = ScriptableObject.CreateInstance<TalentData>();
            armorProf.talentId = 1;
            armorProf.talentName = "Armor Proficiency";
            armorProf.description = "You know how to use a type of armor";
            armorProf.effect = "Gain proficiency in 1 armor type";
            armorProf.minimumLevel = 1;
            armorProf.source = TalentSource.TechTree;

            CreateAsset(armorProf, "Assets/ScriptableObjects/Talents/ArmorProficiency.asset");
        }

        static void CreateUnravelTalent()
        {
            var unravel = ScriptableObject.CreateInstance<TalentData>();
            unravel.talentId = 25;
            unravel.talentName = "Unravel";
            unravel.description = "Spending some time to think about the matter at hand, you gain perspective";
            unravel.effect = "Gain advantage on Inspect checks when you take time to analyze";
            unravel.minimumLevel = 1;
            unravel.source = TalentSource.Skill;

            unravel.skillBonuses = new List<SkillBonus>
            {
                new SkillBonus { skillType = SkillType.Inspect, bonus = 2 }
            };

            CreateAsset(unravel, "Assets/ScriptableObjects/Talents/Unravel.asset");
        }

        static void CreateDodgeTalent()
        {
            var dodge = ScriptableObject.CreateInstance<TalentData>();
            dodge.talentId = 54;
            dodge.talentName = "Dodge";
            dodge.description = "Once per rest, dodge an incoming melee attack by expending this ability";
            dodge.effect = "Automatically avoid one melee attack per rest";
            dodge.minimumLevel = 1;
            dodge.source = TalentSource.TechTree;

            CreateAsset(dodge, "Assets/ScriptableObjects/Talents/Dodge.asset");
        }

        static void CreateSneakAttackFeature()
        {
            var sneakAttack = ScriptableObject.CreateInstance<TalentData>();
            sneakAttack.talentId = 60;
            sneakAttack.talentName = "Sneak Attack";
            sneakAttack.description = "When you are behind an enemy or they are flanked, you deal an additional 1d6 physical damage to them on any physical move";
            sneakAttack.effect = "+1d6 damage when attacking from behind or when target is flanked";
            sneakAttack.minimumLevel = 1;
            sneakAttack.source = TalentSource.TechTree;
            sneakAttack.specialEffects = new List<string> { "Flanking Bonus", "Backstab Bonus" };

            CreateAsset(sneakAttack, "Assets/ScriptableObjects/Talents/SneakAttack.asset");
        }
        #endregion

        #region Abilities
        [MenuItem("MythReal/Create Data/Abilities")]
        static void CreateAbilities()
        {
            Debug.Log("Creating Ability assets...");

            // Sly tree abilities
            CreateAdrenalineRushAbility();
            CreatePrecisionThrowAbility();
            CreateSuckerPunchAbility();

            // Dual Wielding abilities
            CreateTwinStrikeAbility();
            CreateSwingAndBlockAbility();

            // 1H Weapons
            CreateSlashAbility();
            CreateParryAbility();

            // Magic - Fire
            CreateFireBoltAbility();
            CreateFireballAbility();

            Debug.Log("Ability assets created!");
        }

        static void CreateAdrenalineRushAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Adrenaline Rush";
            ability.abilityLevel = 1;
            ability.description = "Gain 2 AP immediately, but lose 2 AP next turn";
            ability.actionPointCost = 0;
            ability.memoryCost = 1;
            ability.actionType = ActionType.Regular;
            ability.duration = "Instant";
            ability.range = 0;
            ability.areaOfEffect = "Self";
            ability.requiredTechTree = TechTreeType.Sly;
            ability.requiredTechTreeLevel = 1;
            ability.specialProperties = new List<string> { "Gain 2 AP this turn", "Lose 2 AP next turn" };

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/AdrenalineRush.asset");
        }

        static void CreatePrecisionThrowAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Precision Throw";
            ability.abilityLevel = 1;
            ability.description = "Throw a weapon with deadly accuracy";
            ability.actionPointCost = 2;
            ability.memoryCost = 1;
            ability.actionType = ActionType.Regular;
            ability.duration = "Instant";
            ability.range = 9;
            ability.areaOfEffect = "Single target";
            ability.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 };
            ability.damageType = DamageType.Piercing;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = AttributeType.Coordination;
            ability.requiredTechTree = TechTreeType.Sly;
            ability.requiredTechTreeLevel = 1;
            ability.components = new List<ComponentRequirement>
            {
                new ComponentRequirement { componentName = "Throwing knife", consumed = false }
            };

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/PrecisionThrow.asset");
        }

        static void CreateSuckerPunchAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Sucker Punch";
            ability.abilityLevel = 1;
            ability.description = "A quick punch that can daze an opponent";
            ability.actionPointCost = 1;
            ability.memoryCost = 1;
            ability.actionType = ActionType.Reaction;
            ability.duration = "1 turn";
            ability.range = 0;
            ability.areaOfEffect = "Single target in melee";
            ability.requiresSave = true;
            ability.saveType = AttributeType.Coordination;
            ability.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 };
            ability.damageType = DamageType.Blunt;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = AttributeType.Strength;
            ability.requiredTechTree = TechTreeType.Sly;
            ability.requiredTechTreeLevel = 1;
            ability.inflictsStatus = new List<StatusEffect>
            {
                new StatusEffect { condition = StatusCondition.Dazed, duration = 1, saveType = "COR" }
            };

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/SuckerPunch.asset");
        }

        static void CreateTwinStrikeAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Twin Strike";
            ability.abilityLevel = 1;
            ability.description = "Strike using both weapons without an offhand penalty";
            ability.actionPointCost = 2;
            ability.memoryCost = 1;
            ability.actionType = ActionType.Standard;
            ability.duration = "Instant";
            ability.range = 0;
            ability.areaOfEffect = "Single target";
            ability.damageRoll = new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 };
            ability.damageType = DamageType.Slashing;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = AttributeType.Coordination;
            ability.requiredTechTree = TechTreeType.DualWielding;
            ability.requiredTechTreeLevel = 1;
            ability.otherRequirements = new List<string> { "Dual wielding 1h weapons" };
            ability.specialProperties = new List<string> { "Attack twice with no offhand penalty" };

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/TwinStrike.asset");
        }

        static void CreateSwingAndBlockAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Swing 'n Block";
            ability.abilityLevel = 1;
            ability.description = "Swing with your mainhand weapon and ready your other to give you a +1 DEF bonus this turn";
            ability.actionPointCost = 3;
            ability.memoryCost = 1;
            ability.actionType = ActionType.Standard;
            ability.duration = "1 turn";
            ability.range = 0;
            ability.areaOfEffect = "Single target";
            ability.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 };
            ability.damageType = DamageType.Slashing;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = AttributeType.Coordination;
            ability.requiredTechTree = TechTreeType.DualWielding;
            ability.requiredTechTreeLevel = 1;
            ability.specialProperties = new List<string> { "+1 DEF this turn" };

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/SwingAndBlock.asset");
        }

        static void CreateSlashAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Slash";
            ability.abilityLevel = 1;
            ability.description = "Slash enemies in front of you using your 1h weapon";
            ability.actionPointCost = 2;
            ability.memoryCost = 1;
            ability.actionType = ActionType.Standard;
            ability.duration = "Instant";
            ability.range = 0;
            ability.areaOfEffect = "3x1m line in front";
            ability.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 };
            ability.damageType = DamageType.Slashing;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = AttributeType.Strength;
            ability.requiredTechTree = TechTreeType.OneHandedWeapons;
            ability.requiredTechTreeLevel = 1;
            ability.otherRequirements = new List<string> { "1 1h weapon equipped" };

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/Slash.asset");
        }

        static void CreateParryAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Parry";
            ability.abilityLevel = 1;
            ability.description = "Parry an enemy's physical attack";
            ability.actionPointCost = 2;
            ability.memoryCost = 2;
            ability.actionType = ActionType.Reaction;
            ability.duration = "Instant";
            ability.range = 0;
            ability.areaOfEffect = "Self";
            ability.requiredTechTree = TechTreeType.OneHandedWeapons;
            ability.requiredTechTreeLevel = 1;
            ability.otherRequirements = new List<string> { "1 1h weapon equipped" };
            ability.specialProperties = new List<string> { "Negate one physical attack" };

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/Parry.asset");
        }

        static void CreateFireBoltAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Fire Bolt";
            ability.abilityLevel = 1;
            ability.description = "Launch a bolt of fire at a target";
            ability.actionPointCost = 2;
            ability.memoryCost = 1;
            ability.actionType = ActionType.Regular;
            ability.duration = "Instant";
            ability.range = 20;
            ability.areaOfEffect = "Single target";
            ability.damageRoll = new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 };
            ability.damageType = DamageType.Fire;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = AttributeType.Intelligence;
            ability.requiredTechTree = TechTreeType.Fire;
            ability.requiredTechTreeLevel = 1;

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/FireBolt.asset");
        }

        static void CreateFireballAbility()
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            ability.abilityName = "Fireball";
            ability.abilityLevel = 2;
            ability.description = "Hurl an explosive ball of fire";
            ability.actionPointCost = 2;
            ability.memoryCost = 2;
            ability.actionType = ActionType.Regular;
            ability.duration = "Instant";
            ability.range = 20;
            ability.areaOfEffect = "Single target";
            ability.requiresSave = true;
            ability.saveType = AttributeType.Coordination;
            ability.damageRoll = new DiceRoll { numberOfDice = 2, diceSize = 6, modifier = 0 };
            ability.damageType = DamageType.Fire;
            ability.addAttributeModifier = true;
            ability.damageAttributeType = AttributeType.Intelligence;
            ability.requiredTechTree = TechTreeType.Fire;
            ability.requiredTechTreeLevel = 2;
            ability.cooldownTurns = 2;

            CreateAsset(ability, "Assets/ScriptableObjects/Abilities/Fireball.asset");
        }
        #endregion

        #region Items
        [MenuItem("MythReal/Create Data/Items")]
        static void CreateItems()
        {
            Debug.Log("Creating Item assets...");

            // Weapons
            CreateDaggerWeapon();
            CreateShortSwordWeapon();
            CreateLongSwordWeapon();
            CreateShortbowWeapon();

            // Armor
            CreateLeatherArmor();
            CreateChainShirt();
            CreateWoodenShield();

            // Consumables
            CreateHealingPotion();
            CreateManaElixir();

            Debug.Log("Item assets created!");
        }

        static void CreateDaggerWeapon()
        {
            var dagger = ScriptableObject.CreateInstance<WeaponData>();
            dagger.itemName = "Dagger";
            dagger.itemId = 1;
            dagger.description = "A small, light blade perfect for quick strikes";
            dagger.valueInNotes = 5;
            dagger.weight = 1;
            dagger.weaponType = WeaponType.Dagger;
            dagger.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 4, modifier = 0 };
            dagger.damageType = DamageType.Piercing;
            dagger.primaryAttribute = AttributeType.Coordination;
            dagger.isFinesse = true;
            dagger.isThrown = true;
            dagger.throwRange = 20;
            dagger.requiredProficiency = TechTreeType.DualWielding;

            CreateAsset(dagger, "Assets/ScriptableObjects/Items/Weapons/Dagger.asset");
        }

        static void CreateShortSwordWeapon()
        {
            var sword = ScriptableObject.CreateInstance<WeaponData>();
            sword.itemName = "Short Sword";
            sword.itemId = 2;
            sword.description = "A versatile one-handed blade";
            sword.valueInNotes = 10;
            sword.weight = 2;
            sword.weaponType = WeaponType.Sword;
            sword.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 };
            sword.damageType = DamageType.Slashing;
            sword.primaryAttribute = AttributeType.Strength;
            sword.isFinesse = true;
            sword.requiredProficiency = TechTreeType.OneHandedWeapons;

            CreateAsset(sword, "Assets/ScriptableObjects/Items/Weapons/ShortSword.asset");
        }

        static void CreateLongSwordWeapon()
        {
            var sword = ScriptableObject.CreateInstance<WeaponData>();
            sword.itemName = "Long Sword";
            sword.itemId = 3;
            sword.description = "A powerful two-handed blade";
            sword.valueInNotes = 20;
            sword.weight = 3;
            sword.weaponType = WeaponType.Sword;
            sword.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 8, modifier = 0 };
            sword.damageType = DamageType.Slashing;
            sword.primaryAttribute = AttributeType.Strength;
            sword.isTwoHanded = true;
            sword.requiredProficiency = TechTreeType.TwoHandedWeapons;

            CreateAsset(sword, "Assets/ScriptableObjects/Items/Weapons/LongSword.asset");
        }

        static void CreateShortbowWeapon()
        {
            var bow = ScriptableObject.CreateInstance<WeaponData>();
            bow.itemName = "Shortbow";
            bow.itemId = 4;
            bow.description = "A light ranged weapon";
            bow.valueInNotes = 15;
            bow.weight = 2;
            bow.weaponType = WeaponType.Bow;
            bow.damageRoll = new DiceRoll { numberOfDice = 1, diceSize = 6, modifier = 0 };
            bow.damageType = DamageType.Piercing;
            bow.primaryAttribute = AttributeType.Coordination;
            bow.isTwoHanded = true;
            bow.throwRange = 80;
            bow.requiredProficiency = TechTreeType.RangedWeapons;

            CreateAsset(bow, "Assets/ScriptableObjects/Items/Weapons/Shortbow.asset");
        }

        static void CreateLeatherArmor()
        {
            var armor = ScriptableObject.CreateInstance<ArmorData>();
            armor.itemName = "Leather Armor";
            armor.itemId = 10;
            armor.description = "Light armor that offers decent protection without sacrificing mobility";
            armor.valueInNotes = 10;
            armor.weight = 10;
            armor.armorType = ArmorType.Light;
            armor.equipmentSlot = EquipmentSlot.Chest;
            armor.armorClass = 2;

            CreateAsset(armor, "Assets/ScriptableObjects/Items/Armor/LeatherArmor.asset");
        }

        static void CreateChainShirt()
        {
            var armor = ScriptableObject.CreateInstance<ArmorData>();
            armor.itemName = "Chain Shirt";
            armor.itemId = 11;
            armor.description = "Medium armor made of interlocking rings";
            armor.valueInNotes = 50;
            armor.weight = 20;
            armor.armorType = ArmorType.Medium;
            armor.equipmentSlot = EquipmentSlot.Chest;
            armor.armorClass = 4;
            armor.speedPenalty = 1;

            CreateAsset(armor, "Assets/ScriptableObjects/Items/Armor/ChainShirt.asset");
        }

        static void CreateWoodenShield()
        {
            var shield = ScriptableObject.CreateInstance<ArmorData>();
            shield.itemName = "Wooden Shield";
            shield.itemId = 12;
            shield.description = "A basic shield for defense";
            shield.valueInNotes = 5;
            shield.weight = 6;
            shield.armorType = ArmorType.Shield;
            shield.equipmentSlot = EquipmentSlot.OffHand;
            shield.armorClass = 2;

            CreateAsset(shield, "Assets/ScriptableObjects/Items/Armor/WoodenShield.asset");
        }

        static void CreateHealingPotion()
        {
            var potion = ScriptableObject.CreateInstance<ConsumableData>();
            potion.itemName = "Healing Potion";
            potion.itemId = 20;
            potion.description = "A basic potion that restores health";
            potion.valueInNotes = 25;
            potion.weight = 0.5f;
            potion.isConsumable = true;
            potion.maxStackSize = 10;
            potion.healingAmount = new DiceRoll { numberOfDice = 2, diceSize = 4, modifier = 2 };
            potion.actionPointCost = 1;

            CreateAsset(potion, "Assets/ScriptableObjects/Items/Consumables/HealingPotion.asset");
        }

        static void CreateManaElixir()
        {
            var elixir = ScriptableObject.CreateInstance<ConsumableData>();
            elixir.itemName = "Mana Elixir";
            elixir.itemId = 21;
            elixir.description = "Restores magical energy";
            elixir.valueInNotes = 30;
            elixir.weight = 0.5f;
            elixir.isConsumable = true;
            elixir.maxStackSize = 10;
            elixir.actionPointCost = 1;
            elixir.specialProperties = new List<string> { "Restore 1d4 memory slots for one combat" };

            CreateAsset(elixir, "Assets/ScriptableObjects/Items/Consumables/ManaElixir.asset");
        }
        #endregion

        #region Tech Trees
        [MenuItem("MythReal/Create Data/Tech Trees")]
        static void CreateTechTrees()
        {
            Debug.Log("Creating Tech Tree assets...");

            CreateSlyTechTree();
            CreateDualWieldingTechTree();

            Debug.Log("Tech Tree assets created!");
        }

        static void CreateSlyTechTree()
        {
            var sly = ScriptableObject.CreateInstance<TechTreeData>();
            sly.techTreeType = TechTreeType.Sly;
            sly.techTreeName = "Sly";
            sly.description = "The Sly Tech Tree caters to those who prefer subtlety over brute force. It includes skills in stealth, thievery, and deception.";

            sly.levels = new List<TechTreeLevel>
            {
                new TechTreeLevel
                {
                    level = 1,
                    abilitiesUnlocked = new List<AbilityData>(),
                    featuresUnlocked = new List<TalentData>(),
                    talentsAvailable = new List<TalentData>()
                }
            };

            CreateAsset(sly, "Assets/ScriptableObjects/TechTrees/Sly.asset");
        }

        static void CreateDualWieldingTechTree()
        {
            var dualWield = ScriptableObject.CreateInstance<TechTreeData>();
            dualWield.techTreeType = TechTreeType.DualWielding;
            dualWield.techTreeName = "Dual Wielding";
            dualWield.description = "Dual Wielding is the art of fighting with a weapon in each hand. This Tech Tree focuses on the agility and flurry of attacks this style allows.";

            dualWield.levels = new List<TechTreeLevel>
            {
                new TechTreeLevel
                {
                    level = 1,
                    abilitiesUnlocked = new List<AbilityData>(),
                    featuresUnlocked = new List<TalentData>(),
                    talentsAvailable = new List<TalentData>()
                }
            };

            CreateAsset(dualWield, "Assets/ScriptableObjects/TechTrees/DualWielding.asset");
        }
        #endregion

        #region Utility
        static void CreateAsset(ScriptableObject asset, string path)
        {
            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Create or overwrite the asset
            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"Created: {path}");
        }
        #endregion
    }
}
