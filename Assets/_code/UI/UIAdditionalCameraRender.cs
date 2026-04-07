using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIAdditionalCameraRender : MonoBehaviour
    {
        [SerializeField] RawImage victoryRawImage;
        [SerializeField] RawImage defeatRawImage;
        [SerializeField] RawImage shopRawImage;
        [SerializeField] RawImage shopPremiumRawImage;

        AdditionalSceneObjects additionalSceneObjects;
        AdditionalCamera additionalCamera;

        void Start()
        {
            additionalSceneObjects = AdditionalSceneObjects.Instance;
            additionalCamera = additionalSceneObjects.GetComponent<AdditionalCamera>();

            victoryRawImage.texture = additionalCamera.GetRenderTexture();
            defeatRawImage.texture = additionalCamera.GetRenderTexture();
            shopRawImage.texture = additionalCamera.GetRenderTexture();
            shopPremiumRawImage.texture = additionalCamera.GetRenderTexture();
        }
    }
}
