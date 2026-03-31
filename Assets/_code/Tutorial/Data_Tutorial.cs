using UnityEngine;
using UnityEngine.Localization;

namespace MegaGame
{
    [CreateAssetMenu(fileName = "Data Tutorial", menuName = "Mega Game/Data Tutorial")]
    public class Data_Tutorial : ScriptableObject
    {
        public LocalizedString title;
        public LocalizedString description;
    }
}
