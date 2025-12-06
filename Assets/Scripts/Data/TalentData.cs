using System.Collections.Generic;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Data
{
    /// <summary>
    /// ScriptableObject that defines a talent (feat)
    /// Talents provide special capabilities and bonuses to characters
    /// </summary>
    [CreateAssetMenu(fileName = "New Talent", menuName = "MythReal/Character/Talent")]
    public class TalentData : ScriptableObject
    {
        [Header("Basic Information")]
        public int talentId;
        public string talentName;
        [TextArea(2, 5)]
        public string description;
        [TextArea(2, 5)]
        public string effect;

        [Header("Requirements")]
        public int minimumLevel = 1;
        public List<AttributeRequirement> attributeRequirements = new List<AttributeRequirement>();
        public List<SkillRequirement> skillRequirements = new List<SkillRequirement>();
        public TalentSource source;

        [Header("Effects")]
        public List<AttributeModifier> attributeBonuses = new List<AttributeModifier>();
        public List<SkillBonus> skillBonuses = new List<SkillBonus>();
        public List<AbilityData> grantsAbilities = new List<AbilityData>();
        public List<string> specialEffects = new List<string>(); // For custom/unique effects

        /// <summary>
        /// Check if character meets requirements for this talent
        /// </summary>
        public bool MeetsRequirements(CharacterData character)
        {
            if (character.level < minimumLevel)
                return false;

            foreach (var req in attributeRequirements)
            {
                if (character.attributes.GetValue(req.attributeType) < req.minimumValue)
                    return false;
            }

            foreach (var req in skillRequirements)
            {
                if (character.GetSkillValue(req.skillType) < req.minimumValue)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Apply talent effects to character
        /// </summary>
        public void ApplyEffects(CharacterData character)
        {
            // Apply attribute bonuses
            foreach (var bonus in attributeBonuses)
            {
                character.attributes.ApplyBonus(bonus.attributeType, bonus.bonus);
            }

            // Apply skill bonuses
            foreach (var bonus in skillBonuses)
            {
                character.GetSkill(bonus.skillType).AddBonus(bonus.bonus);
            }

            // Grant abilities
            foreach (var ability in grantsAbilities)
            {
                character.LearnAbility(ability);
            }
        }
    }

    [System.Serializable]
    public class AttributeRequirement
    {
        public AttributeType attributeType;
        public int minimumValue;
    }

    [System.Serializable]
    public class SkillRequirement
    {
        public SkillType skillType;
        public int minimumValue;
    }

    [System.Serializable]
    public class SkillBonus
    {
        public SkillType skillType;
        public int bonus;
    }

    public enum TalentSource
    {
        Ancestry,
        Background,
        TechTree,
        Skill,
        General
    }
}
