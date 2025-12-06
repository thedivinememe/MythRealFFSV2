using System.Collections.Generic;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Data
{
    /// <summary>
    /// ScriptableObject that defines an ancestry (race)
    /// Used to configure the four ancestries: Human, Elf, Orc, Drake
    /// </summary>
    [CreateAssetMenu(fileName = "New Ancestry", menuName = "MythReal/Character/Ancestry")]
    public class AncestryData : ScriptableObject
    {
        [Header("Basic Information")]
        public AncestryType ancestryType;
        public string ancestryName;
        [TextArea(3, 6)]
        public string description;

        [Header("Age Information")]
        public int adulthoodAge;
        public int maxAge; // -1 for immortal (Elves)

        [Header("Physical Traits")]
        public float minHeight; // in feet
        public float maxHeight; // in feet

        [Header("Starting Stats")]
        public int startingHP;

        [Header("Attribute Modifiers")]
        public List<AttributeModifier> attributeIncreases = new List<AttributeModifier>();
        public List<AttributeType> attributeFlaws = new List<AttributeType>();

        [Header("Languages")]
        public List<Language> knownLanguages = new List<Language>();

        [Header("Tags")]
        public List<string> tags = new List<string>();

        [Header("Starting Talents")]
        public int startingTalentSlots = 1;
        public List<TalentData> ancestryTalents = new List<TalentData>();

        /// <summary>
        /// Get the total HP modifier from attributes for this ancestry
        /// </summary>
        public int GetHPModifier(CharacterAttributes attributes)
        {
            // HP calculation can be customized per ancestry if needed
            // For now, using FRT modifier
            return attributes.GetModifier(AttributeType.Fortitude);
        }

        /// <summary>
        /// Apply ancestry bonuses to character attributes
        /// </summary>
        public void ApplyAttributeBonuses(CharacterAttributes attributes)
        {
            foreach (var modifier in attributeIncreases)
            {
                attributes.ApplyBonus(modifier.attributeType, modifier.bonus);
            }

            foreach (var flaw in attributeFlaws)
            {
                attributes.ApplyBonus(flaw, -1);
            }
        }
    }

    [System.Serializable]
    public class AttributeModifier
    {
        public AttributeType attributeType;
        public int bonus = 1;
    }
}
