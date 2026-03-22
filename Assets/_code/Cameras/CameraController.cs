using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Vopere.Common;

namespace MegaGame
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        [Header("Cameras")]
        public Camera mainCamera;
        [SerializeField] List<CinemachineCamera> virtualCameras = new List<CinemachineCamera>();

        [Header("Movement")]
        [SerializeField] float movementSpeed = 1.0f;
        [SerializeField] Vector2 positionLimits = new Vector2(100f, 100f);
        [SerializeField] Vector3 zoomInfluence = new Vector3(1f, 0f, 1f);

        [Header("Zoom")]
        [SerializeField] float scrollSpeed = 1;
        [SerializeField] float currentZoom = 0.5f;
        [SerializeField] Vector2Int baseTranslationZ = new Vector2Int(100, 500);
        [SerializeField] float doubleTouchZoomSpeed = 1.0f;

        Vector2Int translationZ = new Vector2Int(100, 500);

        [Header("Lens")]
        [SerializeField] float lensFOV = 60;
        [SerializeField] int farClipPlane = 1000;

        bool freeze = false;
        bool scrollLock = false;

        int currentCamera;

        Vector2 startMousePosition;
        Vector2 currentMousePosition;
        Vector2 mousePositionOffset;

        Vector3 startCameraPosition;

        Vector3 currentCameraRotation;
        float cos;
        float sin;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create CameraController");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Init();
        }

        void Update()
        {
            if (freeze)
                return;

            MoveCamera();
            ZoomView();
        }

        public void Init()
        {
            for (int i = 0; i < virtualCameras.Count; i++)
            {
                virtualCameras[i].Lens.FieldOfView = lensFOV;
                virtualCameras[i].Lens.FarClipPlane = farClipPlane;
            }

            float movementSensitivity = DataSaveLoad.Instance.GetSavedFloat("MovementSensitivity");

            if (movementSensitivity != -1)
                ChangeMovementSensitivity(movementSensitivity);

            SetTranslationZToBase();
        }

        void MoveCamera()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                startCameraPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            }

            if (Input.GetMouseButton(0))
            {
                currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                currentCameraRotation = mainCamera.transform.rotation.eulerAngles;
                cos = Mathf.Cos(currentCameraRotation.y * Mathf.PI / 180);
                sin = Mathf.Sin(currentCameraRotation.y * Mathf.PI / 180);

                mousePositionOffset = (startMousePosition - currentMousePosition) * movementSpeed * Mathf.Clamp(currentZoom * zoomInfluence.x, zoomInfluence.y, zoomInfluence.z);

                float positionX = mousePositionOffset.x * cos + mousePositionOffset.y * sin;
                float positionZ = mousePositionOffset.y * cos - mousePositionOffset.x * sin;

                positionX = Mathf.Clamp(startCameraPosition.x + positionX, -positionLimits.x, positionLimits.x);
                positionZ = Mathf.Clamp(startCameraPosition.z + positionZ, -positionLimits.y, positionLimits.y);

                transform.position = new Vector3(positionX, startCameraPosition.y, positionZ);
            }

            if (Input.GetMouseButtonUp(0))
                startCameraPosition = transform.position;
        }

        void SetCamera()
        {
            for (int i = 0; i < virtualCameras.Count; i++)
            {
                if (i == currentCamera)
                    virtualCameras[i].Priority = 1;
                else
                    virtualCameras[i].Priority = 0;
            }
        }

        public void GoToCamera(bool isNext)
        {
            if (isNext)
                currentCamera++;
            else
                currentCamera--;

            if (currentCamera >= virtualCameras.Count)
                currentCamera = 0;
            else if (currentCamera < 0)
                currentCamera = (virtualCameras.Count - 1);

            SetCamera();
        }

        void ZoomView()
        {
            if (!scrollLock)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                    currentZoom -= scrollSpeed;
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    currentZoom += scrollSpeed;

                currentZoom = Mathf.Clamp01(currentZoom);

#if PLATFORM_ANDROID
                HandleZoom();
#endif
            }

            for (int i = 0; i < virtualCameras.Count; i++)
                virtualCameras[i].transform.localPosition = Vector3.Lerp(new Vector3(0, 0, -translationZ.x), new Vector3(0, 0, -translationZ.y), currentZoom);
        }

        void HandleZoom()
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                float prevDistance = Vector2.Distance(touch1PrevPos, touch2PrevPos);
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                float difference = currentDistance - prevDistance;
                currentZoom -= difference * doubleTouchZoomSpeed * scrollSpeed;
            }
        }

        public void CameraZoom(float INvalue)
        {
            if (scrollLock)
                return;

            currentZoom += scrollSpeed * INvalue;
            currentZoom = Mathf.Clamp01(currentZoom);
        }

        public float GetCameraZoom()
        {
            return currentZoom;
        }

        public void Freeze(bool state)
        {
            freeze = state;
        }

        public void ChangeMovementSensitivity(float INvalue)
        {
            movementSpeed = INvalue / 100;
            DataSaveLoad.Instance.Save("MovementSensitivity", INvalue);
        }

        public void ChangeZoomSensitivity(float INvalue)
        {
            scrollSpeed = INvalue / 100;
            DataSaveLoad.Instance.Save("ZoomSensitivity", INvalue);
        }

        public void SetTranslationZToBase()
        {
            translationZ = baseTranslationZ;
            scrollLock = false;
            currentZoom = 0.5f;
        }

        public void SetTranslationZToMax()
        {
            translationZ = new Vector2Int(baseTranslationZ.y, baseTranslationZ.y);
            scrollLock = true;
            currentZoom = 1.0f;
        }

        public void SetFarClipPlaneToZero(bool isZero)
        {
            for (int i = 0; i < virtualCameras.Count; i++)
                if (isZero)
                    virtualCameras[i].Lens.FarClipPlane = virtualCameras[i].Lens.NearClipPlane + 0.1f;
                else
                    virtualCameras[i].Lens.FarClipPlane = farClipPlane;
        }
    }
}
