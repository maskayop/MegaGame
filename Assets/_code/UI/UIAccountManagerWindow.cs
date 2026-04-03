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

        [Header("Delete Saves")]
        [SerializeField] GameObject deleteSavesButton;
        [SerializeField] GameObject deleteSavesQuestionWindow;
        [SerializeField] GameObject deleteSavesSecurityQuestionWindow;

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

            if (accountsNames.Count == loadAccountButtons.Count)
                deleteSavesButton.SetActive(true);
            else
                deleteSavesButton.SetActive(false);
        }

        public void Close()
        {
            isOpen = false;
            window.SetActive(false);

            CloseLoadAccountQuestionWindow();
            CloseCreateAccountQuestionWindow();
            CloseDeleteSavesQuestionWindow();
            CloseDeleteSavesSecurityQuestionWindow();
        }

        public void CreateAccount()
        {
            gameDataSaver.CreateAccount(createAccountNameInputField.text);
            mainMenu.Init();
        }

        public void DeleteAccount()
        {
            gameDataSaver.LoadLastAccount();
            mainMenu.Init();
        }

        void InitAccountsButtons()
        {
            gameDataSaver.LoadGameData();
            accountsNames = gameDataSaver.GetAccountsNames();

            for (int i = 0; i < accountsNames.Count; i++)
            {
                if (gameDataSaver.GetCurrentAccountName() == accountsNames[i])
                    loadAccountButtons[i].Init(accountsNames[i], true);
                else
                    loadAccountButtons[i].Init(accountsNames[i], false);

                loadAccountButtons[i].SetData(gameDataSaver.GetAccountData(accountsNames[i]));
            }

            for (int i = 0; i < loadAccountButtons.Count; i++)
                loadAccountButtons[i].Init();
        }

        public void DeletePlayerPrefs()
        {
            DataSaveLoad.Instance.DeletePlayerPrefs();
            ScenesManager.Instance.LoadScene(ScenesManager.Instance.GetCurrentOpenScene().name);
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
            CheckForNameAvailability();
        }

        public void CloseCreateAccountQuestionWindow()
        {
            createAccountQuestionWindow.SetActive(false);
        }

        public void OpenDeleteSavesQuestionWindow()
        {
            deleteSavesQuestionWindow.SetActive(true);
        }

        public void CloseDeleteSavesQuestionWindow()
        {
            deleteSavesQuestionWindow.SetActive(false);
        }

        public void OpenDeleteSavesSecurityQuestionWindow()
        {
            deleteSavesSecurityQuestionWindow.SetActive(true);
            CloseDeleteSavesQuestionWindow();
        }

        public void CloseDeleteSavesSecurityQuestionWindow()
        {
            deleteSavesSecurityQuestionWindow.SetActive(false);
            CloseDeleteSavesQuestionWindow();
        }

        void SetCreateAccountButtonInteractable(bool state)
        {
            createAccountButton.interactable = state;
        }

        public void CheckForNameAvailability()
        {
            if (string.IsNullOrWhiteSpace(createAccountNameInputField.text))
            {
                SetCreateAccountButtonInteractable(false);
                return;
            }

            for (int i = 0; i < accountsNames.Count; i++)
            {
                if (accountsNames[i] == createAccountNameInputField.text)
                {
                    SetCreateAccountButtonInteractable(false);
                    return;
                }
            }

            SetCreateAccountButtonInteractable(true);
        }
    }
}
