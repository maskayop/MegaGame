using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIAdditionalCameraRender : MonoBehaviour
    {
        [SerializeField] AdditionalCamera additionalCamera;
        [SerializeField] RawImage rawImage;

        void Start()
        {
            rawImage.texture = additionalCamera.rt;
        }
    }
}
