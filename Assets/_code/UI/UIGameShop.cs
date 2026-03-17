using UnityEngine;

namespace MegaGame.UI
{
    public class UIGameShop : MonoBehaviour
    {
        public static UIGameShop Instance { get; private set; }

        [SerializeField] GameObject window;

        GameController gameController;
        GameDataSaver gameDataSaver;
        CameraController cameraController;

        public bool isOpen = false;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIGameShop");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (!isOpen)
                return;
        }

        public void Init()
        {
            gameController = GameController.Instance;
            gameDataSaver = GameDataSaver.Instance;
            cameraController = CameraController.Instance;

            Close();
        }

        public void Open()
        {
            isOpen = true;
            window.SetActive(true);
            cameraController.Freeze(true);
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);
            cameraController.Freeze(false);
        }
    }
}
