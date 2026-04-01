using MegaGame.UI;
using UnityEngine;
using UnityEngine.Events;

namespace MegaGame
{
    public class ModelButton : MonoBehaviour
    {
        public UnityEvent onClick;

        void Update()
        {
            if (!GameController.Instance)
                return;

            if (GameController.Instance && GameController.Instance.gameState == GameController.GameState.menu)
                return;

            if (!UIGameShop.Instance)
                return;

            if (UIGameShop.Instance && UIGameShop.Instance.IsOpen)
                return;

            SelectObject();
        }

        void SelectObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CameraController.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000000, 1 << 9))
                {
                    ModelButton mb = hit.collider.GetComponentInParent<ModelButton>();

                    if (mb == this)
                        OnClickAction();
                }
            }
        }

        void OnClickAction()
        {
            onClick.Invoke();
        }

        public void StartBattle()
        {
            UIMainCanvas.Instance.StartBattle();

            if (Tutorial.Instance)
                if (Tutorial.Instance.isTutorial)
                    UITutorialWindow.Instance.ShowNextChapter();
        }
    }
}
