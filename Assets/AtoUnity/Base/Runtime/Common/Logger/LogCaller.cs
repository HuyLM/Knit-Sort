using System.Runtime.CompilerServices;

namespace AtoGame.Base {
    public static class LogExtension {
        public static void LogCaller(string message = "",
              [CallerLineNumber] int line = 0
            , [CallerMemberName] string memberName = ""
            , [CallerFilePath] string filePath = ""
        )
        {
            if(string.IsNullOrEmpty(message))
            {
                UnityEngine.Debug.Log($"{line} :: {memberName} :: {filePath}");
                return;
            }
            UnityEngine.Debug.Log($"{message}\n{line} :: {memberName} :: {filePath}");
        }
    }
}