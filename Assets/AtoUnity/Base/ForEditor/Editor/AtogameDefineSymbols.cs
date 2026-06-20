using System;
using System.Collections.Generic;
using UnityEditor;

namespace AtoGame.Base
{
    [InitializeOnLoad]
    public class AtogameDefineSymbols
    {
        public static string ATOGAME_SYMBOL = "ATOGAME_ENABLE";
        static AtogameDefineSymbols()
        {
            AddDefine(ATOGAME_SYMBOL);
        }

        private static void AddDefine(string symbol)
        {
            List<string> defineList = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';'));
            if(!defineList.Contains(symbol))
            {
                defineList.Add(symbol);
                string defines = string.Join(";", defineList.ToArray());
                var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            }
        }
    }
}