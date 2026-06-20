using AtoGame.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OtherModules.CommandSystem
{

    public class CommandManager : SingletonBindAlive<CommandManager>
    {
        [SerializeField] private char splitKey = ' ';
        [SerializeField] private bool matchCase = false;
        [Header("Configs")]
        [SerializeField] private Color formatColor;
        [SerializeField] private int idFieldPos;
        [SerializeField] private int descFieldPos;
        [SerializeField] private int formatFieldPos;

        [SerializeField] private CommandUI commandUI;

        private bool enableCheatSecret;
        private CommandSystem commandSystem;

        public CommandSystem CommandSystem => commandSystem;

        private string helpResult = string.Empty;

        protected override void OnAwake()
        {
            base.OnAwake();
            HideUI();
            commandUI.gameObject.SetActive(false);
            commandSystem = new CommandSystem();
            commandSystem.matchCase = matchCase;
            commandSystem.splitKey = splitKey;
            commandSystem.AddCommand(new Command("help", "show all command", "help", () =>
            {
                if (string.IsNullOrEmpty(helpResult))
                {
                    helpResult = string.Empty;
                    List<BaseCommand> commands = commandSystem.GetAllCommand();
                    for (int i = 0; i < commands.Count; ++i)
                    {
                        if (i != 0)
                        {
                            helpResult += "\n";
                        }
                        string commandString = commands[i].ToString(formatColor, idFieldPos, descFieldPos, formatFieldPos);
                        helpResult += $"{i + 1} - {commandString}";

                    }
                }
                commandUI.AddLine(helpResult);
            }));

            commandSystem.AddCommand(new Command<string>("help", "show all command with id", "help <id_command>", (str) =>
            {
                string result = string.Empty;
                List<BaseCommand> commands = commandSystem.GetCommands(str);
                for (int i = 0; i < commands.Count; ++i)
                {
                    if (i != 0)
                    {
                        result += "\n";
                    }
                    result += $"{i + 1} - {commands[i].ToString()}";
                }
                commandUI.AddLine(result);
            }));

            commandSystem.AddCommand(new Command("clear", "clear all console", "clear", () =>
            {
                commandUI.ClearAll();
            }));
        }

        public void AddCommand(BaseCommand command)
        {
            commandSystem.AddCommand(command);
        }

        public bool DoCommand(string input)
        {
            return commandSystem.DoCommand(input);
        }

        public void ShowUI()
        {
            commandUI.ShowUI();
        }

        public void HideUI()
        {
            commandUI.HideUI();
        }
    }

}