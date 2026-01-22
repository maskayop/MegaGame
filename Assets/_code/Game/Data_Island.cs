using UnityEngine;
using UnityEngine.Localization;

namespace MegaGame
{
    [CreateAssetMenu(fileName = "Data Island", menuName = "Mega Game/Data Island")]
    public class Data_Island : ScriptableObject
    {
        public LocalizedString islandName;
    }
}
