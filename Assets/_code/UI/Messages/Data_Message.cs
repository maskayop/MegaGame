using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace MegaGame
{
    [CreateAssetMenu(fileName = "Data Message", menuName = "Mega Game/Data Message")]
    public class Data_Message : ScriptableObject
    {
        public List<LocalizedString> messages = new List<LocalizedString>();

        public string GetMessageText()
        {
            return messages[Random.Range(0, messages.Count)].GetLocalizedString();
        }
    }
}
