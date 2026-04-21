using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIMessageButton : MonoBehaviour
    {
        [SerializeField] Image image;

        Vector3 targetPosition = Vector3.zero;
        CameraController cameraController;

        void Start()
        {
            cameraController = CameraController.Instance;
        }

        public void Init(Vector3 position)
        {
            targetPosition = position;
        }

        public void Init(Vector3 position, Color color)
        {
            targetPosition = position;
            SetImageColor(color);
        }

        public void ShowTarget()
        {
            cameraController.transform.position = new Vector3(targetPosition.x, 0, targetPosition.z);
        }

        public void SetImageColor(Color c)
        {
            if (!image)
                return;

            image.color = c;
        }
    }
}
