using UnityEngine;
using UnityEngine.Localization;

namespace MegaGame
{
    [CreateAssetMenu(fileName = "Data Tutorial", menuName = "Mega Game/Data Tutorial")]
    public class Data_Tutorial : ScriptableObject
    {
        public LocalizedString title;
        public LocalizedString description;
        public LocalizedString readyButtonText;

        public bool hideBackgrounds = false;
        public bool showTarget = false;
        public bool waitForUser = false;
        public bool freezeCamera = false;
        public bool showStartBattleMedal = false;
        public bool freezeTime = false;
    }
}
