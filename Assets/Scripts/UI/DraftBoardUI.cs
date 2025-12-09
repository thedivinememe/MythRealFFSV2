using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Character;
using System.Collections.Generic;
using System.Linq;

namespace MythRealFFSV2.UI
{
    /// <summary>
    /// Displays draft board showing all picks and available characters
    /// </summary>
    public class DraftBoardUI : MonoBehaviour
    {
        [Header("Draft Info")]
        public TextMeshProUGUI draftTitleText;
        public TextMeshProUGUI draftStatusText;

        [Header("Draft Picks Display")]
        public Transform picksContainer;
        public GameObject pickRowPrefab;

        [Header("Available Characters")]
        public Transform availableContainer;
        public GameObject characterCardPrefab;

        [Header("Team Filter")]
        public TMP_Dropdown teamFilterDropdown;

        [Header("Navigation")]
        public Button backButton;
        public Button startDraftButton;

        private DraftManager draftManager;
        private TeamManager teamManager;
        private TeamData filterTeam;

        void Start()
        {
            // Get managers
            draftManager = FindFirstObjectByType<DraftManager>();
            teamManager = FindFirstObjectByType<TeamManager>();

            // Setup buttons
            if (backButton != null)
                backButton.onClick.AddListener(OnBack);

            if (startDraftButton != null)
                startDraftButton.onClick.AddListener(OnStartDraft);

            // Setup team filter dropdown
            if (teamFilterDropdown != null)
                teamFilterDropdown.onValueChanged.AddListener(OnTeamFilterChanged);
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
            UpdateDraftInfo();
            UpdateTeamFilter();
            UpdateDraftPicks();
            UpdateAvailableCharacters();
            UpdateButtons();
        }

        /// <summary>
        /// Update draft information display
        /// </summary>
        void UpdateDraftInfo()
        {
            if (draftManager == null)
                return;

            if (draftTitleText != null)
            {
                draftTitleText.text = "Draft Board";
            }

            if (draftStatusText != null)
            {
                if (draftManager.draftInProgress)
                {
                    int currentPick = draftManager.currentDraft.currentPickIndex + 1; // +1 for display (1-indexed)
                    int totalPicks = draftManager.currentDraft.allPicks.Count;
                    var currentPickObj = draftManager.currentDraft.GetCurrentPick();

                    if (currentPickObj != null)
                    {
                        draftStatusText.text = $"Pick {currentPick}/{totalPicks} - {currentPickObj.draftingTeam.teamName} is drafting...";
                    }
                    else
                    {
                        draftStatusText.text = "Draft Complete!";
                    }
                }
                else
                {
                    draftStatusText.text = "No draft in progress";
                }
            }
        }

        /// <summary>
        /// Update team filter dropdown
        /// </summary>
        void UpdateTeamFilter()
        {
            if (teamFilterDropdown == null || teamManager == null)
                return;

            teamFilterDropdown.ClearOptions();

            List<string> options = new List<string> { "All Teams" };
            foreach (var team in teamManager.teams)
            {
                options.Add(team.teamName);
            }

            teamFilterDropdown.AddOptions(options);
        }

        /// <summary>
        /// Handle team filter change
        /// </summary>
        void OnTeamFilterChanged(int index)
        {
            if (index == 0)
            {
                filterTeam = null; // Show all teams
            }
            else if (index > 0 && index <= teamManager.teams.Count)
            {
                filterTeam = teamManager.teams[index - 1];
            }

            UpdateDraftPicks();
        }

        /// <summary>
        /// Update draft picks display
        /// </summary>
        void UpdateDraftPicks()
        {
            if (picksContainer == null)
                return;

            // Clear existing picks
            foreach (Transform child in picksContainer)
            {
                Destroy(child.gameObject);
            }

            if (draftManager == null || draftManager.currentDraft == null)
            {
                CreateNoDraftMessage();
                return;
            }

            // Get all picks (completed ones)
            var allPicks = draftManager.currentDraft.allPicks;

            // Filter by team if selected
            var picksToShow = filterTeam != null
                ? allPicks.Where(p => p.draftingTeam == filterTeam && p.selectedCharacter != null).ToList()
                : allPicks.Where(p => p.selectedCharacter != null).ToList();

            // Create rows for picks
            foreach (var pick in picksToShow)
            {
                CreatePickRow(pick);
            }
        }

        /// <summary>
        /// Create a message when no draft is active
        /// </summary>
        void CreateNoDraftMessage()
        {
            if (pickRowPrefab == null)
                return;

            GameObject messageObj = Instantiate(pickRowPrefab, picksContainer);
            TextMeshProUGUI messageText = messageObj.GetComponentInChildren<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = "No draft has been conducted yet. Start a new game to begin drafting.";
            }
        }

        /// <summary>
        /// Create a draft pick row
        /// </summary>
        void CreatePickRow(DraftPick pick)
        {
            if (pickRowPrefab == null || pick.selectedCharacter == null)
                return;

            GameObject row = Instantiate(pickRowPrefab, picksContainer);

            // Find text elements (adjust based on your prefab structure)
            TextMeshProUGUI pickNumText = row.transform.Find("PickNumText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI roundText = row.transform.Find("RoundText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI teamText = row.transform.Find("TeamText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI characterText = row.transform.Find("CharacterText")?.GetComponent<TextMeshProUGUI>();

            // Set text values
            if (pickNumText != null)
                pickNumText.text = $"#{pick.pickNumber}";

            if (roundText != null)
                roundText.text = $"R{pick.round}";

            if (teamText != null)
                teamText.text = pick.draftingTeam.teamName;

            if (characterText != null)
            {
                var character = pick.selectedCharacter;
                characterText.text = $"{character.characterName} ({character.ancestry?.ancestryName}, {character.background?.backgroundName})";
            }
        }

        /// <summary>
        /// Update available characters display
        /// </summary>
        void UpdateAvailableCharacters()
        {
            if (availableContainer == null)
                return;

            // Clear existing cards
            foreach (Transform child in availableContainer)
            {
                Destroy(child.gameObject);
            }

            if (draftManager == null || draftManager.currentDraft == null)
                return;

            // Get available characters
            var availableCharacters = draftManager.currentDraft.availableCharacters;

            // Show first 20 available characters (to avoid UI overflow)
            int maxToShow = Mathf.Min(20, availableCharacters.Count);

            for (int i = 0; i < maxToShow; i++)
            {
                CreateCharacterCard(availableCharacters[i]);
            }

            // Show count if more available
            if (availableCharacters.Count > maxToShow)
            {
                // Could add a "... and X more" text element
            }
        }

        /// <summary>
        /// Create a character card for available character
        /// </summary>
        void CreateCharacterCard(CharacterData character)
        {
            if (characterCardPrefab == null)
                return;

            GameObject card = Instantiate(characterCardPrefab, availableContainer);

            // Find text elements
            TextMeshProUGUI nameText = card.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI detailsText = card.transform.Find("DetailsText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ratingText = card.transform.Find("RatingText")?.GetComponent<TextMeshProUGUI>();

            // Set text values
            if (nameText != null)
                nameText.text = character.characterName;

            if (detailsText != null)
            {
                string ancestry = character.ancestry?.ancestryName ?? "Unknown";
                string background = character.background?.backgroundName ?? "Unknown";
                detailsText.text = $"{ancestry}\n{background}";
            }

            if (ratingText != null)
            {
                float rating = Draft.CalculateCharacterRating(character);
                ratingText.text = $"Rating: {rating:F0}";
            }

            // Could add click handler for manual drafting in the future
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null && draftManager.draftInProgress)
            {
                cardButton.onClick.AddListener(() => OnSelectCharacter(character));
            }
        }

        /// <summary>
        /// Handle character selection (for manual drafting)
        /// </summary>
        void OnSelectCharacter(CharacterData character)
        {
            // Future: Implement manual draft selection
            Debug.Log($"Selected character: {character.characterName}");
            // For now, this is just a placeholder for future manual draft feature
        }

        /// <summary>
        /// Update button states
        /// </summary>
        void UpdateButtons()
        {
            if (startDraftButton != null)
            {
                // Only enable if no draft in progress and teams exist
                bool canStartDraft = !draftManager.draftInProgress && teamManager.teams.Count >= 2;
                startDraftButton.interactable = canStartDraft;
            }
        }

        /// <summary>
        /// Start a new draft
        /// </summary>
        void OnStartDraft()
        {
            // This would need proper configuration
            // For now, just log a message
            Debug.Log("Start Draft - This would trigger a new draft configuration screen");
            // Future: Show draft configuration dialog
        }

        /// <summary>
        /// Go back to previous screen
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
