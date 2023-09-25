using Archipelago;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Reptile.Phone
{
    public class AppArchipelago : App
    {
        public List<string> Messages
        {
            get { return Multiworld.messages; }
        }

        public TextMeshProUGUI text;

        public void UpdateText()
        {
            text.text = string.Join("\n", Messages.ToArray());
        }

        public override void OnAppRefresh()
        {
            base.OnAppRefresh();
            UpdateText();
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            HandleInput = true;
            UpdateText();
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.94f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.89f);
        }

        public override void OnAppDisable()
        {
            base.OnAppDisable();
            HandleInput = false;
        }

        public override void OnReleaseLeft()
        {
            MyPhone.CloseCurrentApp();
        }
    }
}
