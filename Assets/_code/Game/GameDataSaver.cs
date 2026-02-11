using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameDataSaver : MonoBehaviour
    {
        public static GameDataSaver Instance { get; private set; }

        [Header("Accounts")]
        public short currentAccountId = -1;
        public string currentAccountName;
        public short totalAccountsAmount = -1;

        DataSaveLoad dataSaveLoad;
        GameController gameController;

        string islandOwnerFormat = " IO";
        string startPlayerIslandFormat = " SPI";

        string playerMoneyFormat = "PP";
        string enemyMoneyFormat = "EP";

        string currentDayFormat = "CD";
        string campaignIsEndedFormat = "CIE";

        string accountNameFormat = "ACC";
        string lastAccountIdFormat = "LACC";
        string currentAccountNameKey = "";
        string totalAccountsAmountFormat = "TAC";

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GameDataSaver");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Init()
        {
            dataSaveLoad = DataSaveLoad.Instance;
            gameController = GameController.Instance;
        }

        public void SaveGameData()
        {
            dataSaveLoad.Save(currentAccountNameKey + startPlayerIslandFormat, gameController.PlayerStartIslandId);

            for (int i = 0; i < gameController.allIslands.Count; i++)
            {
                if (gameController.allIslands[i].owner == BaseCharacter.Owner.player)
                    dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat, (short)0);
                else if (gameController.allIslands[i].owner == BaseCharacter.Owner.enemy)
                    dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat, (short)1);
                else if (gameController.allIslands[i].owner == BaseCharacter.Owner.neutral)
                    dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat, (short)2);
            }

            dataSaveLoad.Save(currentAccountNameKey + playerMoneyFormat, gameController.playerMoney);
            dataSaveLoad.Save(currentAccountNameKey + enemyMoneyFormat, gameController.enemyMoney);

            dataSaveLoad.Save(currentAccountNameKey + "Player Money", Strint.GetInt(gameController.playerMoney));
            dataSaveLoad.Save(currentAccountNameKey + "Enemy Money", Strint.GetInt(gameController.enemyMoney));

            dataSaveLoad.Save(currentAccountNameKey + currentDayFormat, GlobalTimeController.Instance.currentDay);

            if (gameController.campaignIsEnded)
            {
                if (gameController.isVictory)
                    dataSaveLoad.Save(currentAccountNameKey + campaignIsEndedFormat, (short)1);
                else
                    dataSaveLoad.Save(currentAccountNameKey + campaignIsEndedFormat, (short)2);
            }
            else
                dataSaveLoad.Save(currentAccountNameKey + campaignIsEndedFormat, (short)0);

            SaveLastAccount();
        }

        public void LoadGameData()
        {
            gameController.PlayerStartIslandId = dataSaveLoad.GetSavedShort(currentAccountNameKey + startPlayerIslandFormat);

            for (int i = 0; i < gameController.allIslands.Count; i++)
            {
                if (dataSaveLoad.GetSavedShort(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat) == 0)
                    gameController.allIslands[i].owner = BaseCharacter.Owner.player;
                else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat) == 1)
                    gameController.allIslands[i].owner = BaseCharacter.Owner.enemy;
                else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat) == 2)
                    gameController.allIslands[i].owner = BaseCharacter.Owner.neutral;
            }

            gameController.playerMoney = dataSaveLoad.GetSavedString(currentAccountNameKey + playerMoneyFormat);
            gameController.enemyMoney = dataSaveLoad.GetSavedString(currentAccountNameKey + enemyMoneyFormat);

            GlobalTimeController.Instance.currentDay = dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat);
            gameController.currentDay = dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat);

            if (dataSaveLoad.GetSavedShort(currentAccountNameKey + campaignIsEndedFormat) == 1)
            {
                gameController.isVictory = true;
                gameController.campaignIsEnded = true;
            }
            else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + campaignIsEndedFormat) == 2)
            {
                gameController.isVictory = false;
                gameController.campaignIsEnded = true;
            }
            else
                gameController.campaignIsEnded = false;
        }

        public void LoadLastAccount()
        {
            short lastAccount = dataSaveLoad.GetSavedShort(lastAccountIdFormat);

            if (dataSaveLoad.GetSavedShort(lastAccountIdFormat) == -1)
                currentAccountId = 1;
            else
                currentAccountId = lastAccount;

            currentAccountNameKey = accountNameFormat + currentAccountId.ToString() + "-";

            if (dataSaveLoad.GetSavedString(currentAccountNameKey) == "")
                currentAccountName = "A" + currentAccountId.ToString();
            else
                currentAccountName = dataSaveLoad.GetSavedString(currentAccountNameKey);

            if (dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat) == -1)
            {
                GlobalTimeController.Instance.currentDay = 0;
                dataSaveLoad.Save(currentAccountNameKey + currentDayFormat, 0);
            }

            if (dataSaveLoad.GetSavedShort(totalAccountsAmountFormat) == -1)
                totalAccountsAmount = 1;
            else
                totalAccountsAmount = dataSaveLoad.GetSavedShort(totalAccountsAmountFormat);

            SaveLastAccount();
        }

        public void SaveLastAccount()
        {
            dataSaveLoad.Save(lastAccountIdFormat, currentAccountId);

            currentAccountNameKey = accountNameFormat + currentAccountId.ToString() + "-";
            dataSaveLoad.Save(currentAccountNameKey, currentAccountName);

            dataSaveLoad.Save(totalAccountsAmountFormat, totalAccountsAmount);
        }

        public string GetCurrentAccountName()
        {
            return currentAccountName;
        }

        public void SetAccountName(string textValue)
        {
            currentAccountName = textValue;
            SaveLastAccount();
        }

        public void CreateAccount(string textValue)
        {
            totalAccountsAmount++;
            currentAccountId = totalAccountsAmount;
            currentAccountName = textValue;
            SaveLastAccount();
        }

        public List<string> GetAccountsNames()
        {
            List<string> accountsNames = new List<string>();

            for (int i = 1; i <= totalAccountsAmount; i++)
                accountsNames.Add(dataSaveLoad.GetSavedString(accountNameFormat + i + "-"));

            return accountsNames;
        }

        public void LoadAccount(string targetAccountName)
        {
            int value = -1;
            List<string> accountsNames = GetAccountsNames();

            for (int i = 0; i < accountsNames.Count; i++)
                if (accountsNames[i] == targetAccountName)
                    value = i + 1;

            currentAccountId = (short)value;
            dataSaveLoad.Save(lastAccountIdFormat, currentAccountId);

            LoadLastAccount();
        }
    }
}
