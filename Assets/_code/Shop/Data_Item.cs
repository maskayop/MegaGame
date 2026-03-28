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

        public bool IsPremium()
        {
            if (openRealPrice != 0 || !string.IsNullOrWhiteSpace(rustoreId))
                return true;
            else
                return false;
        }
    }
}
