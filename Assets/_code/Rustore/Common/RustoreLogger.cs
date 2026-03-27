using UnityEngine;

namespace MegaGame
{
    public static class RustoreLogger
    {
        public static void LogWarning(string tag, string message)
        {
            AndroidJavaClass log = new AndroidJavaClass("android.util.Log");
            log.CallStatic<int>("w", tag, message);
        }
    }
}
