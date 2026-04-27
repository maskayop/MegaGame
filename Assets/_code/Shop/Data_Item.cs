using UnityEngine;
using UnityEngine.Localization;

namespace MegaGame
{
    [CreateAssetMenu(fileName = "Data Item", menuName = "Mega Game/Data Item")]
    public class Data_Item : ScriptableObject
    {
        public LocalizedString itemName;
        public LocalizedString itemDescription;

        [Header("Prices")]
        public int priceId;
        public int openGamePrice;

        [Header("Real Prices")]
        public int openRealPrice;
        public string rustoreId;

        [Space(20)]
        public GameObject prefab;

        [Header("Characteristics")]
        public bool showDamage = false;
        public bool showAttackSpeed = false;
        public bool showMovementSpeed = false;
        public bool showHealth = false;
        public bool showRegeneration = false;
        public bool showMaintenance = false;
        public bool showRevenue = false;
        public bool showBuildingPrice = false;

        public enum SettlementConstructionType { none, bigPortFort, smallPortFort, trader }
        public SettlementConstructionType type = SettlementConstructionType.none;

        public bool IsPremium()
        {
            if (openRealPrice != 0 || !string.IsNullOrWhiteSpace(rustoreId))
                return true;
            else
                return false;
        }
    }
}
