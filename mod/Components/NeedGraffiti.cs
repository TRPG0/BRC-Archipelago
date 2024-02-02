using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.Components
{
    public class NeedGraffiti : MonoBehaviour
    {
        public GameObject Trigger { get; private set; }
        public List<GraffitiSize> Sizes { get; private set; }

        public NeedGraffitiType Type { get; private set; }

        public enum NeedGraffitiType
        {
            Trigger,
            ProgressObject
        }

        public Collider collider;

        public void Init(List<GraffitiSize> sizes, GameObject originalTrigger, NeedGraffitiType type = NeedGraffitiType.Trigger)
        {
            Sizes = sizes;
            Trigger = originalTrigger;
            collider = GetComponent<Collider>();
            collider.tag = Tags.Untagged;
            gameObject.name = "NeedGraffitiTrigger";
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

        public void EnableTrigger()
        {
            if (Type == NeedGraffitiType.Trigger) Trigger.SetActive(true);
            else if (Type == NeedGraffitiType.ProgressObject) Trigger.GetComponent<ProgressObject>().SetTriggerable(true);
            gameObject.SetActive(false);
        }

        public void DisableTrigger()
        {
            if (Type == NeedGraffitiType.Trigger) Trigger.SetActive(false);
            else if (Type == NeedGraffitiType.ProgressObject) Trigger.GetComponent<ProgressObject>().SetTriggerable(false);
            gameObject.SetActive(true);
        }

        public void ShowContext()
        {
            Core.Instance.stageManager.contextLabel.text = string.Format(Core.Instance.Localizer.GetRawTextValue("REQUIREMENT_GRAFFITI"), SizesToString());
            Core.Instance.stageManager.contextLabel.gameObject.SetActive(true);
            Core.Instance.stageManager.contextGraffitiIcon.SetActive(true);
        }

        public void HideContext()
        {
            Core.Instance.stageManager.contextLabel.gameObject.SetActive(false);
            Core.Instance.stageManager.contextGraffitiIcon.SetActive(false);
        }
    }
}
