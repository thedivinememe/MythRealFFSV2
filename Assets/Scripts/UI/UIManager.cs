using UnityEngine;
using System.Collections.Generic;

namespace MythRealFFSV2.UI
{
    /// <summary>
    /// Manages UI screens and navigation between them
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Screens")]
        public GameObject mainMenuScreen;
        public GameObject leagueDashboardScreen;
        public GameObject matchResultsScreen;
        public GameObject draftBoardScreen;
        public GameObject teamRosterScreen;

        private Stack<GameObject> screenHistory = new Stack<GameObject>();
        private GameObject currentScreen;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            // Show main menu by default
            ShowMainMenu();
        }

        /// <summary>
        /// Show a screen and hide all others
        /// </summary>
        void ShowScreen(GameObject screen, bool addToHistory = true)
        {
            // Hide current screen
            if (currentScreen != null && addToHistory)
            {
                screenHistory.Push(currentScreen);
                currentScreen.SetActive(false);
            }
            else if (currentScreen != null)
            {
                currentScreen.SetActive(false);
            }

            // Show new screen
            if (screen != null)
            {
                screen.SetActive(true);
                currentScreen = screen;
            }
        }

        /// <summary>
        /// Go back to previous screen
        /// </summary>
        public void GoBack()
        {
            if (screenHistory.Count > 0)
            {
                GameObject previousScreen = screenHistory.Pop();
                ShowScreen(previousScreen, false);
            }
        }

        /// <summary>
        /// Clear screen history (when starting fresh)
        /// </summary>
        public void ClearHistory()
        {
            screenHistory.Clear();
        }

        #region Screen Navigation
        public void ShowMainMenu()
        {
            ClearHistory();
            ShowScreen(mainMenuScreen, false);
        }

        public void ShowLeagueDashboard()
        {
            ShowScreen(leagueDashboardScreen);
        }

        public void ShowMatchResults()
        {
            ShowScreen(matchResultsScreen);
        }

        public void ShowDraftBoard()
        {
            ShowScreen(draftBoardScreen);
        }

        public void ShowTeamRoster()
        {
            ShowScreen(teamRosterScreen);
        }
        #endregion

        #region Utility
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion
    }
}
