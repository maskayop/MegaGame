using TMPro;
using UnityEngine;

namespace MegaGame.UI
{
    public class UILoadAccountButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;

        UIMainMenu mainMenu;
        UIAccountManagerWindow accountManagerWindow;

        void Start()
        {
            accountManagerWindow = UIAccountManagerWindow.Instance;
            mainMenu = UIMainMenu.Instance;
        }

        public void Init(string accountName)
        {
            nameText.text = accountName;
        }

        public void LoadAccount()
        {
            mainMenu.LoadAccount(nameText.text);
        }

        public void TryRenameAccount()
        {
            accountManagerWindow.RenameAccount();
        }

        public void TryDeleteAccount()
        {
            accountManagerWindow.DeleteAccount();
        }
    }
}
