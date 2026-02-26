using TMPro;
using UnityEngine;

namespace MegaGame
{
    public class NameWidget : MonoBehaviour
    {
        [SerializeField] GameObject container;
        [SerializeField] TextMeshPro text;

        [Header("Island Type Mesh")]
        [SerializeField] MeshRenderer islandTypeRenderer;
        [SerializeField] MeshRenderer defenderShipRenderer;
        [SerializeField] MeshRenderer defenceFortRenderer;

        [SerializeField] string materialValueName = "_BaseColor";

        [Header("Transform")]
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
                container.SetActive(false);
            else
                container.SetActive(true);

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
            if (!text)
                return;

            text.text = nameText;
        }

        public void SetDefenderShip(bool state)
        {
            if (!defenderShipRenderer)
                return;

            defenderShipRenderer.gameObject.SetActive(state);
        }

        public void SetDefenceFort(bool state)
        {
            if (!defenceFortRenderer)
                return;

            defenceFortRenderer.gameObject.SetActive(state);
        }

        public void SetColor(BaseCharacter.Owner owner)
        {
            if (owner == BaseCharacter.Owner.player)
            {
                if (text)
                    text.color = playerColor;

                if (islandTypeRenderer)
                    islandTypeRenderer.material.SetColor(materialValueName, playerColor);

                if (defenceFortRenderer)
                    defenceFortRenderer.material.SetColor(materialValueName, playerColor);
            }
            else if (owner == BaseCharacter.Owner.enemy)
            {
                if (text)
                    text.color = enemyColor;

                if (islandTypeRenderer)
                    islandTypeRenderer.material.SetColor(materialValueName, enemyColor);

                if (defenceFortRenderer)
                    defenceFortRenderer.material.SetColor(materialValueName, enemyColor);
            }
            else if (owner == BaseCharacter.Owner.neutral)
            {
                if (text)
                    text.color = neutralColor;

                if (islandTypeRenderer)
                    islandTypeRenderer.material.SetColor(materialValueName, neutralColor);

                if (defenceFortRenderer)
                    defenceFortRenderer.material.SetColor(materialValueName, neutralColor);
            }
        }

        public void SetDefenderShipColor(BaseCharacter.Owner owner)
        {
            if (!defenderShipRenderer)
                return;

            if (owner == BaseCharacter.Owner.player)
            {
                if (defenderShipRenderer)
                    defenderShipRenderer.material.SetColor(materialValueName, playerColor);
            }
            else if (owner == BaseCharacter.Owner.enemy)
            {
                if (defenderShipRenderer)
                    defenderShipRenderer.material.SetColor(materialValueName, enemyColor);
            }
            else if (owner == BaseCharacter.Owner.neutral)
            {
                if (defenderShipRenderer)
                    defenderShipRenderer.material.SetColor(materialValueName, neutralColor);
            }
        }
    }
}
