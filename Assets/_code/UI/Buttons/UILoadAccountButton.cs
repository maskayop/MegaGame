using System.Collections.Generic;
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

        string accountName;
        public string AccountName { get { return accountName; } }

        void Start()
        {
            accountManagerWindow = UIAccountManagerWindow.Instance;
        }

        public void Init(string INaccountName, bool isCurrentAccount)
        {
            accountName = INaccountName;
            nameText.text = accountName;

            accountInfoPanel.SetActive(true);
            createAccountButton.SetActive(false);

            if (isCurrentAccount)
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

        public void SetData(List<int> dataList)
        {
            playerMoneyText.text = dataList[0].ToString();
            playerPortsText.text = dataList[1].ToString();
            enemyPortsText.text = dataList[2].ToString();
            playerVillagesText.text = dataList[3].ToString();
            enemyVillagesText.text = dataList[4].ToString();

            gameEndPanel.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);

            if (dataList[5] == -1)
                return;

            if (dataList[5] > 0)
            {
                gameEndPanel.SetActive(true);
                moneyPanel.SetActive(false);
                settlementsPanel.SetActive(false);
            }

            if (dataList[5] == 1)
                victoryPanel.SetActive(true);
            else if (dataList[5] == 2)
                defeatPanel.SetActive(true);
        }

        public void TryLoadAccount()
        {
            accountManagerWindow.SetCurrentSelectedAccount(accountName);
            accountManagerWindow.OpenLoadAccountQuestionWindow();
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
