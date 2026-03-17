using UnityEngine;

namespace MegaGame
{
    [RequireComponent(typeof(Camera))]
    public class AdditionalCamera : MonoBehaviour
    {
        public Camera cam;
        public int width = 512;
        public int height = 512;
        public RenderTexture rt;

        void Awake()
        {
            rt = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);

            rt.autoGenerateMips = false;
            rt.useMipMap = false;
            rt.antiAliasing = 1;
            rt.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt;

            rt.Create();
            cam.targetTexture = rt;

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
        }

        void OnDestroy()
        {
            if (cam != null && cam.targetTexture != null)
            {
                cam.targetTexture.Release();
                Destroy(cam.targetTexture);
            }
        }
    }
}
