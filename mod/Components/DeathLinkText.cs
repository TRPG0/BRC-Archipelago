using TMPro;
using UnityEngine;

namespace Archipelago.BRC.Components
{
    public class DeathLinkText : MonoBehaviour
    {
        private TextMeshProUGUI parentText;
        private TextMeshProUGUI targetText;

        internal void Init(TextMeshProUGUI target)
        {
            parentText = GetComponent<TextMeshProUGUI>();
            targetText = target;
        }

        public void Update()
        {
            parentText.color = targetText.color;
        }

        public void OnEnable()
        {
            parentText.text = Core.Instance.Multiworld.DeathLinkReason;
        }
    }
}
