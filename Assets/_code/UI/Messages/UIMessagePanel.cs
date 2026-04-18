using UnityEngine;

namespace MegaGame.UI
{
    public class UIMessagePanel : MonoBehaviour
    {
        [SerializeField] RectTransform textMessagesContainer;
        [SerializeField] RectTransform messageButtonsContainer;

        [Header("Message Prefabs")]
        [SerializeField] GameObject warningMessagePrefab;
        [SerializeField] GameObject nekarkMessagePrefab;
        [SerializeField] GameObject nafaivelMessagePrefab;
        [SerializeField] GameObject fortConstructionMessagePrefab;
        [SerializeField] GameObject traderConstructionMessagePrefab;
        [SerializeField] GameObject piratesAttackVillageMessagePrefab;

        [Header("Messages")]
        [SerializeField] Data_Message tooFarFromPort;
        [SerializeField] Data_Message wrongTargetPort;
        [SerializeField] Data_Message nekark;
        [SerializeField] Data_Message nafaivel;
        [SerializeField] Data_Message fortConstruction;
        [SerializeField] Data_Message traderConstruction;
        [SerializeField] Data_Message piratesAttackVillage;

        [Header("Message Buttons")]
        [SerializeField] GameObject nekarkMessageButtonPrefab;
        [SerializeField] GameObject nafaivelMessageButtonPrefab;
        [SerializeField] GameObject fortConstructionMessageButtonPrefab;
        [SerializeField] GameObject traderConstructionMessageButtonPrefab;
        [SerializeField] GameObject piratesAttackVillageMessageButtonPrefab;

        UIColors uiColors;
        UISoundPlayer soundPlayer;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            uiColors = UIColors.Instance;
            soundPlayer = GetComponent<UISoundPlayer>();

            foreach (Transform t in textMessagesContainer.transform)
                Destroy(t.gameObject);

            foreach (Transform t in messageButtonsContainer.transform)
                Destroy(t.gameObject);
        }

        public void SpawnTooFarFromPortMessage()
        {
            SpawnMessage(warningMessagePrefab, tooFarFromPort.GetMessageText());
        }

        public void SpawnWrongTargetPortMessage(Island target)
        {
            string txt = wrongTargetPort.GetMessageText() + uiColors.GetTextOwnerColorString(target.owner)
                + target.islandData.islandName.GetLocalizedString();
            SpawnMessage(warningMessagePrefab, txt);
        }

        public void SpawnNekarkMessage(Vector3 targetPosition)
        {
            SpawnMessage(nekarkMessagePrefab, nekark.GetMessageText());
            SpawnMessageButton(nekarkMessageButtonPrefab, targetPosition);
        }

        public void SpawnNafaivelMessage(Vector3 targetPosition)
        {
            SpawnMessage(nafaivelMessagePrefab, nafaivel.GetMessageText());
            SpawnMessageButton(nafaivelMessageButtonPrefab, targetPosition);
        }

        public void SpawnFortConstructionMessage(Port port)
        {
            string txt = uiColors.GetTextOwnerColorString(port.owner) + port.Island.islandData.islandName.GetLocalizedString() +
                " " + uiColors.GetDefaultColorString() + fortConstruction.GetMessageText();
            SpawnMessage(fortConstructionMessagePrefab, txt, uiColors.GetTextOwnerColor(port.owner));
            SpawnMessageButton(fortConstructionMessageButtonPrefab, port.transform.position);
        }

        public void SpawnTraderConstructionMessage(Port port)
        {
            string txt = uiColors.GetTextOwnerColorString(port.owner) + port.Island.islandData.islandName.GetLocalizedString() +
                " " + uiColors.GetDefaultColorString() + traderConstruction.GetMessageText();
            SpawnMessage(traderConstructionMessagePrefab, txt, uiColors.GetTextOwnerColor(port.owner));
            SpawnMessageButton(traderConstructionMessageButtonPrefab, port.transform.position);
        }

        public void SpawnPiratesAttackVillageMessage(Village village)
        {
            string txt = piratesAttackVillage.GetMessageText() + uiColors.GetTextOwnerColorString(village.owner) +
                village.Island.islandData.islandName.GetLocalizedString();
            SpawnMessage(piratesAttackVillageMessagePrefab, txt);
            SpawnMessageButton(piratesAttackVillageMessageButtonPrefab, village.transform.position);
        }

        void SpawnMessage(GameObject prefab, string text)
        {
            GameObject mes = Instantiate(prefab, textMessagesContainer);
            UIMessageObject messageObject = mes.GetComponent<UIMessageObject>();
            messageObject.SetText(text);

            soundPlayer?.PlayClip();
        }

        void SpawnMessage(GameObject prefab, string text, Color color)
        {
            GameObject mes = Instantiate(prefab, textMessagesContainer);
            UIMessageObject messageObject = mes.GetComponent<UIMessageObject>();
            messageObject.SetText(text);
            messageObject.SetImageColor(color);

            soundPlayer?.PlayClip();
        }

        void SpawnMessageButton(GameObject prefab, Vector3 targetPosition)
        {
            GameObject mes = Instantiate(prefab, messageButtonsContainer);
            UIMessageButton messageButton = mes.GetComponent<UIMessageButton>();
            messageButton.Init(targetPosition);
        }
    }
}
