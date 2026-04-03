using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UILoadAccountButton : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] Button mainButton;
        [SerializeField] GameObject accountInfoPanel;
        [SerializeField] GameObject currentAccountPanel;
        [SerializeField] GameObject createAccountButton;

        [Header("Info Panels")]
        [SerializeField] GameObject moneyPanel;
        [SerializeField] GameObject settlementsPanel;
        [SerializeField] GameObject gameEndPanel;
        [SerializeField] GameObject victoryPanel;
        [SerializeField] GameObject defeatPanel;

        [Header("Info Texts")]
        [SerializeField] TextMeshProUGUI nameText;

        [SerializeField] TextMeshProUGUI playerMoneyText;
        [SerializeField] TextMeshProUGUI playerPortsText;
        [SerializeField] TextMeshProUGUI playerVillagesText;

        [SerializeField] TextMeshProUGUI enemyPortsText;
        [SerializeField] TextMeshProUGUI enemyVillagesText;

        UIAccountManagerWindow accountManagerWindow;
        GameDataSaver gameDataSaver;

        string accountName;
        public string AccountName { get { return accountName; } }

        void Start()
        {
            accountManagerWindow = UIAccountManagerWindow.Instance;
            gameDataSaver = GameDataSaver.Instance;

            accountName = "";
        }

        public void Init(string INaccountName)
        {
            accountName = INaccountName;
            nameText.text = accountName;

            accountInfoPanel.SetActive(true);
            createAccountButton.SetActive(false);

            if (gameDataSaver.GetCurrentAccountName() == accountName)
            {
                mainButton.interactable = false;
                currentAccountPanel.SetActive(true);
            }
            else
            {
                mainButton.interactable = true;
                currentAccountPanel.SetActive(false);
            }
        }

        public void Init()
        {
            if (!string.IsNullOrWhiteSpace(accountName))
                return;

            mainButton.interactable = false;
            accountInfoPanel.SetActive(false);
            currentAccountPanel.SetActive(false);
            createAccountButton.SetActive(true);
        }

        public void TryLoadAccount()
        {
            accountManagerWindow.SetCurrentSelectedAccount(accountName);
            accountManagerWindow.OpenLoadAccountQuestionWindow();
        }

        public void TryRenameAccount()
        {
            accountManagerWindow.RenameAccount();
        }

        public void TryCreateAccount()
        {
            accountManagerWindow.OpenCreateAccountQuestionWindow();
        }

        public void TryDeleteAccount()
        {
            accountManagerWindow.DeleteAccount();
        }
    }
}
