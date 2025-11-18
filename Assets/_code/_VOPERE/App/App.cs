using UnityEngine;

namespace Vopere.Common
{
    public class App : MonoBehaviour
	{
        public static App Instance;

        [SerializeField] int defaultGraphicsLevel = 0;

        [SerializeField] bool useTargetFPS = true;
        [SerializeField] bool initialize = false;

        public bool IsInitialized { get { return initialize; } }

        int graphicsLevel = 0;
        Vector2Int defaultScreenResolution = Vector2Int.zero;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create App");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Init();
        }

        public void Init()
        {
            defaultScreenResolution.x = DataSaveLoad.Instance.GetSavedInt("DefaultScreenResolutionWidth");

            if (defaultScreenResolution.x == -1)
                defaultScreenResolution.x = Screen.width;

            defaultScreenResolution.y = DataSaveLoad.Instance.GetSavedInt("DefaultScreenResolutionHeight");

            if (defaultScreenResolution.y == -1)
                defaultScreenResolution.y = Screen.height;

            SetTargetFPS(useTargetFPS);

            graphicsLevel = DataSaveLoad.Instance.GetSavedInt("GraphicsLevel");

            if (graphicsLevel != -1)
                SetGraphicsLevel(graphicsLevel);
            else
                SetGraphicsLevel(defaultGraphicsLevel);

            int screenResolution = DataSaveLoad.Instance.GetSavedInt("ScreenResolution");
            SetResolution(screenResolution);
        }

        public void ExitGame()
        {
            Debug.Log("Выход из программы" + "\n");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
        }

        public void SetTargetFPS(bool value)
        {
            if (value)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = 30;
        }

        public void SetGraphicsLevel(int level)
        {
            QualitySettings.SetQualityLevel(level, true);
        }

        public void SetResolution(int level)
        {
            if (level == 0)
                Screen.SetResolution(defaultScreenResolution.x * 3 / 8, defaultScreenResolution.y * 3 / 8, FullScreenMode.FullScreenWindow);
            else if (level == 1)
                Screen.SetResolution(defaultScreenResolution.x / 2, defaultScreenResolution.y / 2, FullScreenMode.FullScreenWindow);
            else if (level == 2)
                Screen.SetResolution(defaultScreenResolution.x * 3 / 4, defaultScreenResolution.y * 3 / 4, FullScreenMode.FullScreenWindow);
            else
                Screen.SetResolution(defaultScreenResolution.x, defaultScreenResolution.y, FullScreenMode.FullScreenWindow);
        }

        public Vector2Int GetDefaultScreenResolution()
        {
            return defaultScreenResolution;
        }
    }
}
