using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace MegaGame
{
    [CreateAssetMenu(fileName = "Data Island", menuName = "Mega Game/Data Island")]
    public class Data_Island : ScriptableObject
    {
        public int id = -1;
        public LocalizedString islandName;

        public void SetId(int value)
        {
            id = value;
            EditorUtility.SetDirty(this);
        }
    }
}
