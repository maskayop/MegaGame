using UnityEngine;

namespace MegaGame.UI
{
    public class UIMessagePanel : MonoBehaviour
    {
        [SerializeField] RectTransform container;

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

            foreach (Transform t in container.transform)
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

        public void SpawnNekarkMessage()
        {
            SpawnMessage(nekarkMessagePrefab, nekark.GetMessageText());
        }

        public void SpawnNafaivelMessage()
        {
            SpawnMessage(nafaivelMessagePrefab, nafaivel.GetMessageText());
        }

        public void SpawnFortConstructionMessage(Port port)
        {
            string txt = uiColors.GetTextOwnerColorString(port.owner) + port.Island.islandData.islandName.GetLocalizedString() +
                " " + uiColors.GetDefaultColorString() + fortConstruction.GetMessageText();
            SpawnMessage(fortConstructionMessagePrefab, txt, uiColors.GetTextOwnerColor(port.owner));
        }

        public void SpawnTraderConstructionMessage(Port port)
        {
            string txt = uiColors.GetTextOwnerColorString(port.owner) + port.Island.islandData.islandName.GetLocalizedString() +
                " " + uiColors.GetDefaultColorString() + traderConstruction.GetMessageText();
            SpawnMessage(traderConstructionMessagePrefab, txt, uiColors.GetTextOwnerColor(port.owner));
        }

        public void SpawnPiratesAttackVillageMessage(Village village)
        {
            string txt = piratesAttackVillage.GetMessageText() + uiColors.GetTextOwnerColorString(village.owner) +
                village.Island.islandData.islandName.GetLocalizedString();
            SpawnMessage(piratesAttackVillageMessagePrefab, txt);
        }

        void SpawnMessage(GameObject prefab, string text)
        {
            GameObject mes = Instantiate(prefab, container);
            UIMessageObject messageObject = mes.GetComponent<UIMessageObject>();
            messageObject.SetText(text);

            soundPlayer?.PlayClip();
        }

        void SpawnMessage(GameObject prefab, string text, Color color)
        {
            GameObject mes = Instantiate(prefab, container);
            UIMessageObject messageObject = mes.GetComponent<UIMessageObject>();
            messageObject.SetText(text);
            messageObject.SetImageColor(color);

            soundPlayer?.PlayClip();
        }
    }
}
