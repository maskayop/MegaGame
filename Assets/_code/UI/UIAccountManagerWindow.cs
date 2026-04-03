using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vopere.Common;

namespace MegaGame.UI
{
    public class UIAccountManagerWindow : MonoBehaviour
    {
        public static UIAccountManagerWindow Instance { get; private set; }

        [SerializeField] GameObject window;

        [Header("Load Account")]
        [SerializeField] GameObject loadAccountQuestionWindow;
        [SerializeField] TextMeshProUGUI loadAccountNameText;

        [Header("Create Account")]
        [SerializeField] GameObject createAccountQuestionWindow;
        [SerializeField] TMP_InputField createAccountNameInputField;
        [SerializeField] Button createAccountButton;

        [Header("Buttons")]
        [SerializeField] List<UILoadAccountButton> loadAccountButtons = new List<UILoadAccountButton>();

        GameDataSaver gameDataSaver;
        UIMainMenu mainMenu;

        bool isOpen = false;

        List<string> accountsNames = new List<string>();
        string currentSelectedAccount;

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

            InitAccountsButtons();
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);

            CloseLoadAccountQuestionWindow();
            CloseCreateAccountQuestionWindow();
        }

        public void RenameAccount()
        {
            //gameDataSaver.SetAccountName(accountNameInputField.text);

            mainMenu.Init();
        }

        public void CreateAccount()
        {
            gameDataSaver.CreateAccount(createAccountNameInputField.text);

            mainMenu.Init();
        }

        public void DeleteAccount()
        {
            gameDataSaver.DeleteCurrentAccount();
            gameDataSaver.LoadLastAccount();

            mainMenu.Init();
        }

        void InitAccountsButtons()
        {
            accountsNames = gameDataSaver.GetAccountsNames();

            for (int i = 0; i < accountsNames.Count; i++)
                loadAccountButtons[i].Init(accountsNames[i]);

            for (int i = 0; i < loadAccountButtons.Count; i++)
                loadAccountButtons[i].Init();
        }

        public void DeletePlayerPrefs()
        {
            DataSaveLoad.Instance.DeletePlayerPrefs();
            gameDataSaver.LoadLastAccount();
            mainMenu.Init();
        }

        public void LoadAccount(string targetAccountName)
        {
            gameDataSaver.LoadAccount(targetAccountName);
            gameDataSaver.SetAccountName(targetAccountName);
            mainMenu.Init();
        }

        public void LoadCurrentSelectedAccount()
        {
            if (string.IsNullOrWhiteSpace(currentSelectedAccount))
                return;

            gameDataSaver.LoadAccount(currentSelectedAccount);
            gameDataSaver.SetAccountName(currentSelectedAccount);
            mainMenu.Init();
        }

        public void SetCurrentSelectedAccount(string accountName)
        {
            currentSelectedAccount = accountName;
        }

        public void OpenLoadAccountQuestionWindow()
        {
            loadAccountNameText.text = currentSelectedAccount;
            loadAccountQuestionWindow.SetActive(true);
        }

        public void CloseLoadAccountQuestionWindow()
        {
            loadAccountQuestionWindow.SetActive(false);
        }

        public void OpenCreateAccountQuestionWindow()
        {
            createAccountQuestionWindow.SetActive(true);
            createAccountNameInputField.text = "";
        }

        public void CloseCreateAccountQuestionWindow()
        {
            createAccountQuestionWindow.SetActive(false);
        }
    }
}
