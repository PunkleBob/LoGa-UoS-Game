using System;
using UnityEngine;

namespace LoGaCulture.LUTE.Logs
{
    public class LogManager : MonoBehaviour
    {
        public static LogManager Instance { get; private set; }

        public string UUID { get; private set; }

        private void Awake()
        {
            UUID = SystemInfo.deviceUniqueIdentifier;

            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Persist across scenes
        }

        public void Log(LogLevel logLevel, string message, string additionalData = "{}")
        {
            if (Instance == null)
            {
                Debug.LogError("LogManager not initialized. Please ensure it exists in the scene.");
                return;
            }

            if (!LogaConstants.UseLogs)
            {
                return;
            }

            Instance.InternalLog(logLevel, message, additionalData);
        }

        private void InternalLog(LogLevel logLevel, string message, string additionalData = "{}")
        {
            var log = new UserLog
            {
                UUID = UUID,
                LogLevel = logLevel.ToString(),
                Message = message,
                Timestamp = DateTime.UtcNow.ToString("o"),
                AdditionalData = additionalData
            };

            ConnectionManager.Instance.EnqueueLog(log);  // Forward to ConnectionManager
        }
    }

    [Serializable]
    public class UserLog
    {
        public string UUID;
        public string LogLevel;
        public string Message;
        public string Timestamp;
        public string AdditionalData;
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }
}
