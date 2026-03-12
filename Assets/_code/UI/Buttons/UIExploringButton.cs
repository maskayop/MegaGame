using UnityEngine;

namespace MegaGame.UI
{
    public class UIExploringButton : MonoBehaviour
    {
        [SerializeField] GameObject imageOff;
        [SerializeField] GameObject imageOn;

        GameController gameController;

        bool isSelected;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            gameController = GameController.Instance;
            Select(false);
        }

        public void Switch()
        {
            Select(!isSelected);
        }

        public void Select(bool state)
        {
            isSelected = state;

            imageOn.SetActive(state);
            imageOff.SetActive(!state);

            gameController.SetGameModeAsExploring(state);
        }
    }
}
