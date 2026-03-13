using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class GameDataSaver : MonoBehaviour
    {
        public static GameDataSaver Instance { get; private set; }

        [Header("Accounts")]
        public short totalAccountsAmount = -1;
        public short currentAccountId = -1;
        public string currentAccountName;

        DataSaveLoad dataSaveLoad;
        GameController gameController;
        ResourcesController resourcesController;

        string islandOwnerFormat = " IO";
        string startPlayerIslandFormat = " SPI";
        string startEnemyIslandFormat = " SEI";

        string islandHealthFormat = " ISH";

        string playerMoneyFormat = "PP";
        string enemyMoneyFormat = "EP";

        string currentDayFormat = "CD";
        string campaignIsEndedFormat = "CIE";

        string accountNameFormat = "ACC";
        string lastAccountIdFormat = "LACC";
        string currentAccountNameKey = "";
        string totalAccountsAmountFormat = "TAC";

        string settlementConstructionsStateFormat = " SCS";

        const int FORT_BIT = 0;
        const int TRADE_BIT = 1;

        int buildingStateForSave = 0;

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
            resourcesController = ResourcesController.Instance;
        }

        public void SaveGameData()
        {
            dataSaveLoad.Save(currentAccountNameKey + startPlayerIslandFormat, gameController.PlayerStartIslandId);
            dataSaveLoad.Save(currentAccountNameKey + startEnemyIslandFormat, gameController.EnemyStartIslandId);

            for (int i = 0; i < gameController.allIslands.Count; i++)
            {
                if (gameController.allIslands[i].owner == BaseCharacter.Owner.player)
                    dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat, (short)0);
                else if (gameController.allIslands[i].owner == BaseCharacter.Owner.enemy)
                    dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat, (short)1);
                else if (gameController.allIslands[i].owner == BaseCharacter.Owner.neutral)
                    dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat, (short)2);

                if (gameController.allIslands[i].settlement)
                {
                    int currentHealth = Mathf.CeilToInt(gameController.allIslands[i].settlement.currentHealth);
                    dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandHealthFormat, currentHealth);

                    if (gameController.allIslands[i].settlement.GetSettlementConstructions())
                    {
                        SetBuildingState(FORT_BIT, gameController.allIslands[i].settlement.GetSettlementConstructions().fortIsBuilt);
                        SetBuildingState(TRADE_BIT, gameController.allIslands[i].settlement.GetSettlementConstructions().traderIsBuilt);

                        dataSaveLoad.Save(currentAccountNameKey + gameController.allIslands[i].islandData.id + settlementConstructionsStateFormat, buildingStateForSave);
                    }
                }
            }

            dataSaveLoad.Save(currentAccountNameKey + playerMoneyFormat, resourcesController.PlayerMoney);
            dataSaveLoad.Save(currentAccountNameKey + enemyMoneyFormat, resourcesController.EnemyMoney);

            dataSaveLoad.Save(currentAccountNameKey + "Player Money", Strint.GetInt(resourcesController.PlayerMoney));
            dataSaveLoad.Save(currentAccountNameKey + "Enemy Money", Strint.GetInt(resourcesController.EnemyMoney));

            dataSaveLoad.Save(currentAccountNameKey + currentDayFormat, GlobalTimeController.Instance.currentDay);

            if (gameController.CampaignIsEnded)
            {
                if (gameController.IsVictory)
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
            gameController.EnemyStartIslandId = dataSaveLoad.GetSavedShort(currentAccountNameKey + startEnemyIslandFormat);

            for (int i = 0; i < gameController.allIslands.Count; i++)
            {
                if (dataSaveLoad.GetSavedShort(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat) == 0)
                    gameController.allIslands[i].owner = BaseCharacter.Owner.player;
                else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat) == 1)
                    gameController.allIslands[i].owner = BaseCharacter.Owner.enemy;
                else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandOwnerFormat) == 2)
                    gameController.allIslands[i].owner = BaseCharacter.Owner.neutral;

                if (gameController.allIslands[i].settlement)
                {
                    if (gameController.allIslands[i].settlement.GetSettlementConstructions())
                    {
                        int buildingState = dataSaveLoad.GetSavedInt(currentAccountNameKey + gameController.allIslands[i].islandData.id + settlementConstructionsStateFormat);

                        if (buildingState == -1)
                        {
                            gameController.allIslands[i].settlement.GetSettlementConstructions().fortIsBuilt = false;
                            gameController.allIslands[i].settlement.GetSettlementConstructions().traderIsBuilt = false;
                        }
                        else
                        {
                            gameController.allIslands[i].settlement.GetSettlementConstructions().fortIsBuilt = GetBuildingState(FORT_BIT, buildingState);
                            gameController.allIslands[i].settlement.GetSettlementConstructions().traderIsBuilt = GetBuildingState(TRADE_BIT, buildingState);
                        }
                    }
                }
            }

            LoadAllIslandsCurrentHealth();

            resourcesController.PlayerMoney = dataSaveLoad.GetSavedString(currentAccountNameKey + playerMoneyFormat);
            resourcesController.EnemyMoney = dataSaveLoad.GetSavedString(currentAccountNameKey + enemyMoneyFormat);

            GlobalTimeController.Instance.currentDay = dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat);
            resourcesController.CurrentDay = dataSaveLoad.GetSavedInt(currentAccountNameKey + currentDayFormat);

            if (dataSaveLoad.GetSavedShort(currentAccountNameKey + campaignIsEndedFormat) == 1)
            {
                gameController.IsVictory = true;
                gameController.CampaignIsEnded = true;
            }
            else if (dataSaveLoad.GetSavedShort(currentAccountNameKey + campaignIsEndedFormat) == 2)
            {
                gameController.IsVictory = false;
                gameController.CampaignIsEnded = true;
            }
            else
                gameController.CampaignIsEnded = false;
        }

        public void LoadAllIslandsCurrentHealth()
        {
            for (int i = 0; i < gameController.allIslands.Count; i++)
            {
                if (gameController.allIslands[i].settlement)
                {
                    int currentHealth = dataSaveLoad.GetSavedInt(currentAccountNameKey + gameController.allIslands[i].islandData.id + islandHealthFormat);

                    if (currentHealth == -1)
                        gameController.allIslands[i].settlement.currentHealth = gameController.allIslands[i].settlement.MaxHealth;
                    else
                        gameController.allIslands[i].settlement.currentHealth = currentHealth;
                }
            }
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
            short value = -1;
            List<string> accountsNames = GetAccountsNames();

            for (short i = 0; i < accountsNames.Count; i++)
                if (accountsNames[i] == targetAccountName)
                    value = (short)(i + 1);

            currentAccountId = value;
            dataSaveLoad.Save(lastAccountIdFormat, currentAccountId);

            LoadLastAccount();
        }

        void SetBuildingState(int bitIndex, bool isBuilt)
        {
            if (isBuilt)
                buildingStateForSave |= (1 << bitIndex);
            else
                buildingStateForSave &= ~(1 << bitIndex);
        }

        bool GetBuildingState(int bitIndex, int buildingState)
        {
            return ((buildingState >> bitIndex) & 1) == 1;
        }
    }
}
