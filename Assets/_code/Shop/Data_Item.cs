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
        public int openRealPrice;

        [Space(20)]
        public GameObject prefab;
    }
}
