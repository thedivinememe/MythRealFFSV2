using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MythRealFFSV2.Data;

namespace MythRealFFSV2.Character
{
    /// <summary>
    /// Main character data class that represents a playable character
    /// Handles all character stats, progression, and abilities
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        #region Basic Information
        [Header("Basic Information")]
        public string characterName;
        public SexType sex;
        public float height; // in feet
        public float weight; // in pounds
        public int age;
        public AlignmentType alignment;

        public AncestryData ancestry;
        public BackgroundData background;
        #endregion

        #region Character Identity
        [Header("Character Identity")]
        [TextArea(2, 5)]
        public string backstory;
        public List<string> flaws = new List<string>();
        public List<string> bonds = new List<string>();
        public List<string> ideals = new List<string>();
        public string philosophyReligion;
        public string catchphrase;
        #endregion

        #region Core Stats
        [Header("Core Stats")]
        public int level = 1;
        public int experiencePoints;
        public int experienceToNextLevel = 1000;

        public CharacterAttributes attributes;
        private Dictionary<SkillType, Skill> skills;
        public List<Language> knownLanguages = new List<Language>();
        #endregion

        #region Combat Stats
        [Header("Combat Stats")]
        public int currentHP;
        public int maxHP;
        public int currentAP = 5;
        public int maxAP = 5;
        public int bankedAP = 0; // Can bank up to 2 AP
        public int defense;
        public int initiative;
        public int speed = 2; // meters per AP
        public int criticalHitThreshold = 20; // Natural 20 is a crit
        #endregion

        #region Special Attributes
        [Header("Special Attributes")]
        public int luck;
        public int memory; // Determines how many abilities can be known
        public int proficiencyBonus;
        #endregion

        #region Primary/Secondary Attributes
        [Header("Primary & Secondary Attributes")]
        public AttributeType primaryAttribute;
        public AttributeType secondaryAttribute;
        #endregion

        #region Progression
        [Header("Progression")]
        public int techPoints = 2;
        public int availableTechPoints = 2;
        public Dictionary<TechTreeType, int> techTreeLevels = new Dictionary<TechTreeType, int>();
        #endregion

        #region Abilities and Talents
        [Header("Abilities and Talents")]
        public List<AbilityData> knownAbilities = new List<AbilityData>();
        public List<TalentData> talents = new List<TalentData>();
        public int usedMemorySlots = 0;
        #endregion

        #region Equipment
        [Header("Equipment")]
        public Dictionary<EquipmentSlot, ItemData> equippedItems = new Dictionary<EquipmentSlot, ItemData>();
        public List<ItemData> inventory = new List<ItemData>();
        public int notes = 50; // Currency
        #endregion

        #region Status Effects
        [Header("Status Effects")]
        public List<ActiveStatusEffect> activeStatusEffects = new List<ActiveStatusEffect>();
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize a new character with ancestry and background
        /// </summary>
        public void Initialize(AncestryData ancestryData, BackgroundData backgroundData,
                              AttributeType chosenBackgroundAttribute)
        {
            this.ancestry = ancestryData;
            this.background = backgroundData;

            // Initialize attributes
            attributes = new CharacterAttributes();

            // Roll or assign attributes (using standard array for now)
            AssignStandardArray();

            // Apply ancestry bonuses
            ancestry.ApplyAttributeBonuses(attributes);

            // Initialize skills (must be done before background bonuses)
            InitializeSkills();

            // Apply background bonuses
            background.ApplyBackgroundBonuses(this, chosenBackgroundAttribute);

            // Set starting HP
            CalculateMaxHP();
            currentHP = maxHP;

            // Calculate defense
            CalculateDefense();

            // Initialize languages
            knownLanguages.AddRange(ancestry.knownLanguages);

            // Initialize tech trees
            foreach (TechTreeType techType in Enum.GetValues(typeof(TechTreeType)))
            {
                techTreeLevels[techType] = 0;
            }

            // Initialize equipment slots
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                equippedItems[slot] = null;
            }

            // Calculate memory
            CalculateMemory();

            // Initialize starting equipment
            if (background != null && background.startingEquipment != null)
            {
                inventory.AddRange(background.startingEquipment);
            }

            notes = background != null ? background.startingNotes : 50;
        }

        private void AssignStandardArray()
        {
            // Standard array: [16, 15, 14, 12, 11, 10, 8]
            // For now, assigning in order - should be customizable
            int[] standardArray = { 16, 15, 14, 12, 11, 10, 8 };
            int index = 0;

            foreach (AttributeType attrType in Enum.GetValues(typeof(AttributeType)))
            {
                if (index < standardArray.Length)
                {
                    attributes.SetBaseValue(attrType, standardArray[index]);
                    index++;
                }
            }
        }

        private void InitializeSkills()
        {
            skills = new Dictionary<SkillType, Skill>();
            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                skills[skillType] = new Skill(skillType, attributes);
            }
        }
        #endregion

        #region Stat Calculations
        /// <summary>
        /// Calculate maximum HP based on ancestry and attributes
        /// </summary>
        public void CalculateMaxHP()
        {
            int baseHP = ancestry != null ? ancestry.startingHP : 8; // Default to 8 if no ancestry
            int fortitudeBonus = attributes != null ? attributes.GetModifier(AttributeType.Fortitude) * level : 0;
            maxHP = baseHP + fortitudeBonus;

            // Ensure minimum HP
            if (maxHP < 1)
                maxHP = 1;
        }

        /// <summary>
        /// Calculate defense: 10 + ((STR || COR) + WIT)/2 + Armor
        /// </summary>
        public void CalculateDefense()
        {
            if (attributes == null)
            {
                defense = 10;
                return;
            }

            int strMod = attributes.GetModifier(AttributeType.Strength);
            int corMod = attributes.GetModifier(AttributeType.Coordination);
            int witMod = attributes.GetModifier(AttributeType.Wits);

            int baseDefense = 10 + ((Math.Max(strMod, corMod) + witMod) / 2);

            // Add armor bonus
            int armorBonus = GetArmorBonus();

            defense = baseDefense + armorBonus;
        }

        /// <summary>
        /// Calculate initiative modifier
        /// </summary>
        public void CalculateInitiative()
        {
            if (attributes == null)
            {
                initiative = 0;
                return;
            }

            // Initiative is typically based on Coordination and Wits
            initiative = (attributes.GetModifier(AttributeType.Coordination) +
                         attributes.GetModifier(AttributeType.Wits)) / 2;
        }

        /// <summary>
        /// Calculate memory (ability slots) based on INT
        /// </summary>
        public void CalculateMemory()
        {
            if (attributes == null)
            {
                memory = 4;
                return;
            }

            int intMod = attributes.GetModifier(AttributeType.Intelligence);
            memory = 4 + intMod; // Start with 4 at level 1

            // Ensure minimum
            if (memory < 2)
                memory = 2;
        }

        private int GetArmorBonus()
        {
            int bonus = 0;

            foreach (var item in equippedItems.Values)
            {
                if (item is ArmorData armorData)
                {
                    bonus += armorData.armorClass;
                }
            }

            return bonus;
        }
        #endregion

        #region Skills
        public Skill GetSkill(SkillType skillType)
        {
            return skills[skillType];
        }

        public int GetSkillValue(SkillType skillType)
        {
            return skills[skillType].Value;
        }

        public void AddSkillProficiency(SkillType skillType)
        {
            skills[skillType].AddProficiency();
        }

        public Dictionary<SkillType, Skill> GetAllSkills()
        {
            return skills;
        }
        #endregion

        #region Abilities
        public void LearnAbility(AbilityData ability)
        {
            if (usedMemorySlots + ability.memoryCost <= memory)
            {
                knownAbilities.Add(ability);
                usedMemorySlots += ability.memoryCost;
            }
            else
            {
                Debug.LogWarning($"Not enough memory slots to learn {ability.abilityName}");
            }
        }

        public void ForgetAbility(AbilityData ability)
        {
            if (knownAbilities.Contains(ability))
            {
                knownAbilities.Remove(ability);
                usedMemorySlots -= ability.memoryCost;
            }
        }

        public bool HasAbility(AbilityData ability)
        {
            return knownAbilities.Contains(ability);
        }

        public bool CanLearnAbility(AbilityData ability)
        {
            return (usedMemorySlots + ability.memoryCost) <= memory;
        }
        #endregion

        #region Talents
        public void AddTalent(TalentData talent)
        {
            if (!talents.Contains(talent) && talent.MeetsRequirements(this))
            {
                talents.Add(talent);
                talent.ApplyEffects(this);
            }
        }

        public bool HasTalent(TalentData talent)
        {
            return talents.Contains(talent);
        }
        #endregion

        #region Tech Trees
        public void InvestTechPoint(TechTreeType techTree)
        {
            if (availableTechPoints > 0)
            {
                techTreeLevels[techTree]++;
                availableTechPoints--;
            }
        }

        public int GetTechTreeLevel(TechTreeType techTree)
        {
            return techTreeLevels[techTree];
        }
        #endregion

        #region Combat
        public void StartTurn()
        {
            currentAP = maxAP;

            // Add banked AP (max 2)
            if (bankedAP > 0)
            {
                currentAP += Math.Min(bankedAP, 2);
                bankedAP = 0;
            }

            // Process status effects
            ProcessStatusEffects();
        }

        public void EndTurn()
        {
            // Bank AP (up to 2)
            if (currentAP > 0)
            {
                bankedAP = Math.Min(currentAP, 2);
            }
        }

        public bool SpendAP(int amount)
        {
            if (currentAP >= amount)
            {
                currentAP -= amount;
                return true;
            }
            return false;
        }

        public void TakeDamage(int damage)
        {
            currentHP -= damage;
            if (currentHP < 0)
                currentHP = 0;
        }

        public void Heal(int amount)
        {
            currentHP += amount;
            if (currentHP > maxHP)
                currentHP = maxHP;
        }

        public bool IsAlive()
        {
            return currentHP > 0;
        }

        public bool IsUnconscious()
        {
            return currentHP <= 0;
        }

        private void ProcessStatusEffects()
        {
            for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeStatusEffects[i];
                effect.remainingDuration--;

                // Apply any per-turn damage/effects
                ApplyStatusEffectPerTurn(effect);

                // Remove if expired
                if (effect.remainingDuration <= 0)
                {
                    activeStatusEffects.RemoveAt(i);
                }
            }
        }

        private void ApplyStatusEffectPerTurn(ActiveStatusEffect effect)
        {
            switch (effect.condition)
            {
                case StatusCondition.Poisoned:
                    TakeDamage(UnityEngine.Random.Range(1, 5)); // 1d4 poison damage
                    break;
                case StatusCondition.Burning:
                    TakeDamage(UnityEngine.Random.Range(1, 7)); // 1d6 fire damage
                    break;
                    // Add more status effect logic as needed
            }
        }

        public void AddStatusEffect(StatusCondition condition, int duration)
        {
            activeStatusEffects.Add(new ActiveStatusEffect
            {
                condition = condition,
                remainingDuration = duration
            });
        }

        public bool HasStatusEffect(StatusCondition condition)
        {
            return activeStatusEffects.Any(e => e.condition == condition);
        }
        #endregion

        #region Leveling
        public void GainExperience(int xp)
        {
            experiencePoints += xp;

            while (experiencePoints >= experienceToNextLevel)
            {
                LevelUp();
            }
        }

        public void LevelUp()
        {
            level++;
            experiencePoints -= experienceToNextLevel;

            // Grant tech point
            availableTechPoints++;
            techPoints++;

            // Recalculate stats
            CalculateMaxHP();
            CalculateDefense();
            CalculateMemory();

            // Fully heal on level up
            currentHP = maxHP;

            Debug.Log($"{characterName} leveled up to level {level}!");
        }
        #endregion

        #region Equipment
        public void EquipItem(ItemData item, EquipmentSlot slot)
        {
            // Unequip current item if any
            if (equippedItems[slot] != null)
            {
                inventory.Add(equippedItems[slot]);
            }

            equippedItems[slot] = item;
            inventory.Remove(item);

            // Recalculate stats
            CalculateDefense();
        }

        public void UnequipItem(EquipmentSlot slot)
        {
            if (equippedItems[slot] != null)
            {
                inventory.Add(equippedItems[slot]);
                equippedItems[slot] = null;

                // Recalculate stats
                CalculateDefense();
            }
        }
        #endregion

        #region Utility
        public override string ToString()
        {
            return $"{characterName} - Level {level} {ancestry.ancestryName} {background.backgroundName}\n" +
                   $"HP: {currentHP}/{maxHP} | DEF: {defense} | AP: {currentAP}/{maxAP}";
        }

        public string GetDetailedStats()
        {
            string stats = $"=== {characterName} ===\n";
            stats += $"Level {level} {ancestry.ancestryName} {background.backgroundName}\n\n";

            stats += "ATTRIBUTES:\n";
            foreach (var attr in attributes.GetAllAttributes())
            {
                stats += $"  {attr.Value}\n";
            }

            stats += "\nCOMBAT STATS:\n";
            stats += $"  HP: {currentHP}/{maxHP}\n";
            stats += $"  Defense: {defense}\n";
            stats += $"  Initiative: {initiative}\n";
            stats += $"  Speed: {speed}m per AP\n";

            stats += "\nSKILLS:\n";
            foreach (var skill in skills.Values)
            {
                stats += $"  {skill}\n";
            }

            stats += $"\nAbilities Known: {knownAbilities.Count} (Memory: {usedMemorySlots}/{memory})\n";
            stats += $"Talents: {talents.Count}\n";
            stats += $"Tech Points Available: {availableTechPoints}\n";

            return stats;
        }
        #endregion
    }

    [Serializable]
    public class ActiveStatusEffect
    {
        public StatusCondition condition;
        public int remainingDuration;
    }
}
