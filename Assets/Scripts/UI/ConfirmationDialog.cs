using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace MythRealFFSV2.UI
{
    /// <summary>
    /// Reusable confirmation dialog for important user actions
    /// </summary>
    public class ConfirmationDialog : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject dialogPanel;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI messageText;
        public Button confirmButton;
        public Button cancelButton;
        public TextMeshProUGUI confirmButtonText;
        public TextMeshProUGUI cancelButtonText;

        private UnityAction onConfirmAction;
        private UnityAction onCancelAction;

        void Awake()
        {
            // Setup button listeners
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirm);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancel);

            // Hide by default
            Hide();
        }

        /// <summary>
        /// Show confirmation dialog with custom message and actions
        /// </summary>
        public void Show(string title, string message, UnityAction onConfirm, UnityAction onCancel = null,
                        string confirmText = "Confirm", string cancelText = "Cancel")
        {
            // Set texts
            if (titleText != null)
                titleText.text = title;

            if (messageText != null)
                messageText.text = message;

            if (confirmButtonText != null)
                confirmButtonText.text = confirmText;

            if (cancelButtonText != null)
                cancelButtonText.text = cancelText;

            // Store actions
            onConfirmAction = onConfirm;
            onCancelAction = onCancel;

            // Show dialog
            if (dialogPanel != null)
                dialogPanel.SetActive(true);
        }

        /// <summary>
        /// Hide the dialog
        /// </summary>
        public void Hide()
        {
            if (dialogPanel != null)
                dialogPanel.SetActive(false);

            // Clear actions
            onConfirmAction = null;
            onCancelAction = null;
        }

        /// <summary>
        /// Handle confirm button click
        /// </summary>
        void OnConfirm()
        {
            onConfirmAction?.Invoke();
            Hide();
        }

        /// <summary>
        /// Handle cancel button click
        /// </summary>
        void OnCancel()
        {
            onCancelAction?.Invoke();
            Hide();
        }

        #region Static Helper Methods

        /// <summary>
        /// Show a simple yes/no confirmation
        /// </summary>
        public static void ShowYesNo(string title, string message, UnityAction onYes, UnityAction onNo = null)
        {
            var dialog = FindObjectOfType<ConfirmationDialog>();
            if (dialog != null)
            {
                dialog.Show(title, message, onYes, onNo, "Yes", "No");
            }
        }

        /// <summary>
        /// Show a simple OK/Cancel confirmation
        /// </summary>
        public static void ShowOkCancel(string title, string message, UnityAction onOk, UnityAction onCancel = null)
        {
            var dialog = FindObjectOfType<ConfirmationDialog>();
            if (dialog != null)
            {
                dialog.Show(title, message, onOk, onCancel, "OK", "Cancel");
            }
        }

        /// <summary>
        /// Show a simple information dialog with just OK button
        /// </summary>
        public static void ShowInfo(string title, string message, UnityAction onOk = null)
        {
            var dialog = FindObjectOfType<ConfirmationDialog>();
            if (dialog != null)
            {
                dialog.Show(title, message, onOk ?? (() => { }), null, "OK", "");

                // Hide cancel button for info dialogs
                if (dialog.cancelButton != null)
                    dialog.cancelButton.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
