using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class AdditionalCamera : MonoBehaviour
    {
        [SerializeField] int width = 512;
        [SerializeField] int height = 512;

        [SerializeField] List<Camera> cameras = new List<Camera>();

        RenderTextureFormat colorFormat = RenderTextureFormat.ARGB32;
        RenderTexture renderTexture;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            CreateRenderTexture();
            SetRenderTextureToAllCameras();
        }

        void CreateRenderTexture()
        {
            int depthBits = 24;

            // Пробуем 32 бита, но если не получится — падаем на 16
            RenderTexture testRT = new RenderTexture(64, 64, 32, colorFormat);

            if (!testRT.Create())
            {
                depthBits = 16;  // 16 бит работает абсолютно везде
                Debug.LogWarning("32-bit depth не поддерживается, используем 16-bit");
            }
            else
                testRT.Release();

            // Создаём финальную текстуру с правильной глубиной
            renderTexture = new RenderTexture(width, height, depthBits, colorFormat);
            renderTexture.autoGenerateMips = false;
            renderTexture.useMipMap = false;
            renderTexture.antiAliasing = 1;
            renderTexture.Create();
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
