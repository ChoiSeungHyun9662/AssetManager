using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssetManager
{
    public sealed class MissionCandidateView : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        private readonly List<Text> slotTexts = new List<Text>();
        private readonly List<Button> mulliganButtons = new List<Button>();
        private readonly List<Button> discardButtons = new List<Button>();
        private Action<int> onMulliganRequested;
        private Action<int> onDiscardRequested;

        public GameObject Panel => panel;
        public IReadOnlyList<Text> SlotTexts => slotTexts;
        public IReadOnlyList<Button> MulliganButtons => mulliganButtons;
        public IReadOnlyList<Button> DiscardButtons => discardButtons;

        public void Bind(
            GameObject missionPanel,
            IEnumerable<Text> missionSlotTexts,
            IEnumerable<Button> missionMulliganButtons,
            IEnumerable<Button> missionDiscardButtons)
        {
            panel = missionPanel;
            Replace(slotTexts, missionSlotTexts);
            Replace(mulliganButtons, missionMulliganButtons);
            Replace(discardButtons, missionDiscardButtons);
        }

        public void SetActionHandlers(Action<int> mulliganHandler, Action<int> discardHandler)
        {
            onMulliganRequested = mulliganHandler;
            onDiscardRequested = discardHandler;
        }

        public void Show(RunSessionState run)
        {
            var hasRun = run != null && run.State == RunState.Playing && run.Missions.CandidateSlots.Count > 0;
            SetActive(panel, hasRun);
            if (!hasRun)
            {
                return;
            }

            if (run.Missions.HasConfirmedMission)
            {
                ShowConfirmedMission(run.Missions.ConfirmedMission);
                return;
            }

            for (var i = 0; i < slotTexts.Count; i++)
            {
                var hasSlot = i < run.Missions.CandidateSlots.Count;
                SetActive(slotTexts[i].transform.parent.gameObject, hasSlot);
                if (!hasSlot)
                {
                    continue;
                }

                var slot = run.Missions.CandidateSlots[i];
                slotTexts[i].text = FormatSlot(slot);
                ConfigureButton(mulliganButtons[i], i, !slot.IsEmpty && !slot.HasSpentMulligan, onMulliganRequested);
                ConfigureButton(discardButtons[i], i, !slot.IsEmpty && slot.HasSpentMulligan, onDiscardRequested);
            }
        }

        private void ShowConfirmedMission(MissionDefinitionData confirmedMission)
        {
            for (var i = 0; i < slotTexts.Count; i++)
            {
                var isConfirmedSlot = i == 0;
                SetActive(slotTexts[i].transform.parent.gameObject, isConfirmedSlot);
                slotTexts[i].text = isConfirmedSlot ? FormatConfirmedMission(confirmedMission) : string.Empty;
                ConfigureButton(mulliganButtons[i], i, false, onMulliganRequested);
                ConfigureButton(discardButtons[i], i, false, onDiscardRequested);
            }
        }

        private static string FormatSlot(MissionCandidateSlotState slot)
        {
            if (slot.IsEmpty)
            {
                return "빈 미션 슬롯";
            }

            var mission = slot.Candidate;
            var builder = new StringBuilder();
            builder.AppendLine(mission.DisplayName);
            builder.Append("Tags: ");
            builder.AppendLine(FormatTags(mission.TargetTags));
            builder.Append("Clear: ");
            builder.AppendLine(mission.ClearConditionDescription);
            builder.Append("Formula: ");
            builder.AppendLine(mission.SettlementFormulaDescription);
            builder.Append("Difficulty: ");
            builder.Append(mission.DifficultyDisplay);
            return builder.ToString();
        }

        private static string FormatConfirmedMission(MissionDefinitionData mission)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Confirmed Mission");
            builder.Append(FormatSlot(new MissionCandidateSlotState(mission, true)));
            return builder.ToString();
        }

        private static string FormatTags(IReadOnlyList<TagData> tags)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < tags.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(" / ");
                }

                builder.Append(tags[i].DisplayName);
            }

            return builder.ToString();
        }

        private static void ConfigureButton(Button button, int slotIndex, bool interactable, Action<int> handler)
        {
            button.interactable = interactable;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => handler?.Invoke(slotIndex));
        }

        private static void Replace<T>(List<T> target, IEnumerable<T> source)
        {
            target.Clear();
            target.AddRange(source);
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}
