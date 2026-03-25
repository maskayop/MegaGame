//using RuStore;
using System;
using System.Collections.Generic;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    [Serializable]
    public class GameplayObjectItem
    {
        public string name;
        public Data_Item item;
        public string rustoreId;
        public bool isAvailable;
        public bool isPurchased;
    }

    public class GameShop : MonoBehaviour
    {
        public static GameShop Instance { get; private set; }

        [Header("Items")]
        [SerializeField] List<GameplayObjectItem> items = new List<GameplayObjectItem>();

        ResourcesController resourcesController;
        GameDataSaver gameDataSaver;

        bool rustoreIsAvailable;
        public bool RustoreIsAvailable { get { return rustoreIsAvailable; } }

        public string[] currentRustoreStatus = new string[4];

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create GameShop");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            resourcesController = ResourcesController.Instance;
            gameDataSaver = GameDataSaver.Instance;

            LoadData();
            //RustoreValidation();
        }

        public void LoadData()
        {
            gameDataSaver.LoadLastAccount();
            gameDataSaver.LoadPlayerMoneyData();
            gameDataSaver.LoadShopData();
            gameDataSaver.LoadPremiumShopData();
        }

        public void TryPurchaseItem(Data_Item INitem)
        {
            if (INitem.openGamePrice != 0 && INitem.openRealPrice == 0)
                TryPurchaseGameItem(INitem);
            else if (INitem.openGamePrice == 0 && INitem.openRealPrice != 0)
                TryPurchasePremiumItem(INitem);
        }

        void TryPurchaseGameItem(Data_Item INitem)
        {
            if (Strint.Subtraction(resourcesController.PlayerMoney, Strint.GetString(INitem.openGamePrice)) < 0)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                {
                    items[i].isPurchased = true;
                    resourcesController.RemoveMoneyFromPlayer(INitem.openGamePrice);
                    break;
                }
            }
        }

        void TryPurchasePremiumItem(Data_Item INitem)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                {
                    items[i].isPurchased = true;
                    break;
                }
            }

            gameDataSaver.SavePremiumShopData();
        }

        public void SetPurchasedState(Data_Item INitem, bool state)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                    items[i].isPurchased = state;
            }
        }

        public bool CheckForPurchasing(Data_Item INitem)
        {
            if (INitem.openGamePrice == 0 && INitem.openRealPrice == 0)
                return true;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == INitem)
                {
                    if (items[i].isPurchased)
                        return true;
                }
            }

            return false;
        }

        public bool CheckForAllPremiumItemPurchased()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item.openRealPrice != 0)
                {
                    if (!items[i].isPurchased)
                        return false;
                }
            }

            return true;
        }
        /*
        public void RustoreValidation()
        {
            RustorePayments.OnStoreAvailable += OnRustoreAvailable;
            RustorePayments.OnStoreAvailableError += OnRustoreAvailableError;
            RustorePayments.OnStoreConnectionFailed += OnRustoreConnectionFailed;

            RustorePayments.OnUserAuthorized += OnRustoreUserAuthorized;
            RustorePayments.OnUserUnauthorized += OnRustoreUserUnauthorized;
            RustorePayments.OnUserAuthorizationFailed += OnRustoreUserAuthorizationFailed;

            RustorePayments.OnProductsLoaded += OnRustoreProductsLoaded;
            RustorePayments.OnProductsLoadingError += OnRustoreProductsLoadingError;

            RustorePayments.OnGetUserPurchasesSuccess += OnRustoreGetUserPurchasesSuccess;
            RustorePayments.OnGetUserSubscriptionPurchasesSuccess += OnRustoreGetUserSubscriptionPurchasesSuccess;
            RustorePayments.OnGetUserPurchasesFailed += OnRustoreGetUserPurchasesFailed;
        }

        void OnRustoreAvailable()
        {
            currentRustoreStatus[0] = "<color=green>" + "Rustore is Ready" + "</color>";
        }

        void OnRustoreAvailableError(string reason)
        {
            currentRustoreStatus[0] = "<color=red>" + "Rustore is Not ready" + " - " + "</color>" + reason;
        }

        void OnRustoreConnectionFailed(RuStoreError error)
        {
            currentRustoreStatus[0] = "<color=red>" + "Rustore is Not ready" + " - " + "</color>" + error.name + " - " + error.description;
        }

        void OnRustoreUserAuthorized()
        {
            currentRustoreStatus[1] = "<color=green>" + "User is Authorized" + "</color>";
        }

        void OnRustoreUserUnauthorized()
        {
            currentRustoreStatus[1] = "<color=red>" + "User is Not authorized" + "</color>";
        }

        void OnRustoreUserAuthorizationFailed(RuStoreError error)
        {
            currentRustoreStatus[1] = "<color=red>" + "User is Not authorized" + " - " + "</color>" + error.name + " - " + error.description;
        }

        void OnRustoreProductsLoaded()
        {
            currentRustoreStatus[2] = "<color=green>" + "Products Successfully loaded" + "</color>";
        }

        void OnRustoreProductsLoadingError(RuStoreError error)
        {
            currentRustoreStatus[2] = "<color=red>" + "Products Not loaded" + " - " + "</color>" + error.name + " - " + error.description;
        }

        void OnRustoreGetUserPurchasesSuccess()
        {
            currentRustoreStatus[3] = "<color=green>" + "User Purchases Successfully loaded" + "</color>";
        }

        void OnRustoreGetUserSubscriptionPurchasesSuccess()
        {
            currentRustoreStatus[3] = "<color=green>" + "User Subscription Purchases Successfully loaded" + "</color>";
        }

        void OnRustoreGetUserPurchasesFailed(RuStoreError error)
        {
            currentRustoreStatus[3] = "<color=red>" + "User Purchases Not loaded" + " - " + "</color>" + error.name + " - " + error.description;
        }
        */
    }
}
