using UnityEngine;

namespace MegaGame
{
    public class HealthIndicatorWidget : MonoBehaviour
    {
        [SerializeField] bool useLookAt = true;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] string floatValueName;

        float value;

        CameraController cameraController;

        void Start()
        {
            cameraController = CameraController.Instance;
        }

        void Update()
        {
            if (!cameraController)
                return;

            if (useLookAt)
                transform.LookAt(cameraController.mainCamera.transform.position);
            else
            {
                transform.rotation = cameraController.mainCamera.transform.rotation;
                transform.Rotate(180, 0, 180);
            }

            meshRenderer.material.SetFloat(floatValueName, value);
        }

        public void SetValue(float floatValue)
        {
            value = Mathf.Clamp01(floatValue);
        }
    }
}
