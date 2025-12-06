using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Character;
using System.Collections.Generic;

namespace MythRealFFSV2.UI
{
    /// <summary>
    /// Displays team rosters and character details
    /// </summary>
    public class TeamRosterUI : MonoBehaviour
    {
        [Header("Team Selection")]
        public TMP_Dropdown teamDropdown;
        public TextMeshProUGUI teamInfoText;

        [Header("Roster Display")]
        public Transform rosterContainer;
        public GameObject characterRowPrefab;

        [Header("Character Detail Panel")]
        public GameObject detailPanel;
        public TextMeshProUGUI detailNameText;
        public TextMeshProUGUI detailStatsText;
        public TextMeshProUGUI detailAbilitiesText;
        public Button closeDetailButton;

        [Header("Navigation")]
        public Button backButton;

        private TeamManager teamManager;
        private TeamData currentTeam;
        private CharacterData selectedCharacter;

        void Start()
        {
            // Get managers
            teamManager = FindObjectOfType<TeamManager>();

            // Setup buttons
            if (backButton != null)
                backButton.onClick.AddListener(OnBack);

            if (closeDetailButton != null)
                closeDetailButton.onClick.AddListener(OnCloseDetail);

            // Setup team dropdown
            if (teamDropdown != null)
                teamDropdown.onValueChanged.AddListener(OnTeamSelected);

            // Hide detail panel initially
            if (detailPanel != null)
                detailPanel.SetActive(false);
        }

        void OnEnable()
        {
            RefreshDisplay();
        }

        /// <summary>
        /// Refresh the entire UI
        /// </summary>
        public void RefreshDisplay()
        {
            if (teamManager == null)
                return;

            PopulateTeamDropdown();

            // Select first team by default if none selected
            if (currentTeam == null && teamManager.teams.Count > 0)
            {
                currentTeam = teamManager.teams[0];
            }

            UpdateTeamInfo();
            UpdateRoster();
        }

        /// <summary>
        /// Populate the team selection dropdown
        /// </summary>
        void PopulateTeamDropdown()
        {
            if (teamDropdown == null)
                return;

            teamDropdown.ClearOptions();

            List<string> teamNames = new List<string>();
            foreach (var team in teamManager.teams)
            {
                teamNames.Add(team.teamName);
            }

            teamDropdown.AddOptions(teamNames);
        }

        /// <summary>
        /// Handle team selection from dropdown
        /// </summary>
        void OnTeamSelected(int index)
        {
            if (index >= 0 && index < teamManager.teams.Count)
            {
                currentTeam = teamManager.teams[index];
                UpdateTeamInfo();
                UpdateRoster();
            }
        }

        /// <summary>
        /// Update team information display
        /// </summary>
        void UpdateTeamInfo()
        {
            if (teamInfoText == null || currentTeam == null)
                return;

            string info = $"{currentTeam.teamName}\n";
            info += $"Manager: {currentTeam.managerName}\n";
            info += $"Record: {currentTeam.wins}-{currentTeam.losses}-{currentTeam.draws} ";
            info += $"({currentTeam.WinPercentage:P1})\n";
            info += $"Roster Size: {currentTeam.roster.Count} characters\n";
            info += $"Championships: {currentTeam.championships}";

            teamInfoText.text = info;
        }

        /// <summary>
        /// Update the roster display
        /// </summary>
        void UpdateRoster()
        {
            if (rosterContainer == null || currentTeam == null)
                return;

            // Clear existing rows
            foreach (Transform child in rosterContainer)
            {
                Destroy(child.gameObject);
            }

            // Create row for each character
            foreach (var character in currentTeam.roster)
            {
                CreateCharacterRow(character);
            }
        }

        /// <summary>
        /// Create a character row
        /// </summary>
        void CreateCharacterRow(CharacterData character)
        {
            if (characterRowPrefab == null)
            {
                Debug.LogWarning("Character row prefab not assigned!");
                return;
            }

            GameObject row = Instantiate(characterRowPrefab, rosterContainer);

            // Find text elements (adjust based on your prefab structure)
            TextMeshProUGUI nameText = row.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI levelText = row.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ancestryText = row.transform.Find("AncestryText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI backgroundText = row.transform.Find("BackgroundText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI hpText = row.transform.Find("HPText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI statusText = row.transform.Find("StatusText")?.GetComponent<TextMeshProUGUI>();

            // Set text values
            if (nameText != null)
                nameText.text = character.characterName;

            if (levelText != null)
                levelText.text = $"Lv {character.level}";

            if (ancestryText != null)
                ancestryText.text = character.ancestry?.ancestryName ?? "Unknown";

            if (backgroundText != null)
                backgroundText.text = character.background?.backgroundName ?? "Unknown";

            if (hpText != null)
                hpText.text = $"HP: {character.currentHP}/{character.maxHP}";

            if (statusText != null)
            {
                if (character.currentHP <= 0)
                    statusText.text = "KO'd";
                else if (character.currentHP < character.maxHP / 2)
                    statusText.text = "Injured";
                else
                    statusText.text = "Ready";
            }

            // Add click handler to show details
            Button rowButton = row.GetComponent<Button>();
            if (rowButton != null)
            {
                rowButton.onClick.AddListener(() => OnShowCharacterDetail(character));
            }
        }

        /// <summary>
        /// Show detailed character information
        /// </summary>
        void OnShowCharacterDetail(CharacterData character)
        {
            selectedCharacter = character;

            if (detailPanel == null)
                return;

            detailPanel.SetActive(true);

            // Set character name
            if (detailNameText != null)
            {
                detailNameText.text = $"{character.characterName} - Level {character.level}";
            }

            // Set character stats
            if (detailStatsText != null)
            {
                string stats = "Stats:\n\n";
                stats += $"Ancestry: {character.ancestry?.ancestryName ?? "Unknown"}\n";
                stats += $"Background: {character.background?.backgroundName ?? "Unknown"}\n\n";

                stats += "Attributes:\n";
                if (character.attributes != null)
                {
                    stats += $"  COR: {character.attributes.COR.score} ({character.attributes.COR.GetModifierString()})\n";
                    stats += $"  FAI: {character.attributes.FAI.score} ({character.attributes.FAI.GetModifierString()})\n";
                    stats += $"  FRT: {character.attributes.FRT.score} ({character.attributes.FRT.GetModifierString()})\n";
                    stats += $"  INT: {character.attributes.INT.score} ({character.attributes.INT.GetModifierString()})\n";
                    stats += $"  SOC: {character.attributes.SOC.score} ({character.attributes.SOC.GetModifierString()})\n";
                    stats += $"  STR: {character.attributes.STR.score} ({character.attributes.STR.GetModifierString()})\n";
                    stats += $"  WIT: {character.attributes.WIT.score} ({character.attributes.WIT.GetModifierString()})\n";
                }

                stats += $"\nHP: {character.currentHP}/{character.maxHP}\n";
                stats += $"Defense: {character.defense}\n";
                stats += $"XP: {character.experiencePoints}";

                detailStatsText.text = stats;
            }

            // Set abilities/skills
            if (detailAbilitiesText != null)
            {
                string abilities = "Abilities:\n\n";

                if (character.learnedAbilities.Count > 0)
                {
                    foreach (var ability in character.learnedAbilities)
                    {
                        abilities += $"• {ability.abilityName}\n";
                        abilities += $"  {ability.description}\n";
                        abilities += $"  Cost: {ability.apCost} AP\n\n";
                    }
                }
                else
                {
                    abilities += "No abilities learned yet.";
                }

                detailAbilitiesText.text = abilities;
            }
        }

        /// <summary>
        /// Close the detail panel
        /// </summary>
        void OnCloseDetail()
        {
            if (detailPanel != null)
                detailPanel.SetActive(false);
        }

        /// <summary>
        /// Go back to league dashboard
        /// </summary>
        void OnBack()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.GoBack();
            }
        }
    }
}
