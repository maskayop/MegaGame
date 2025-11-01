using System.IO;
using UnityEngine;

namespace Vopere.Common
{
    public class FileLogger : MonoBehaviour
    {
        string logFilePath;

        void Awake()
        {
            string assemblyPath = Application.dataPath;

            // Для Windows standalone
            if (Application.platform == RuntimePlatform.WindowsPlayer)
                logFilePath = Path.Combine(assemblyPath, "../game_log.txt");
            else // Для Editor
                logFilePath = Path.Combine(Application.dataPath, "game_log.txt");

            // Подписываемся на события логирования
            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            string logEntry = $"[{System.DateTime.Today:d}-{System.DateTime.Now:HH:mm:ss}][{type}] {logString}\n";

            try
            {
                File.AppendAllText(logFilePath, logEntry);

                // Для ошибок добавляем stack trace
                if (type == LogType.Error || type == LogType.Exception)
                    File.AppendAllText(logFilePath, $"Stack Trace: {stackTrace}\n");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Не удалось записать лог: {e.Message}");
            }
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}
