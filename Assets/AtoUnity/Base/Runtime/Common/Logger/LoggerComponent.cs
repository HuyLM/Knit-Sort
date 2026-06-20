
using UnityEngine;

namespace AtoGame.Base
{
    public class LoggerComponent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] bool _showLogs = true;
        [SerializeField] string _prefix = "Logger";
        [SerializeField, ColorUsage(true)] Color _prefixColor = Color.white;

        private string _hexColor;

        private void Awake()
        {
            _hexColor = ColorUtility.ToHtmlStringRGBA(_prefixColor);
        }

        public void Log(object message, Object sender)
        {
            if (!_showLogs) return;
            Debug.Log($"<color={_hexColor}>{_prefix}: {message}", sender);
        }
    }
}