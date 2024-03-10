using TMPro;
using UnityEngine;

namespace Archipelago.BRC.Components
{
    public class OutlineSetter : MonoBehaviour
    {
        public TextMeshProUGUI text;

        public void SetText(TextMeshProUGUI text)
        {
            this.text = text;
        }

        public void Start()
        {
            text.outlineWidth = 0.15f;
        }
    }
}
