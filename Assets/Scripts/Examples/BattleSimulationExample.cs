using UnityEngine;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Combat;
using System.Collections.Generic;

namespace MythRealFFSV2.Examples
{
    /// <summary>
    /// Example demonstrating automated battle simulation with AI
    /// Shows how to create teams and run simulated battles
    /// </summary>
    public class BattleSimulationExample : MonoBehaviour
    {
        [Header("Data Assets")]
        [Tooltip("Assign your created ScriptableObject assets")]
        public List<AncestryData> ancestries;
        public List<BackgroundData> backgrounds;
        public List<AbilityData> abilities;
        public List<TalentData> talents;

        [Header("Battle Settings")]
        public int teamSize = 3;
        public bool instantSimulation = true;
        public AIPersonality team1Personality = AIPersonality.Tactical;
        public AIPersonality team2Personality = AIPersonality.Aggressive;

        [Header("Simulation Results")]
        public BattleResult lastBattleResult;

        private CharacterGenerator generator;
        private BattleSimulator simulator;

        void Start()
        {
            // Get or create components
            generator = GetComponent<CharacterGenerator>();
            if (generator == null)
                generator = gameObject.AddComponent<CharacterGenerator>();

            simulator = GetComponent<BattleSimulator>();
            if (simulator == null)
                simulator = gameObject.AddComponent<BattleSimulator>();

            // Configure simulator
            simulator.instantSimulation = instantSimulation;
            simulator.team1Personality = team1Personality;
            simulator.team2Personality = team2Personality;

            // Subscribe to battle complete event
            simulator.onBattleComplete.AddListener(OnBattleComplete);
        }

        [ContextMenu("Run Example Battle")]
        void RunExampleBattle()
        {
            // Ensure components are initialized (in case Start hasn't run yet)
            if (generator == null)
            {
                generator = GetComponent<CharacterGenerator>();
                if (generator == null)
                    generator = gameObject.AddComponent<CharacterGenerator>();
            }

            if (simulator == null)
            {
                simulator = GetComponent<BattleSimulator>();
                if (simulator == null)
                    simulator = gameObject.AddComponent<BattleSimulator>();

                // Configure simulator
                simulator.instantSimulation = instantSimulation;
                simulator.team1Personality = team1Personality;
                simulator.team2Personality = team2Personality;
                simulator.onBattleComplete.AddListener(OnBattleComplete);
            }

            Debug.Log("=== CREATING TEAMS ===\n");

            // Create Team 1: Player Heroes
            List<CharacterData> team1 = new List<CharacterData>();
            team1.Add(CreateWarriorHero());
            team1.Add(CreateMageHero());
            team1.Add(CreateRogueHero());

            // Create Team 2: Enemy Team
            List<CharacterData> team2 = new List<CharacterData>();

            if (ancestries != null && ancestries.Count > 0 && backgrounds != null && backgrounds.Count > 0)
            {
                for (int i = 0; i < teamSize; i++)
                {
                    team2.Add(generator.CreateRandomCharacter(ancestries, backgrounds));
                }
            }
            else
            {
                // Create placeholder enemies
                team2.Add(CreatePlaceholderCharacter("Enemy 1"));
                team2.Add(CreatePlaceholderCharacter("Enemy 2"));
                team2.Add(CreatePlaceholderCharacter("Enemy 3"));
            }

            // Give characters some abilities
            AssignBasicAbilities(team1);
            AssignBasicAbilities(team2);

            Debug.Log("\n=== TEAM 1 (Heroes) ===");
            foreach (var hero in team1)
            {
                Debug.Log($"{hero.characterName} - {hero.ancestry?.ancestryName ?? "Unknown"} {hero.background?.backgroundName ?? "Unknown"}");
                Debug.Log($"  HP: {hero.currentHP}/{hero.maxHP} | DEF: {hero.defense}");
                Debug.Log($"  Abilities: {hero.knownAbilities.Count}");
            }

            Debug.Log("\n=== TEAM 2 (Enemies) ===");
            foreach (var enemy in team2)
            {
                Debug.Log($"{enemy.characterName} - {enemy.ancestry?.ancestryName ?? "Unknown"} {enemy.background?.backgroundName ?? "Unknown"}");
                Debug.Log($"  HP: {enemy.currentHP}/{enemy.maxHP} | DEF: {enemy.defense}");
                Debug.Log($"  Abilities: {enemy.knownAbilities.Count}");
            }

            // Start battle simulation
            Debug.Log("\n=== STARTING BATTLE ===\n");
            simulator.SimulateBattle(team1, team2);
        }

        [ContextMenu("Run Multiple Simulations")]
        void RunMultipleSimulations()
        {
            int battlesToRun = 10;
            int team1Wins = 0;
            int team2Wins = 0;
            int draws = 0;

            Debug.Log($"=== RUNNING {battlesToRun} SIMULATIONS ===\n");

            for (int i = 0; i < battlesToRun; i++)
            {
                // Create teams
                List<CharacterData> team1 = new List<CharacterData>
                {
                    CreateWarriorHero(),
                    CreateMageHero(),
                    CreateRogueHero()
                };

                List<CharacterData> team2 = new List<CharacterData>
                {
                    CreatePlaceholderCharacter($"Enemy {i}-1"),
                    CreatePlaceholderCharacter($"Enemy {i}-2"),
                    CreatePlaceholderCharacter($"Enemy {i}-3")
                };

                AssignBasicAbilities(team1);
                AssignBasicAbilities(team2);

                // Run instant simulation
                simulator.instantSimulation = true;
                simulator.SimulateBattle(team1, team2);

                // Wait for simulation to complete (this is a coroutine, so in real usage you'd need to wait)
                // For this example, we'll just count the last result

                Debug.Log($"Battle {i + 1} complete");
            }

            Debug.Log($"\n=== SIMULATION RESULTS ===");
            Debug.Log($"Team 1 Wins: {team1Wins}");
            Debug.Log($"Team 2 Wins: {team2Wins}");
            Debug.Log($"Draws: {draws}");
        }

        void OnBattleComplete(BattleResult result)
        {
            lastBattleResult = result;

            Debug.Log("\n=== BATTLE COMPLETE ===");
            Debug.Log($"Winner: {result.outcome}");
            Debug.Log("\nFinal Team States:");

            Debug.Log("\nTEAM 1:");
            foreach (var character in result.team1FinalState)
            {
                Debug.Log($"  {character.characterName}: HP {character.currentHP}/{character.maxHP} - {(character.IsAlive() ? "ALIVE" : "DEFEATED")}");
            }

            Debug.Log("\nTEAM 2:");
            foreach (var character in result.team2FinalState)
            {
                Debug.Log($"  {character.characterName}: HP {character.currentHP}/{character.maxHP} - {(character.IsAlive() ? "ALIVE" : "DEFEATED")}");
            }

            if (result.statistics != null)
            {
                Debug.Log(result.statistics.ToString());
            }
        }

        #region Character Creation Helpers
        CharacterData CreateWarriorHero()
        {
            CharacterData warrior = CreatePlaceholderCharacter("Warrior Hero");
            warrior.attributes.SetBaseValue(AttributeType.Strength, 16);
            warrior.attributes.SetBaseValue(AttributeType.Fortitude, 14);
            warrior.attributes.SetBaseValue(AttributeType.Coordination, 12);
            warrior.CalculateMaxHP();
            warrior.CalculateDefense();
            warrior.currentHP = warrior.maxHP;

            return warrior;
        }

        CharacterData CreateMageHero()
        {
            CharacterData mage = CreatePlaceholderCharacter("Mage Hero");
            mage.attributes.SetBaseValue(AttributeType.Intelligence, 16);
            mage.attributes.SetBaseValue(AttributeType.Wits, 14);
            mage.attributes.SetBaseValue(AttributeType.Coordination, 12);
            mage.attributes.SetBaseValue(AttributeType.Fortitude, 10);
            mage.CalculateMaxHP();
            mage.CalculateDefense();
            mage.currentHP = mage.maxHP;

            return mage;
        }

        CharacterData CreateRogueHero()
        {
            CharacterData rogue = CreatePlaceholderCharacter("Rogue Hero");
            rogue.attributes.SetBaseValue(AttributeType.Coordination, 16);
            rogue.attributes.SetBaseValue(AttributeType.Wits, 14);
            rogue.attributes.SetBaseValue(AttributeType.Strength, 12);
            rogue.CalculateMaxHP();
            rogue.CalculateDefense();
            rogue.currentHP = rogue.maxHP;

            return rogue;
        }

        CharacterData CreatePlaceholderCharacter(string name)
        {
            CharacterData character = new CharacterData
            {
                characterName = name,
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

            character.maxHP = 10 + character.attributes.GetModifier(AttributeType.Fortitude);
            character.currentHP = character.maxHP;
            character.defense = 10 + ((character.attributes.GetModifier(AttributeType.Strength) +
                                      character.attributes.GetModifier(AttributeType.Wits)) / 2);

            character.memory = 4;

            return character;
        }

        void AssignBasicAbilities(List<CharacterData> team)
        {
            if (abilities == null || abilities.Count == 0)
                return;

            foreach (var character in team)
            {
                // Give each character a few random abilities
                int abilitiesToAssign = Mathf.Min(2, abilities.Count);

                for (int i = 0; i < abilitiesToAssign; i++)
                {
                    var ability = abilities[Random.Range(0, abilities.Count)];

                    if (character.CanLearnAbility(ability) && !character.HasAbility(ability))
                    {
                        character.LearnAbility(ability);
                    }
                }
            }
        }
        #endregion

        #region Test Functions
        [ContextMenu("Test AI Decision Making")]
        void TestAIDecisionMaking()
        {
            Debug.Log("=== TESTING AI DECISION MAKING ===\n");

            // Create a test character
            var testCharacter = CreateWarriorHero();
            AssignBasicAbilities(new List<CharacterData> { testCharacter });

            // Create test enemies
            var enemies = new List<CharacterData>
            {
                CreatePlaceholderCharacter("Test Enemy 1"),
                CreatePlaceholderCharacter("Test Enemy 2")
            };

            // Create AI
            BattleAI ai = gameObject.AddComponent<BattleAI>();
            AIPersonalityPresets.ConfigureAI(ai, AIPersonality.Tactical);

            Debug.Log($"Test Character: {testCharacter.characterName}");
            Debug.Log($"  HP: {testCharacter.currentHP}/{testCharacter.maxHP}");
            Debug.Log($"  AP: {testCharacter.currentAP}");
            Debug.Log($"  Known Abilities: {testCharacter.knownAbilities.Count}");

            // The AI would make decisions here in actual combat
            Debug.Log("\nAI is configured and ready to make decisions in combat");
        }
        #endregion
    }
}
