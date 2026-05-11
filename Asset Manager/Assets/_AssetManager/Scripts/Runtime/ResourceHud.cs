using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class ResourceHud : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text resourceText;

        [SerializeField]
        private Text messageText;

        public Text ResourceText => resourceText;
        public Text MessageText => messageText;

        public void Bind(GameObject hudPanel, Text resources, Text message)
        {
            panel = hudPanel;
            resourceText = resources;
            messageText = message;
        }

        public void Show(RunSessionState run, string message)
        {
            if (run == null)
            {
                SetActive(panel, false);
                return;
            }

            SetActive(panel, true);
            var resources = run.Resources;
            var resourceConfig = run.StaticData.ResourceConfig;

            SetText(
                resourceText,
                $"보유 자원 | 현금 {resources.Cash} | 리서치 {resources.Research} | 신용 {resources.Credit} | 원자재 {resources.Commodity} | 전문 자원 {resources.ProfessionalTotal}/{resourceConfig.ProfessionalResourceCap} | 딜 {resources.Deal}/{resourceConfig.MaxDeal}");
            SetText(messageText, message ?? string.Empty);
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
