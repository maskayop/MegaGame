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

        [Header("Items")]
        [SerializeField] List<Data_Item> items = new List<Data_Item>();

        DataSaveLoad dataSaveLoad;
        GameController gameController;
        ResourcesController resourcesController;
        GameShop gameShop;

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
        string shopDataStateFormat = " SDS";
        string tutorialStateFormat = " TS";

        const int FORT_BIT = 0;
        const int TRADE_BIT = 1;

        const int MediumShip_BIT = 0;
        const int BigShip_BIT = 1;
        const int MegaShip_BIT = 2;
        const int DefenderShip_BIT = 3;
        const int MonstersArtifact_BIT = 4;
        const int Spies_BIT = 5;
        const int DoubleDistances_BIT = 6;

        int buildingStateForSave = 0;
        int shopDataForSave = 0;

        int currentTutorialProgress = -1;

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
            gameShop = GameShop.Instance;
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

            SaveShopData();
            SavePremiumShopData();
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

            LoadShopData();
            LoadPremiumShopData();
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

        public void LoadPlayerMoneyData()
        {
            resourcesController.PlayerMoney = dataSaveLoad.GetSavedString(currentAccountNameKey + playerMoneyFormat);
        }

        public void SaveShopData()
        {
            //Debug.Log("--- Сохраняем обычные покупки ---");

            SetShopDataState(MediumShip_BIT, gameShop.CheckForPurchasing(items[0]), items[0].itemName.GetLocalizedString());
            SetShopDataState(BigShip_BIT, gameShop.CheckForPurchasing(items[1]), items[1].itemName.GetLocalizedString());
            SetShopDataState(DefenderShip_BIT, gameShop.CheckForPurchasing(items[3]), items[3].itemName.GetLocalizedString());

            dataSaveLoad.Save(currentAccountNameKey + shopDataStateFormat, shopDataForSave);

            //Debug.Log("--- Закончили с сохранением обычных покупок ---");
        }

        public void SavePremiumShopData()
        {
            //Debug.Log("--- Сохраняем премиальные покупки ---");

            if (gameShop.CheckForPurchasing(items[2]) || gameShop.CheckForPurchasing(items[7]))
            {
                SetShopDataState(MegaShip_BIT, true, items[2].itemName.GetLocalizedString());
                gameShop.SetPurchasedState(items[2], true);
                gameShop.SetPurchasedState(items[7], true);
            }
            else
            {
                SetShopDataState(MegaShip_BIT, false, items[2].itemName.GetLocalizedString());
                gameShop.SetPurchasedState(items[2], false);
                gameShop.SetPurchasedState(items[7], false);
            }

            SetShopDataState(MonstersArtifact_BIT, gameShop.CheckForPurchasing(items[4]), items[4].itemName.GetLocalizedString());
            SetShopDataState(Spies_BIT, gameShop.CheckForPurchasing(items[5]), items[5].itemName.GetLocalizedString());
            SetShopDataState(DoubleDistances_BIT, gameShop.CheckForPurchasing(items[6]), items[6].itemName.GetLocalizedString());

            dataSaveLoad.Save("P" + shopDataStateFormat, shopDataForSave);

            //Debug.Log("--- Закончили с сохранением премиальных покупок ---");
        }

        public void LoadShopData()
        {
            //Debug.Log("--- Загружаем обычные покупки ---");

            int purchasedState = dataSaveLoad.GetSavedInt(currentAccountNameKey + shopDataStateFormat);

            if (purchasedState == -1)
            {
                gameShop.SetPurchasedState(items[0], false);
                gameShop.SetPurchasedState(items[1], false);
                gameShop.SetPurchasedState(items[3], false);
            }
            else
            {
                gameShop.SetPurchasedState(items[0], GetShopDataState(MediumShip_BIT, purchasedState, items[0].itemName.GetLocalizedString()));
                gameShop.SetPurchasedState(items[1], GetShopDataState(BigShip_BIT, purchasedState, items[1].itemName.GetLocalizedString()));
                gameShop.SetPurchasedState(items[3], GetShopDataState(DefenderShip_BIT, purchasedState, items[3].itemName.GetLocalizedString()));
            }

            //Debug.Log("--- Закончили с загрузкой обычных покупок ---");
        }

        public void LoadPremiumShopData()
        {
            //Debug.Log("--- Загружаем премиальные покупки ---");

            int purchasedState = dataSaveLoad.GetSavedInt("P" + shopDataStateFormat);

            if (purchasedState == -1)
            {
                gameShop.SetPurchasedState(items[2], false);
                gameShop.SetPurchasedState(items[4], false);
                gameShop.SetPurchasedState(items[5], false);
                gameShop.SetPurchasedState(items[6], false);
                gameShop.SetPurchasedState(items[7], false);
            }
            else
            {
                if (GetShopDataState(MegaShip_BIT, purchasedState, items[2].itemName.GetLocalizedString()))
                {
                    gameShop.SetPurchasedState(items[2], true);
                    gameShop.SetPurchasedState(items[7], true);
                }
                else
                {
                    gameShop.SetPurchasedState(items[2], false);
                    gameShop.SetPurchasedState(items[7], false);
                }

                gameShop.SetPurchasedState(items[4], GetShopDataState(MonstersArtifact_BIT, purchasedState, items[4].itemName.GetLocalizedString()));
                gameShop.SetPurchasedState(items[5], GetShopDataState(Spies_BIT, purchasedState, items[5].itemName.GetLocalizedString()));
                gameShop.SetPurchasedState(items[6], GetShopDataState(DoubleDistances_BIT, purchasedState, items[6].itemName.GetLocalizedString()));
            }

            //Debug.Log("--- Закончили с загрузкой премиальных покупок ---");
        }

        void SetShopDataState(int bitIndex, bool isPurchased, string itemName)
        {
            //Debug.Log("- Сохраняю " + itemName);

            if (isPurchased)
                shopDataForSave |= (1 << bitIndex);
            else
                shopDataForSave &= ~(1 << bitIndex);
        }

        bool GetShopDataState(int bitIndex, int purchasedState, string itemName)
        {
            //Debug.Log("- Загружаю " + itemName);

            return ((purchasedState >> bitIndex) & 1) == 1;
        }

        public void SaveTutorialData(int progress)
        {
            dataSaveLoad.Save(tutorialStateFormat, progress);
        }

        public void LoadTutorialData()
        {
            int tutorialState = dataSaveLoad.GetSavedInt(tutorialStateFormat);

            if (tutorialState == -1)
                currentTutorialProgress = 0;
            else
                currentTutorialProgress = tutorialState;
        }

        public int GetCurrentTutorialState()
        {
            return currentTutorialProgress;
        }
    }
}
