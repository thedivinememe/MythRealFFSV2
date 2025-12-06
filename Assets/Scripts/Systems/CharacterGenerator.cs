using System;
using System.Collections.Generic;
using UnityEngine;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;

namespace MythRealFFSV2.Systems
{
    /// <summary>
    /// Utility class for generating characters
    /// Handles attribute rolling, character creation, and initialization
    /// </summary>
    public class CharacterGenerator : MonoBehaviour
    {
        [Header("Standard Array")]
        [Tooltip("The standard attribute array: [16, 15, 14, 12, 11, 10, 8]")]
        public int[] standardArray = { 16, 15, 14, 12, 11, 10, 8 };

        /// <summary>
        /// Create a new character with random attributes (4d6 drop lowest)
        /// </summary>
        public CharacterData CreateCharacter(
            string name,
            AncestryData ancestry,
            BackgroundData background,
            AttributeType chosenBackgroundAttribute,
            bool useStandardArray = true)
        {
            CharacterData character = new CharacterData();
            character.characterName = name;

            // Initialize with ancestry and background
            character.Initialize(ancestry, background, chosenBackgroundAttribute);

            // If not using standard array, roll attributes
            if (!useStandardArray)
            {
                RollAttributes(character);
            }

            return character;
        }

        /// <summary>
        /// Roll 4d6 drop lowest for each attribute
        /// </summary>
        public int[] RollAttributeArray()
        {
            int[] rolls = new int[7]; // 7 attributes

            for (int i = 0; i < 7; i++)
            {
                rolls[i] = Roll4d6DropLowest();
            }

            return rolls;
        }

        /// <summary>
        /// Roll 4d6 and drop the lowest die
        /// </summary>
        private int Roll4d6DropLowest()
        {
            List<int> rolls = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                rolls.Add(UnityEngine.Random.Range(1, 7));
            }

            // Sort and remove lowest
            rolls.Sort();
            rolls.RemoveAt(0);

            // Sum the remaining 3 dice
            return rolls[0] + rolls[1] + rolls[2];
        }

        /// <summary>
        /// Apply rolled attributes to character
        /// </summary>
        private void RollAttributes(CharacterData character)
        {
            int[] rolls = RollAttributeArray();

            int index = 0;
            foreach (AttributeType attrType in Enum.GetValues(typeof(AttributeType)))
            {
                if (index < rolls.Length)
                {
                    character.attributes.SetBaseValue(attrType, rolls[index]);
                    index++;
                }
            }

            // Recalculate derived stats
            character.CalculateMaxHP();
            character.CalculateDefense();
            character.CalculateInitiative();
        }

        /// <summary>
        /// Assign attributes manually
        /// </summary>
        public void AssignAttributes(CharacterData character, Dictionary<AttributeType, int> attributeValues)
        {
            foreach (var kvp in attributeValues)
            {
                character.attributes.SetBaseValue(kvp.Key, kvp.Value);
            }

            // Recalculate derived stats
            character.CalculateMaxHP();
            character.CalculateDefense();
            character.CalculateInitiative();
        }

        /// <summary>
        /// Create a fully random character
        /// </summary>
        public CharacterData CreateRandomCharacter(
            List<AncestryData> availableAncestries,
            List<BackgroundData> availableBackgrounds)
        {
            if (availableAncestries == null || availableBackgrounds == null)
            {
                Debug.LogError("Ancestry or background list is null!");
                return null;
            }

            if (availableAncestries.Count == 0 || availableBackgrounds.Count == 0)
            {
                Debug.LogError("Need at least one ancestry and background to create random character");
                return null;
            }

            // Random ancestry and background
            AncestryData randomAncestry = availableAncestries[UnityEngine.Random.Range(0, availableAncestries.Count)];
            BackgroundData randomBackground = availableBackgrounds[UnityEngine.Random.Range(0, availableBackgrounds.Count)];

            if (randomAncestry == null)
            {
                Debug.LogError("Selected ancestry is null! Check your ScriptableObject assets.");
                return null;
            }

            if (randomBackground == null)
            {
                Debug.LogError("Selected background is null! Check your ScriptableObject assets.");
                return null;
            }

            // Random attribute choice for background
            AttributeType chosenAttribute = UnityEngine.Random.Range(0, 2) == 0
                ? randomBackground.attributeIncreaseOption1
                : randomBackground.attributeIncreaseOption2;

            // Random name
            string randomName = GenerateRandomName();

            return CreateCharacter(randomName, randomAncestry, randomBackground, chosenAttribute, useStandardArray: false);
        }

        /// <summary>
        /// Generate a random character name
        /// </summary>
        private string GenerateRandomName()
        {
            string[] prefixes = { "Kar", "El", "Bro", "Thal", "Gor", "Zal", "Mor", "Fen", "Dra", "Kro" };
            string[] suffixes = { "el", "os", "is", "ar", "on", "ak", "us", "ir", "or", "an" };

            string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
            string suffix = suffixes[UnityEngine.Random.Range(0, suffixes.Length)];

            return prefix + suffix;
        }

        /// <summary>
        /// Create a character from a template/preset
        /// </summary>
        public CharacterData CreateFromTemplate(CharacterTemplate template)
        {
            CharacterData character = CreateCharacter(
                template.characterName,
                template.ancestry,
                template.background,
                template.chosenBackgroundAttribute,
                template.useStandardArray
            );

            // Assign specific attributes if provided
            if (template.attributeAssignments != null && template.attributeAssignments.Count > 0)
            {
                AssignAttributes(character, template.attributeAssignments);
            }

            // Set bio information
            character.sex = template.sex;
            character.age = template.age;
            character.height = template.height;
            character.weight = template.weight;
            character.alignment = template.alignment;
            character.catchphrase = template.catchphrase;
            character.backstory = template.backstory;

            if (template.flaws != null)
                character.flaws = new List<string>(template.flaws);
            if (template.bonds != null)
                character.bonds = new List<string>(template.bonds);
            if (template.ideals != null)
                character.ideals = new List<string>(template.ideals);

            // Set primary and secondary attributes
            character.primaryAttribute = template.primaryAttribute;
            character.secondaryAttribute = template.secondaryAttribute;

            return character;
        }
    }

    /// <summary>
    /// Template for creating pre-configured characters
    /// </summary>
    [System.Serializable]
    public class CharacterTemplate
    {
        public string characterName;
        public SexType sex;
        public int age;
        public float height;
        public float weight;
        public AlignmentType alignment;
        public string catchphrase;
        public string backstory;

        public AncestryData ancestry;
        public BackgroundData background;
        public AttributeType chosenBackgroundAttribute;

        public bool useStandardArray = true;
        public Dictionary<AttributeType, int> attributeAssignments;

        public AttributeType primaryAttribute;
        public AttributeType secondaryAttribute;

        public List<string> flaws;
        public List<string> bonds;
        public List<string> ideals;
    }
}
