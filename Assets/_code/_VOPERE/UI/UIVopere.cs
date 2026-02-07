using UnityEngine;

namespace Vopere.UI
{

    public class UIVopere : MonoBehaviour
    {
        [SerializeField] GameObject window;

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) &&
                Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.Alpha0))
                ShowWindow(true);
            else
                ShowWindow(false);
        }

        public void ShowWindow(bool state)
        {
            window.SetActive(state);
        }
    }
}
