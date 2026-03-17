using UnityEngine;

namespace MegaGame
{
    [RequireComponent(typeof(Camera))]
    public class AdditionalCamera : MonoBehaviour
    {
        public int width = 512;
        public int height = 512;

        Camera cam;
        RenderTexture renderTexture;

        void Awake()
        {
            cam = GetComponent<Camera>();

            renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);

            renderTexture.autoGenerateMips = false;
            renderTexture.useMipMap = false;
            renderTexture.antiAliasing = 1;
            renderTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt;

            renderTexture.Create();
            cam.targetTexture = renderTexture;
        }

        void OnDestroy()
        {
            if (cam != null && cam.targetTexture != null)
            {
                cam.targetTexture.Release();
                Destroy(cam.targetTexture);
            }
        }

        public RenderTexture GetRenderTexture()
        {
            return renderTexture;
        }
    }
}
