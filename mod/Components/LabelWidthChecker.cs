using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Archipelago.Components
{
    public class LabelWidthChecker : MonoBehaviour
    {
        private TextMeshProUGUI nameLabel;
        private TextMeshProUGUI addressLabel;
        private TextMeshProUGUI passwordLabel;
        private TMP_InputField nameInput;
        private TMP_InputField addressInput;
        private TMP_InputField passwordInput;

        public void Init(TextMeshProUGUI nameLabel, TextMeshProUGUI addressLabel, TextMeshProUGUI passwordLabel, TMP_InputField nameInput, TMP_InputField addressInput, TMP_InputField passwordInput)
        {
            this.nameLabel = nameLabel;
            this.addressLabel = addressLabel;
            this.passwordLabel = passwordLabel;
            this.nameInput = nameInput;
            this.addressInput = addressInput;
            this.passwordInput = passwordInput;
        }

        public float GetLargestLabelWidth()
        {
            float[] array = new float[] { nameLabel.renderedWidth, addressLabel.renderedWidth, passwordLabel.renderedWidth };
            return array.Max();
        }

        public IEnumerator AdjustInputs()
        {
            yield return new WaitForEndOfFrame();
            float newX = addressLabel.transform.localPosition.x + GetLargestLabelWidth() + addressLabel.renderedHeight;

            Vector3 pos = nameInput.transform.localPosition;
            pos.x = newX;
            nameInput.transform.localPosition = pos;

            Vector3 pos2 = addressInput.transform.localPosition;
            pos2.x = newX;
            addressInput.transform.localPosition = pos2;

            Vector3 pos3 = passwordInput.transform.localPosition;
            pos3.x = newX;
            passwordInput.transform.localPosition = pos3;
        }

        public void OnEnable()
        {
            StartCoroutine(AdjustInputs());
        }
    }
}
