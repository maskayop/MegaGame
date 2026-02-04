using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UILoadAccountButton : MonoBehaviour
    {
        public TextMeshProUGUI nameText;

        UIMainMenu mainMenu;

        public void Init(UIMainMenu menu, string accountName)
        {
            mainMenu = menu;
            nameText.text = accountName;
        }

        public void LoadAccount()
        {
            mainMenu.LoadAccount(nameText.text);
        }
    }
}
