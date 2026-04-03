using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UIAccountManagerWindow : MonoBehaviour
    {
        public static UIAccountManagerWindow Instance { get; private set; }

        [SerializeField] GameObject window;
        [SerializeField] List<UILoadAccountButton> loadAccountButtons = new List<UILoadAccountButton>();
        [SerializeField] TMP_InputField accountNameInputField;

        GameDataSaver gameDataSaver;
        UIMainMenu mainMenu;

        bool isOpen = false;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UIAccountManagerWindow");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
            Close();
        }

        void Update()
        {
            if (!isOpen)
                return;
        }

        public void Init()
        {
            gameDataSaver = GameDataSaver.Instance;
            mainMenu = UIMainMenu.Instance;
        }

        public void Open()
        {
            isOpen = true;
            window.SetActive(true);

            accountNameInputField.text = gameDataSaver.GetCurrentAccountName();
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);
        }

        public void RenameAccount()
        {
            gameDataSaver.SetAccountName(accountNameInputField.text);

            mainMenu.Init();
        }

        public void CreateAccount()
        {
            gameDataSaver.CreateAccount(accountNameInputField.text);

            mainMenu.Init();
        }

        public void DeleteAccount()
        {
            gameDataSaver.DeleteCurrentAccount();
            gameDataSaver.LoadLastAccount();

            mainMenu.Init();
        }
    }
}
