using AtoGame.Base.Helper;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OtherModules.CommandSystem
{
    public class CommandUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField ipCommand;
        [SerializeField] private Button btnSubmit;
        [SerializeField] private Button btnClose;
        [SerializeField] private TMPro.TextMeshProUGUI txtConsole;
        [SerializeField] private ScrollRect srConsole;

        private const string helpNote = "\"help\" get all command\n\n";
        private StringBuilder strConsole = new StringBuilder(helpNote);

        private void Start()
        {
            btnSubmit.onClick.AddListener(OnSumitButtonClicked);
            btnClose.onClick.AddListener(OnCloseButtonClicked);
#if UNITY_EDITOR
            ipCommand.onSubmit.AddListener(OnInputSubmit);
#endif
        }

        public void ShowUI()
        {
            this.gameObject.SetActive(true);
            UpdateConsole();
        }

        public void HideUI()
        {
            this.gameObject.SetActive(false);
        }

        public void ClearAll()
        {
            strConsole.Clear();
            strConsole = new StringBuilder(helpNote);
            UpdateConsole();
            srConsole.verticalNormalizedPosition = 0;
        }

        public void AddLine(string content)
        {
            strConsole.AppendLine(content);
            UpdateConsole();
            this.DelayFrame(1, () =>
            {
                srConsole.verticalNormalizedPosition = 0;
            });
        }

        private void UpdateConsole()
        {
            txtConsole.text = strConsole.ToString();
        }

        private void OnSumitButtonClicked()
        {
            string input = ipCommand.text;
            ipCommand.text = string.Empty;
            if (!string.IsNullOrEmpty(input))
            {
                AddLine(input);
                bool doCommand = CommandManager.Instance.DoCommand(input);
                if (doCommand)
                {
                    AddLine("Successful!\n");
                }
                else
                {
                    AddLine("Failed!\n");
                }
            }
            else
            {

            }
            ipCommand.ActivateInputField();
        }

        private void OnCloseButtonClicked()
        {
            HideUI();
        }

        private void OnInputSubmit(string text)
        {
            OnSumitButtonClicked();
        }

    }
}