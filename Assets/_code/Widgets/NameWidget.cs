using UnityEngine;
using TMPro;

namespace MegaGame
{
	public class NameWidget : MonoBehaviour
	{
		[SerializeField] TextMeshPro text;
		[SerializeField] float minScale = 0;
		[SerializeField] bool useLookAt = false;

		[Header("Inverse")]
		[SerializeField] bool inverseScaling = false;
		[SerializeField] float scaleForDisabling = 1;

		[Header("Colors")]
		[SerializeField] Color playerColor = Color.white;
		[SerializeField] Color enemyColor = Color.white;
		[SerializeField] Color neutralColor = Color.white;

        void Update()
		{
			if (transform.localScale.x < scaleForDisabling)
				text.gameObject.SetActive(false);
			else
				text.gameObject.SetActive(true);

			if (useLookAt)
				transform.LookAt(CameraController.Instance.mainCamera.transform.position);
			else
			{
				transform.rotation = CameraController.Instance.mainCamera.transform.rotation;
				transform.Rotate(180, 0, 180);
			}

			if (!inverseScaling)
				transform.localScale = Vector3.one * Mathf.Clamp(CameraController.Instance.GetCameraZoom(), minScale, 1);
			else
				transform.localScale = Vector3.one * Mathf.Clamp((1 - CameraController.Instance.GetCameraZoom()), 0, minScale);
		}

		public void SetText(string nameText)
		{
            text.text = nameText;
		}

		public void SetColor(BaseCharacter.Owner owner)
		{
			if (owner == BaseCharacter.Owner.player)
				text.color = playerColor;
			else if (owner == BaseCharacter.Owner.enemy)
				text.color = enemyColor;
			else if (owner == BaseCharacter.Owner.neutral)
				text.color = neutralColor;

        }
	}
}
