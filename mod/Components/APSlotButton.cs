using Reptile;
using UnityEngine;
using UnityEngine.UI;

namespace Archipelago.BRC.Components
{
    public class APSlotButton : MonoBehaviour
    {
        public int slot = 0;

        public Image image = null;
        public SelectEnlargeButton button = null;
        public Image deleteImage = null;

        public SlotState CurrentState
        {
            get { return currentState; }
        }

        private SlotState currentState = SlotState.Vanilla;

        public enum SlotState
        {
            Vanilla,
            NoData,
            YesData,
            Connected,
            Disconnected
        }

        public void Init()
        {
            if (gameObject.name.Contains("One")) slot = 0;
            else if (gameObject.name.Contains("Two")) slot = 1;
            else if (gameObject.name.Contains("Three")) slot = 2;

            if (slot < 0 || slot > 2)
            {
                Core.Logger.LogError("APSlotButton has no corresponding slot.");
                DestroyImmediate(this);
                return;
            }

            if (image == null) FindComponents();
            SetImages();
        }

        private void FindComponents()
        {
            image = GetComponent<Image>();

            button = GetComponent<SelectEnlargeButton>();
            button.onClick.RemoveAllListeners();
            button.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            button.onClick.AddListener(delegate { Core.Instance.UIManager.ShowMenu(this); });

            string number = string.Empty;
            if (slot == 0) number = "One";
            else if (slot == 1) number = "Two";
            else if (slot == 2) number = "Three";

            deleteImage = transform.parent.Find($"DeleteSlot{number}Button").GetComponent<Image>();
        }

        public void SetImages()
        {
            SlotState state = currentState;
            if (state == SlotState.Vanilla) state = SlotState.Disconnected;

            image.sprite = UIManager.bundle.LoadAsset<Sprite>($"assets/state_{state.ToString().ToLower()}1.png");
            SpriteState spriteState = new SpriteState
            {
                highlightedSprite = UIManager.bundle.LoadAsset<Sprite>($"assets/state_{state.ToString().ToLower()}2.png"),
                selectedSprite = UIManager.bundle.LoadAsset<Sprite>($"assets/state_{state.ToString().ToLower()}2.png")
            };
            button.spriteState = spriteState;
        }

        public void ChangeState(SlotState state)
        {
            if (state == currentState) return;
            currentState = state;
            SetImages();
        }

        private void Update()
        {
            image.color = deleteImage.color;
        }
    }
}
