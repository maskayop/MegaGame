using UnityEngine;

namespace MegaGame.UI
{
    public class UIMessageButton : MonoBehaviour
    {
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

        public void ShowTarget()
        {
            cameraController.transform.position = new Vector3(targetPosition.x, 0, targetPosition.z);
        }
    }
}
