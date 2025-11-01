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

            // ��� Windows standalone
            if (Application.platform == RuntimePlatform.WindowsPlayer)
                logFilePath = Path.Combine(assemblyPath, "../game_log.txt");
            else // ��� Editor
                logFilePath = Path.Combine(Application.dataPath, "game_log.txt");

            // ������������� �� ������� �����������
            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            string logEntry = $"[{System.DateTime.Today:d}-{System.DateTime.Now:HH:mm:ss}][{type}] {logString}\n";

            try
            {
                File.AppendAllText(logFilePath, logEntry);

                // ��� ������ ��������� stack trace
                if (type == LogType.Error || type == LogType.Exception)
                    File.AppendAllText(logFilePath, $"Stack Trace: {stackTrace}\n");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"�� ������� �������� ���: {e.Message}");
            }
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}
