using UnityEngine;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Combat;
using System.Collections.Generic;

namespace MythRealFFSV2.Examples
{
    /// <summary>
    /// Example script demonstrating how to use the character system
    /// This shows character creation, combat simulation, and stat tracking
    /// </summary>
    public class CharacterSystemExample : MonoBehaviour
    {
        [Header("Character Data References")]
        [Tooltip("Assign ScriptableObject assets for Ancestries")]
        public List<AncestryData> ancestries;

        [Tooltip("Assign ScriptableObject assets for Backgrounds")]
        public List<BackgroundData> backgrounds;

        [Header("Created Characters")]
        public List<CharacterData> playerTeam = new List<CharacterData>();
        public List<CharacterData> enemyTeam = new List<CharacterData>();

        private CharacterGenerator generator;
        private CombatManager combatManager;

        void Start()
        {
            // Get or create required components
            generator = GetComponent<CharacterGenerator>();
            if (generator == null)
                generator = gameObject.AddComponent<CharacterGenerator>();

            combatManager = GetComponent<CombatManager>();
            if (combatManager == null)
                combatManager = gameObject.AddComponent<CombatManager>();

            // Run example
            ExampleCharacterCreation();
        }

        /// <summary>
        /// Example: Create characters like Karr-El from the rulebook
        /// </summary>
        void ExampleCharacterCreation()
        {
            Debug.Log("=== Character Creation Example ===\n");

            // Example 1: Create Karr-El the Orc Detective
            CharacterData karrEl = CreateKarrEl();
            playerTeam.Add(karrEl);

            Debug.Log(karrEl.GetDetailedStats());
            Debug.Log("\n");

            // Example 2: Create a random character
            if (ancestries.Count > 0 && backgrounds.Count > 0)
            {
                CharacterData randomChar = generator.CreateRandomCharacter(ancestries, backgrounds);
                if (randomChar != null)
                {
                    enemyTeam.Add(randomChar);
                    Debug.Log("Created random character:");
                    Debug.Log(randomChar.GetDetailedStats());
                }
            }

            // Example 3: Simulate a simple combat
            if (playerTeam.Count > 0 && enemyTeam.Count > 0)
            {
                Debug.Log("\n=== Starting Combat Simulation ===\n");
                SimulateCombat();
            }
        }

        /// <summary>
        /// Create Karr-El as described in the rulebook example
        /// </summary>
        CharacterData CreateKarrEl()
        {
            // Find Orc ancestry (you'll need to create this ScriptableObject asset)
            AncestryData orcAncestry = ancestries.Find(a => a.ancestryType == AncestryType.Orc);
            BackgroundData detectiveBackground = backgrounds.Find(b => b.backgroundName == "Detective");

            if (orcAncestry == null || detectiveBackground == null)
            {
                Debug.LogWarning("Missing required ScriptableObject assets. Creating character with placeholders.");
                // For demonstration purposes, we'll create a basic character
                return CreatePlaceholderCharacter();
            }

            // Create character template
            CharacterTemplate template = new CharacterTemplate
            {
                characterName = "Karr-El",
                sex = SexType.Male,
                age = 28,
                height = 7.0f,
                weight = 280f,
                alignment = AlignmentType.LawfulGood,
                catchphrase = "This case is cracked wide open!",
                backstory = "A detective who's been chasing the dead case of a missing child for years before arriving in Yanala.",

                ancestry = orcAncestry,
                background = detectiveBackground,
                chosenBackgroundAttribute = AttributeType.Wits,

                useStandardArray = false,
                primaryAttribute = AttributeType.Wits,
                secondaryAttribute = AttributeType.Strength
            };

            // Assign the rolled attributes from the example
            template.attributeAssignments = new Dictionary<AttributeType, int>
            {
                { AttributeType.Wits, 12 },        // 11 + 1 from background
                { AttributeType.Coordination, 15 },
                { AttributeType.Faith, 9 },
                { AttributeType.Fortitude, 15 },   // 14 + 1 from Orc
                { AttributeType.Intelligence, 13 },
                { AttributeType.Sociability, 13 },
                { AttributeType.Strength, 11 }     // 10 + 1 from Orc
            };

            template.flaws = new List<string> { "Obsessive about unsolved cases" };
            template.bonds = new List<string> { "The missing child's family" };
            template.ideals = new List<string> { "Justice must be served" };

            CharacterData karrEl = generator.CreateFromTemplate(template);

            // Add some starting abilities based on tech points
            // Karr-El put 1 point in Sly and 1 point in Dual Wielding
            karrEl.InvestTechPoint(TechTreeType.Sly);
            karrEl.InvestTechPoint(TechTreeType.DualWielding);

            return karrEl;
        }

        /// <summary>
        /// Create a placeholder character for demonstration
        /// </summary>
        CharacterData CreatePlaceholderCharacter()
        {
            CharacterData character = new CharacterData
            {
                characterName = "Test Hero",
                level = 1,
                sex = SexType.Male,
                age = 25,
                height = 6.0f,
                weight = 180f,
                alignment = AlignmentType.NeutralGood
            };

            // Initialize basic stats
            character.attributes = new CharacterAttributes();
            character.attributes.SetBaseValue(AttributeType.Strength, 14);
            character.attributes.SetBaseValue(AttributeType.Coordination, 12);
            character.attributes.SetBaseValue(AttributeType.Fortitude, 13);
            character.attributes.SetBaseValue(AttributeType.Intelligence, 10);
            character.attributes.SetBaseValue(AttributeType.Wits, 11);
            character.attributes.SetBaseValue(AttributeType.Sociability, 10);
            character.attributes.SetBaseValue(AttributeType.Faith, 8);

            character.maxHP = 10;
            character.currentHP = 10;
            character.defense = 12;

            return character;
        }

        /// <summary>
        /// Simulate a simple combat encounter
        /// </summary>
        void SimulateCombat()
        {
            combatManager.StartCombat(playerTeam, enemyTeam);

            // Simulate a few turns
            int maxTurns = 20; // Prevent infinite loop
            int turnCount = 0;

            while (combatManager.IsCombatActive() && turnCount < maxTurns)
            {
                var currentCombatant = combatManager.GetCurrentCombatant();
                if (currentCombatant == null)
                    break;

                // Simple AI: Attack a random enemy
                var enemies = GetEnemies(currentCombatant.teamId);
                if (enemies.Count > 0)
                {
                    var target = enemies[Random.Range(0, enemies.Count)];
                    combatManager.PerformBasicAttack(currentCombatant.character, target);
                }

                combatManager.EndCurrentTurn();
                turnCount++;
            }

            Debug.Log("\n=== Combat Ended ===");
        }

        /// <summary>
        /// Get all alive enemies for a given team
        /// </summary>
        List<CharacterData> GetEnemies(int teamId)
        {
            List<CharacterData> enemies = new List<CharacterData>();

            if (teamId == 0)
            {
                foreach (var character in enemyTeam)
                {
                    if (character.IsAlive())
                        enemies.Add(character);
                }
            }
            else
            {
                foreach (var character in playerTeam)
                {
                    if (character.IsAlive())
                        enemies.Add(character);
                }
            }

            return enemies;
        }

        /// <summary>
        /// Example: Display character sheet
        /// </summary>
        [ContextMenu("Display All Character Stats")]
        void DisplayAllCharacterStats()
        {
            Debug.Log("=== PLAYER TEAM ===");
            foreach (var character in playerTeam)
            {
                Debug.Log(character.GetDetailedStats());
                Debug.Log("\n");
            }

            Debug.Log("=== ENEMY TEAM ===");
            foreach (var character in enemyTeam)
            {
                Debug.Log(character.GetDetailedStats());
                Debug.Log("\n");
            }
        }

        /// <summary>
        /// Example: Level up a character
        /// </summary>
        [ContextMenu("Level Up All Characters")]
        void LevelUpAllCharacters()
        {
            foreach (var character in playerTeam)
            {
                character.GainExperience(1000);
            }

            foreach (var character in enemyTeam)
            {
                character.GainExperience(1000);
            }

            Debug.Log("All characters leveled up!");
        }
    }
}
