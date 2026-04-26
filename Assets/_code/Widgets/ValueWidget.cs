using TMPro;
using UnityEngine;

namespace MegaGame
{
    public class ValueWidget : MonoBehaviour
    {
        [SerializeField] GameObject container;
        [SerializeField] TextMeshPro text;

        [Header("Transform")]
        [SerializeField] float minScale = 0;
        [SerializeField] bool useLookAt = false;
        [SerializeField] Camera cameraOverride;

        [Header("Inverse")]
        [SerializeField] bool inverseScaling = false;
        [SerializeField] float scaleForDisabling = 1;

        [Header("Values")]
        [SerializeField] Vector2Int randomValue = Vector2Int.zero;

        void Update()
        {
            if (transform.localScale.x < scaleForDisabling)
                container.SetActive(false);
            else
                container.SetActive(true);

            if (useLookAt)
            {
                if (cameraOverride)
                    container.transform.LookAt(cameraOverride.transform.position);
                else
                    container.transform.LookAt(CameraController.Instance.mainCamera.transform.position);
            }
            else
            {
                container.transform.rotation = CameraController.Instance.mainCamera.transform.rotation;
                container.transform.Rotate(180, 0, 180);
            }

            if (!inverseScaling)
                transform.localScale = Vector3.one * Mathf.Clamp(CameraController.Instance.GetCameraZoom(), minScale, 1);
            else
                transform.localScale = Vector3.one * Mathf.Clamp(1 - CameraController.Instance.GetCameraZoom(), 0, minScale);
        }

        public void SetText(string nameText)
        {
            if (!text)
                return;

            text.text = nameText;
        }

        void SetMeshRendererColor(MeshRenderer INrenderer, string INvalueName, Color INcolor)
        {
            if (!INrenderer)
                return;

            INrenderer.material.SetColor(INvalueName, INcolor);
        }

        public void SetRandomPreviewProfitValue()
        {
            int random = Random.Range(randomValue.x, randomValue.y);
            SetText("+" + random.ToString());
        }
    }
}
