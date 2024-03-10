using UnityEngine;

namespace Archipelago.BRC.Components
{
    public class ResizeChatOnEnable : MonoBehaviour
    {
        public RectTransform parentTransform;
        public RectTransform targetTransform;

        private void OnEnable()
        {
            Vector2 delta = new Vector2();
            delta.x = targetTransform.rect.width * -0.75f;
            delta.y = parentTransform.sizeDelta.y;
            parentTransform.sizeDelta = delta;
            Vector3 pos = parentTransform.position;
            pos.x = targetTransform.position.x;
            parentTransform.position = pos;
        }
    }
}
