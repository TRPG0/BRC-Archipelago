using ModLocalizer;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.Components
{
    public class RandoRequirement : MonoBehaviour
    {
        public GameObject Trigger { get; private set; }
        public List<GraffitiSize> Sizes { get; private set; }
        public int Rep { get; private set; }

        public RequirementLinkType Type { get; private set; }
        public RequirementNeeds Needs { get; private set; }

        public Collider collider;

        public void InitGraffiti(List<GraffitiSize> sizes, GameObject originalTrigger, RequirementLinkType type = RequirementLinkType.Trigger)
        {
            Needs = RequirementNeeds.Graffiti;
            Sizes = sizes;
            Trigger = originalTrigger;
            collider = GetComponent<Collider>();
            collider.tag = Tags.Untagged;
            gameObject.name = "RandoRequirementTrigger";
            gameObject.SetActive(false);
        }

        public void InitREP(int rep, GameObject originalTrigger, RequirementLinkType type = RequirementLinkType.Trigger)
        {
            Needs = RequirementNeeds.REP;
            Rep = rep;
            Trigger = originalTrigger;
            collider = GetComponent<Collider>();
            collider.tag = Tags.Untagged;
            gameObject.name = "RandoRequirementTrigger";
            gameObject.SetActive(false);
        }

        public void InitBoth(List<GraffitiSize> sizes, int rep, GameObject originalTrigger, RequirementLinkType type = RequirementLinkType.Trigger)
        {
            Needs = RequirementNeeds.Both;
            Sizes = sizes;
            Rep = rep;
            Trigger = originalTrigger;
            collider = GetComponent<Collider>();
            collider.tag = Tags.Untagged;
            gameObject.name = "RandoRequirementTrigger";
            gameObject.SetActive(false);
        }

        public string SizesToString()
        {
            string sizes = string.Empty;
            foreach (GraffitiSize size in Sizes)
            {
                if (sizes == string.Empty) sizes = $"{size}";
                else sizes += $", {size}";
            }
            return sizes;
        }

        public bool HasGraffiti()
        {
            if (Sizes == null) return true;
            bool has = true;
            foreach (GraffitiSize size in Sizes)
            {
                if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(size)) has = false;
                break;
            }
            return has;
        }

        public bool HasRep()
        {
            return Core.Instance.Data.fakeRep >= Rep;
        }

        public bool IsActive()
        {
            return !gameObject.activeSelf;
        }

        public void EnableTrigger(RequirementNeeds source)
        {
            if (IsActive()) return;
            if (Needs == RequirementNeeds.Both)
            {
                if (HasGraffiti() && HasRep())
                {
                    if (Type == RequirementLinkType.Trigger) Trigger.SetActive(true);
                    else if (Type == RequirementLinkType.ProgressObject) Trigger.GetComponent<ProgressObject>().SetTriggerable(true);
                    gameObject.SetActive(false);
                    Core.Logger.LogInfo($"Enabled trigger of {transform.parent.name}");
                }
                else Core.Logger.LogInfo($"Can't enable trigger of {transform.parent.name} (Graffiti: {HasGraffiti()}, REP: {HasRep()})");
            }
            else if (source == Needs)
            {
                if (Type == RequirementLinkType.Trigger) Trigger.SetActive(true);
                else if (Type == RequirementLinkType.ProgressObject) Trigger.GetComponent<ProgressObject>().SetTriggerable(true);
                gameObject.SetActive(false);
                Core.Logger.LogInfo($"Enabled trigger of {transform.parent.name}");
            }
        }

        public void DisableTrigger()
        {
            if (!IsActive()) return;
            if (Type == RequirementLinkType.Trigger) Trigger.SetActive(false);
            else if (Type == RequirementLinkType.ProgressObject) Trigger.GetComponent<ProgressObject>().SetTriggerable(false);
            gameObject.SetActive(true);
            Core.Logger.LogInfo($"Disabled trigger of {transform.parent.name}");
        }

        public void ShowContext()
        {
            string context = "";
            if (Needs == RequirementNeeds.Graffiti) context = string.Format(Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "REQUIREMENT_GRAFFITI"), SizesToString());
            else if (Needs == RequirementNeeds.REP) context = string.Format(Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "REQUIREMENT_REP"), Rep);
            else if (Needs == RequirementNeeds.Both)
            {
                if (HasRep()) context = string.Format(Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "REQUIREMENT_GRAFFITI"), SizesToString());
                else context = string.Format(Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "REQUIREMENT_REP"), Rep);
            }

            Core.Instance.stageManager.contextLabel.text = context;
            Core.Instance.stageManager.contextLabel.gameObject.SetActive(true);
            Core.Instance.stageManager.contextGraffitiIcon.SetActive(true);
        }

        public void HideContext()
        {
            Core.Instance.stageManager.contextLabel.gameObject.SetActive(false);
            Core.Instance.stageManager.contextGraffitiIcon.SetActive(false);
        }
    }

    public enum RequirementLinkType
    {
        Trigger,
        ProgressObject
    }

    public enum RequirementNeeds
    {
        Graffiti,
        REP,
        Both
    }
}
