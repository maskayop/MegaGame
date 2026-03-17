using UnityEngine;
using UnityEngine.UI;

namespace MegaGame.UI
{
    public class UIAdditionalCameraRender : MonoBehaviour
    {
        [SerializeField] RawImage victoryRawImage;
        [SerializeField] RawImage defeatRawImage;

        AdditionalSceneObjects additionalSceneObjects;

        void Start()
        {
            additionalSceneObjects = AdditionalSceneObjects.Instance;

            victoryRawImage.texture = additionalSceneObjects.victoryAdditionalCamera.GetRenderTexture();
            defeatRawImage.texture = additionalSceneObjects.defeatAdditionalCamera.GetRenderTexture();
        }
    }
}
