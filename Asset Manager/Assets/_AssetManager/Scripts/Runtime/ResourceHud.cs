using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class ResourceHud : MonoBehaviour
    {
        public const string DealGuideText = "드래그하여 투자 철학에 놓으면, 이 칩을 소모하여 해당 투자 철학 마스터리를 1 획득합니다.";

        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Text resourceText;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private Text cashText;

        [SerializeField]
        private Text researchText;

        [SerializeField]
        private Text creditText;

        [SerializeField]
        private Text commodityText;

        [SerializeField]
        private Text dealText;

        [Header("Chip Stack")]
        [SerializeField]
        private Vector2 chipStackOffset = new Vector2(18f, -14f);

        [SerializeField]
        private int chipsPerRow = 5;

        [Header("Manual Sprite Assets")]
        [SerializeField]
        private Sprite researchChipSprite = null;

        [SerializeField]
        private Sprite creditChipSprite = null;

        [SerializeField]
        private Sprite commodityChipSprite = null;

        [SerializeField]
        private Sprite dealChipSprite = null;

        [SerializeField]
        private Image cashImage;

        [SerializeField]
        private Image researchImage;

        [SerializeField]
        private Image creditImage;

        [SerializeField]
        private Image commodityImage;

        [SerializeField]
        private Image dealImage;

        [SerializeField]
        private GameObject dealGuidePanel;

        [SerializeField]
        private Text dealGuideText;

        [SerializeField]
        private Image dealDragImage;

        private bool warnedMissingSprites;
        private int currentDealCount;
        private bool isDraggingDeal;
        private Func<bool> canStartDealDrag;
        private Action<ResourceType> onDealDropped;

        public Text ResourceText => resourceText;
        public Text MessageText => messageText;

        public void Bind(
            GameObject hudPanel,
            Text resources,
            Text message,
            Text cash,
            Text research,
            Text credit,
            Text commodity,
            Text deal,
            Image cashSpriteImage,
            Image researchSpriteImage,
            Image creditSpriteImage,
            Image commoditySpriteImage,
            Image dealSpriteImage,
            GameObject dealGuidePanelObject,
            Text dealGuideLabel,
            Image dealDragSpriteImage)
        {
            panel = hudPanel;
            resourceText = resources;
            messageText = message;
            cashText = cash;
            researchText = research;
            creditText = credit;
            commodityText = commodity;
            dealText = deal;
            cashImage = cashSpriteImage;
            researchImage = researchSpriteImage;
            creditImage = creditSpriteImage;
            commodityImage = commoditySpriteImage;
            dealImage = dealSpriteImage;
            dealGuidePanel = dealGuidePanelObject;
            dealGuideText = dealGuideLabel;
            dealDragImage = dealDragSpriteImage;
        }

        public void SetDealDragHandlers(Func<bool> canStartDrag, Action<ResourceType> dropped)
        {
            canStartDealDrag = canStartDrag;
            onDealDropped = dropped;
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

            SetText(
                resourceText,
                string.Empty);
            SetText(cashText, resources.Cash + "$");
            SetText(researchText, FormatPhilosophy(resources.Reading, run.InvestmentPhilosophyMastery.Reading));
            SetText(creditText, FormatPhilosophy(resources.Meditation, run.InvestmentPhilosophyMastery.Meditation));
            SetText(commodityText, FormatPhilosophy(resources.Patience, run.InvestmentPhilosophyMastery.Patience));
            SetText(dealText, string.Empty);
            DisableImage(cashImage);
            currentDealCount = resources.Deal;
            ApplyChipStack(researchImage, researchChipSprite, resources.Research);
            ApplyChipStack(creditImage, creditChipSprite, resources.Credit);
            ApplyChipStack(commodityImage, commodityChipSprite, resources.Commodity);
            ApplyChipStack(dealImage, dealChipSprite, resources.Deal);
            ConfigureDealChipInteraction();
            WarnMissingSpritesOnce();
            SetText(messageText, message ?? string.Empty);
        }

        private void ConfigureDealChipInteraction()
        {
            SetActive(dealGuidePanel, false);
            SetActive(dealDragImage != null ? dealDragImage.gameObject : null, false);
            if (dealGuideText != null)
            {
                dealGuideText.text = DealGuideText;
            }

            if (dealImage == null)
            {
                return;
            }

            dealImage.raycastTarget = true;
            var trigger = dealImage.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = dealImage.gameObject.AddComponent<EventTrigger>();
            }

            trigger.triggers.Clear();
            AddTrigger(trigger, EventTriggerType.PointerEnter, eventData =>
            {
                if (CanStartDealDrag())
                {
                    ShowDealGuide(((PointerEventData)eventData).position);
                }
            });
            AddTrigger(trigger, EventTriggerType.PointerExit, _ =>
            {
                if (!isDraggingDeal)
                {
                    HideDealGuide();
                }
            });
            AddTrigger(trigger, EventTriggerType.PointerDown, eventData => BeginDealDrag((PointerEventData)eventData));
            AddTrigger(trigger, EventTriggerType.Drag, eventData => DragDeal((PointerEventData)eventData));
            AddTrigger(trigger, EventTriggerType.PointerUp, eventData => EndDealDrag((PointerEventData)eventData));
        }

        private static void AddTrigger(
            EventTrigger trigger,
            EventTriggerType eventType,
            Action<BaseEventData> action)
        {
            var entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(eventData => action(eventData));
            trigger.triggers.Add(entry);
        }

        private bool CanStartDealDrag()
        {
            return currentDealCount > 0
                && dealChipSprite != null
                && (canStartDealDrag == null || canStartDealDrag());
        }

        private void BeginDealDrag(PointerEventData eventData)
        {
            if (!CanStartDealDrag())
            {
                return;
            }

            isDraggingDeal = true;
            if (dealImage != null)
            {
                dealImage.enabled = false;
            }

            if (dealDragImage != null)
            {
                dealDragImage.sprite = dealChipSprite;
                dealDragImage.enabled = true;
                dealDragImage.gameObject.SetActive(true);
                dealDragImage.transform.SetAsLastSibling();
            }

            MoveDealDragVisuals(eventData.position);
        }

        private void DragDeal(PointerEventData eventData)
        {
            if (!isDraggingDeal)
            {
                return;
            }

            MoveDealDragVisuals(eventData.position);
        }

        private void EndDealDrag(PointerEventData eventData)
        {
            if (!isDraggingDeal)
            {
                return;
            }

            isDraggingDeal = false;
            var hasDropTarget = TryGetPhilosophyDropTarget(eventData.position, out var resourceType);
            HideDealGuide();
            SetActive(dealDragImage != null ? dealDragImage.gameObject : null, false);
            if (dealImage != null)
            {
                dealImage.enabled = currentDealCount > 0 && dealChipSprite != null;
            }

            if (hasDropTarget)
            {
                onDealDropped?.Invoke(resourceType);
            }
        }

        private void MoveDealDragVisuals(Vector2 screenPosition)
        {
            if (dealDragImage != null)
            {
                dealDragImage.GetComponent<RectTransform>().position = screenPosition;
            }

            ShowDealGuide(screenPosition);
        }

        private void ShowDealGuide(Vector2 screenPosition)
        {
            if (dealGuidePanel == null)
            {
                return;
            }

            dealGuidePanel.SetActive(true);
            var rectTransform = dealGuidePanel.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(1f, 0f);
            rectTransform.position = screenPosition;
        }

        private void HideDealGuide()
        {
            SetActive(dealGuidePanel, false);
        }

        private bool TryGetPhilosophyDropTarget(Vector2 screenPosition, out ResourceType resourceType)
        {
            if (ContainsScreenPoint(researchText, screenPosition) || ContainsScreenPoint(researchImage, screenPosition))
            {
                resourceType = ResourceType.Reading;
                return true;
            }

            if (ContainsScreenPoint(creditText, screenPosition) || ContainsScreenPoint(creditImage, screenPosition))
            {
                resourceType = ResourceType.Meditation;
                return true;
            }

            if (ContainsScreenPoint(commodityText, screenPosition) || ContainsScreenPoint(commodityImage, screenPosition))
            {
                resourceType = ResourceType.Patience;
                return true;
            }

            resourceType = ResourceType.Cash;
            return false;
        }

        private static bool ContainsScreenPoint(Component component, Vector2 screenPosition)
        {
            if (component == null)
            {
                return false;
            }

            var rectTransform = component.GetComponent<RectTransform>();
            return rectTransform != null
                && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition);
        }

        private void WarnMissingSpritesOnce()
        {
            if (warnedMissingSprites)
            {
                return;
            }

            warnedMissingSprites = true;
            WarnMissingSprite(researchChipSprite, nameof(researchChipSprite));
            WarnMissingSprite(creditChipSprite, nameof(creditChipSprite));
            WarnMissingSprite(commodityChipSprite, nameof(commodityChipSprite));
            WarnMissingSprite(dealChipSprite, nameof(dealChipSprite));
        }

        private static void WarnMissingSprite(Sprite sprite, string fieldName)
        {
            if (sprite == null)
            {
                Debug.LogWarning("ResourceHud sprite not assigned: " + fieldName + "; resource chip image will be hidden.");
            }
        }

        private static void DisableImage(Image image)
        {
            if (image != null)
            {
                image.enabled = false;
            }
        }

        private void ApplyChipStack(Image baseImage, Sprite sprite, int count)
        {
            if (baseImage == null)
            {
                return;
            }

            var visibleCount = sprite == null ? 0 : count;
            for (var i = 0; i < visibleCount; i++)
            {
                var image = EnsureStackImage(baseImage, i);
                image.sprite = sprite;
                image.enabled = true;
                image.preserveAspect = true;
                image.raycastTarget = false;
                image.color = Color.white;

                var rectTransform = image.GetComponent<RectTransform>();
                var rowCapacity = Mathf.Max(1, chipsPerRow);
                var column = i % rowCapacity;
                var row = i / rowCapacity;
                rectTransform.anchoredPosition = baseImage.GetComponent<RectTransform>().anchoredPosition
                    + new Vector2(chipStackOffset.x * column, chipStackOffset.y * row);
            }

            DisableUnusedStackImages(baseImage, visibleCount);
        }

        private static Image EnsureStackImage(Image baseImage, int index)
        {
            if (index == 0)
            {
                return baseImage;
            }

            var childName = baseImage.name + " Stack " + index;
            var existing = baseImage.transform.parent.Find(childName);
            var imageObject = existing != null
                ? existing.gameObject
                : new GameObject(childName, typeof(RectTransform));

            imageObject.transform.SetParent(baseImage.transform.parent, false);
            imageObject.transform.SetSiblingIndex(baseImage.transform.GetSiblingIndex() + index);

            var sourceRect = baseImage.GetComponent<RectTransform>();
            var rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = sourceRect.anchorMin;
            rectTransform.anchorMax = sourceRect.anchorMax;
            rectTransform.pivot = sourceRect.pivot;
            rectTransform.sizeDelta = sourceRect.sizeDelta;

            var image = imageObject.GetComponent<Image>();
            if (image == null)
            {
                image = imageObject.AddComponent<Image>();
            }

            return image;
        }

        private static void DisableUnusedStackImages(Image baseImage, int usedCount)
        {
            baseImage.enabled = usedCount > 0;
            var prefix = baseImage.name + " Stack ";
            for (var i = 0; i < baseImage.transform.parent.childCount; i++)
            {
                var child = baseImage.transform.parent.GetChild(i);
                if (!child.name.StartsWith(prefix))
                {
                    continue;
                }

                var suffix = child.name.Substring(prefix.Length);
                if (int.TryParse(suffix, out var index) && index >= usedCount)
                {
                    var image = child.GetComponent<Image>();
                    if (image != null)
                    {
                        image.enabled = false;
                    }
                }
            }
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

        private static string FormatPhilosophy(int amount, int mastery)
        {
            if (mastery <= 0)
            {
                return amount.ToString();
            }

            return amount + " <size=14>+" + mastery + "</size>";
        }
    }
}
