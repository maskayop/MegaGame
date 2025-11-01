using UnityEngine;

namespace MegaGame
{
	public class HealthIndicatorWidget : MonoBehaviour
	{
		[SerializeField] bool useLookAt = true;
		[SerializeField] MeshRenderer meshRenderer;
		[SerializeField] string floatValueName;

		float value;

		void Update()
		{
			if (useLookAt)
				transform.LookAt(CameraController.Instance.mainCamera.transform.position);
			else
			{
				transform.rotation = CameraController.Instance.mainCamera.transform.rotation;
				transform.Rotate(180, 0, 180);
            }
			
			meshRenderer.material.SetFloat(floatValueName, value);
		}

		public void SetValue(float floatValue)
		{
			value = floatValue;
		}
	}
}
