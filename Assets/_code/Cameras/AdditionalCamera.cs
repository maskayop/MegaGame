using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class AdditionalCamera : MonoBehaviour
    {
        [SerializeField] int width = 512;
        [SerializeField] int height = 512;

        [SerializeField] List<Camera> cameras = new List<Camera>();

        RenderTexture renderTexture;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            renderTexture = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
            renderTexture.autoGenerateMips = false;
            renderTexture.useMipMap = false;
            renderTexture.antiAliasing = 1;
            renderTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt;
            renderTexture.Create();

            SetRenderTextureToAllCameras();
        }

        void OnDestroy()
        {
            ReleaseAllCamerasRenderTexture();
        }

        public RenderTexture GetRenderTexture()
        {
            return renderTexture;
        }

        public void SetRenderTextureToAllCameras()
        {
            for (int i = 0; i < cameras.Count; i++)
                cameras[i].targetTexture = renderTexture;
        }

        void ReleaseAllCamerasRenderTexture()
        {
            for (int i = 0; i < cameras.Count; i++)
                if (cameras[i] != null && cameras[i].targetTexture != null)
                {
                    cameras[i].targetTexture.Release();
                    Destroy(cameras[i].targetTexture);
                }
        }
    }
}
