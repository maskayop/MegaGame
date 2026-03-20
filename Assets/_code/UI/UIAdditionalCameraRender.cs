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

        void Start()
        {
            additionalSceneObjects = AdditionalSceneObjects.Instance;

            victoryRawImage.texture = additionalSceneObjects.victoryAdditionalCamera.GetRenderTexture();
            defeatRawImage.texture = additionalSceneObjects.defeatAdditionalCamera.GetRenderTexture();
            shopRawImage.texture = additionalSceneObjects.shopAdditionalCamera.GetRenderTexture();
            shopPremiumRawImage.texture = additionalSceneObjects.shopPremiumAdditionalCamera.GetRenderTexture();
        }
    }
}
