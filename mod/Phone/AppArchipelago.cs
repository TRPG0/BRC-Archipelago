using Archipelago;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Archipelago.Structures;

namespace Reptile.Phone
{
    public class AppArchipelago : App
    {
        public enum AppArchipelagoState
        {
            Chat,
            Options
        }

        public enum AppArchipelagoOptions
        {
            SkipIntro,
            SkipDreams,
            TotalREP,
            StartingMovestyle,
            LimitedGraffiti,
            ScoreDifficulty,
            DamageMultiplier,
            DeathLink
        }

        public List<string> Messages
        {
            get { return Multiworld.messages; }
        }

        public AppArchipelagoState State => m_State;
        private AppArchipelagoState m_State = AppArchipelagoState.Chat;

        public AppArchipelagoOptions CurrentOption => m_CurrentOption;
        private static AppArchipelagoOptions m_CurrentOption = AppArchipelagoOptions.SkipIntro;

        public TextMeshProUGUI title;
        public TextMeshProUGUI text;
        public TextMeshProUGUI bottomLeftText;
        public UIButtonGlyphComponent bottomLeftGlyph;
        public TextMeshProUGUI bottomRightText;
        public UIButtonGlyphComponent bottomRightGlyph;
        public TextMeshProUGUI optionText;
        public UIButtonGlyphComponent upGlyph;
        public UIButtonGlyphComponent downGlyph;

        public static Color fadeColor = new Color(0.192f, 0.35f, 0.647f);
        public static Color highlightColor = new Color(0.882f, 0.953f, 0.345f);

        public void UpdateText()
        {
            text.text = string.Join("\n", Messages.ToArray());
        }

        public void UpdateOptionText()
        {
            AppArchipelagoOptions prev1Option = m_CurrentOption - 1;
            AppArchipelagoOptions prev2Option = m_CurrentOption - 2;
            AppArchipelagoOptions next1Option = m_CurrentOption + 1;
            AppArchipelagoOptions next2Option = m_CurrentOption + 2;

            if ((int)prev1Option < 0) prev1Option = (AppArchipelagoOptions)(8 + (int)prev1Option);
            if ((int)prev2Option < 0) prev2Option = (AppArchipelagoOptions)(8 + (int)prev2Option);
            if ((int)next1Option > 7) next1Option = (AppArchipelagoOptions)((int)next1Option - 8);
            if ((int)next2Option > 7) next2Option = (AppArchipelagoOptions)((int)next2Option - 8);

            optionText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(prev2Option)}: {GetOptionValueText(prev2Option)}</color>\n"
                + $"<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(prev1Option)}: {GetOptionValueText(prev1Option)}</color>\n"
                + $"{GetOptionNameText(m_CurrentOption)}: <color=#{ColorUtility.ToHtmlStringRGBA(highlightColor)}>{GetOptionValueText(m_CurrentOption)}</color>"
                + $"\n<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(next1Option)}: {GetOptionValueText(next1Option)}</color>"
                + $"\n<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(next2Option)}: {GetOptionValueText(next2Option)}</color>";
        }

        private string GetOptionNameText(AppArchipelagoOptions option)
        {
            return option switch
            {
                AppArchipelagoOptions.SkipIntro => "Skip Intro",
                AppArchipelagoOptions.SkipDreams => "Skip Dreams",
                AppArchipelagoOptions.TotalREP => "Total REP",
                AppArchipelagoOptions.StartingMovestyle => "Starting Movestyle",
                AppArchipelagoOptions.LimitedGraffiti => "Limited Graffiti",
                AppArchipelagoOptions.ScoreDifficulty => "Score Difficulty",
                AppArchipelagoOptions.DamageMultiplier => "Damage Multiplier",
                AppArchipelagoOptions.DeathLink => "Death Link",
                _ => "?"
            };
        }

        private string GetOptionValueText(AppArchipelagoOptions option)
        {
            return option switch
            {
                AppArchipelagoOptions.SkipIntro => ValueToText(Archipelago.Core.Instance.Data.skipIntro),
                AppArchipelagoOptions.SkipDreams => ValueToText(Archipelago.Core.Instance.Data.skipDreams),
                AppArchipelagoOptions.TotalREP => ValueToText(Archipelago.Core.Instance.Data.totalRep),
                AppArchipelagoOptions.StartingMovestyle => ValueToText(Archipelago.Core.Instance.Data.startingMovestyle),
                AppArchipelagoOptions.LimitedGraffiti => ValueToText(Archipelago.Core.Instance.Data.limitedGraffiti),
                AppArchipelagoOptions.ScoreDifficulty => ValueToText(Archipelago.Core.Instance.Data.scoreDifficulty),
                AppArchipelagoOptions.DamageMultiplier => ValueToText(Archipelago.Core.Instance.Data.damageMultiplier),
                AppArchipelagoOptions.DeathLink => ValueToText(Archipelago.Core.Instance.Data.death_link),
                _ => "?"
            };
        }

        private string ValueToText(object value)
        {
            if (value is bool boolValue)
            {
                if (boolValue) return "Yes";
                else return "No";
            }
            else if (value is TotalRep repValue)
            {
                if (repValue == TotalRep.MuchLess) return "Much Less";
                else return repValue.ToString();
            }
            else if (value is MoveStyle styleValue)
            {
                if (styleValue == MoveStyle.SKATEBOARD) return "Skateboard";
                else if (styleValue == MoveStyle.INLINE) return "Inline Skates";
                else return styleValue.ToString();
            }
            else if (value is ScoreDifficulty scoreValue)
            {
                if (scoreValue == ScoreDifficulty.VeryHard) return "Very Hard";
                else return scoreValue.ToString();
            }
            else if (value is int) return value.ToString();
            else return "?";
        }

        private void UpdateGlyphs()
        {
            if (m_State == AppArchipelagoState.Chat)
            {
                bottomLeftText.text = "Close app";
                bottomRightText.text = "Options";
                bottomRightText.gameObject.SetActive(true);
                bottomRightGlyph.gameObject.SetActive(true);
            }
            else if (m_State == AppArchipelagoState.Options)
            {
                bottomLeftText.text = "Back";
                if (m_CurrentOption == AppArchipelagoOptions.ScoreDifficulty)
                {
                    bottomRightText.text = "Change";
                    bottomRightText.gameObject.SetActive(true);
                    bottomRightGlyph.gameObject.SetActive(true);
                }
                else
                {
                    bottomRightText.gameObject.SetActive(false);
                    bottomRightGlyph.gameObject.SetActive(false);
                }
            }
        }

        public void ChangeState(AppArchipelagoState state)
        {
            if (state == AppArchipelagoState.Chat)
            {
                m_State = AppArchipelagoState.Chat;
                title.text = "Archipelago";
                UpdateGlyphs();
                text.gameObject.SetActive(true);
                optionText.gameObject.SetActive(false);
                upGlyph.gameObject.SetActive(false);
                downGlyph.gameObject.SetActive(false);
            }
            else if (state == AppArchipelagoState.Options)
            {
                m_State = AppArchipelagoState.Options;
                title.text = "Options";
                UpdateGlyphs();
                UpdateOptionText();
                text.gameObject.SetActive(false);
                optionText.gameObject.SetActive(true);
                upGlyph.gameObject.SetActive(true);
                downGlyph.gameObject.SetActive(true);
            }
        }

        public void ResizeDeltas()
        {
            Vector2 mainText = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.94f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.89f);
            Vector2 glyphs = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.94f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.88f);
            Vector2 inputTexts = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.67f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.88f);
            text.GetComponent<RectTransform>().sizeDelta = mainText;
            optionText.GetComponent<RectTransform>().sizeDelta = mainText;
            bottomLeftGlyph.GetComponent<RectTransform>().sizeDelta = glyphs;
            bottomRightGlyph.GetComponent<RectTransform>().sizeDelta = glyphs;
            bottomLeftText.GetComponent<RectTransform>().sizeDelta = inputTexts;
            bottomRightText.GetComponent<RectTransform>().sizeDelta = inputTexts;
        }

        public void Start()
        {
            UpdateGlyphs();
            ResizeDeltas();
            Invoke("ResizeDeltas", 0.3f);
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
        }

        public override void OnAppDisable()
        {
            base.OnAppDisable();
            HandleInput = false;
        }

        public override void OnReleaseLeft()
        {
            if (m_State == AppArchipelagoState.Chat) MyPhone.CloseCurrentApp();
            else
            {
                Archipelago.Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                ChangeState(AppArchipelagoState.Chat);
            }
        }

        public override void OnReleaseRight()
        {
            if (m_State == AppArchipelagoState.Chat)
            {
                Archipelago.Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                ChangeState(AppArchipelagoState.Options);
            }
            else if (m_State == AppArchipelagoState.Options)
            {
                if (m_CurrentOption == AppArchipelagoOptions.ScoreDifficulty)
                {
                    Archipelago.Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Archipelago.Core.Instance.Data.scoreDifficulty += 1;
                    if (Archipelago.Core.Instance.Data.scoreDifficulty > ScoreDifficulty.Extreme) Archipelago.Core.Instance.Data.scoreDifficulty = ScoreDifficulty.Normal;
                    Archipelago.Core.Instance.stageManager.SetEncounterScores(WorldHandler.instance.StoryManager);
                    UpdateOptionText();
                }
            }
        }

        public override void OnReleaseUp()
        {
            if (m_State == AppArchipelagoState.Options)
            {
                Archipelago.Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                m_CurrentOption -= 1;
                if ((int)m_CurrentOption < 0) m_CurrentOption = AppArchipelagoOptions.DeathLink;

                UpdateOptionText();
                UpdateGlyphs();
            }
        }

        public override void OnReleaseDown()
        {
            if (m_State == AppArchipelagoState.Options)
            {
                Archipelago.Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                m_CurrentOption += 1;
                if ((int)m_CurrentOption > 7) m_CurrentOption = AppArchipelagoOptions.SkipIntro;

                UpdateOptionText();
                UpdateGlyphs();
            }
        }

        public override void OpenContent(AUnlockable unlockable, bool appAlreadyOpen)
        {
            if (m_State == AppArchipelagoState.Options) ChangeState(AppArchipelagoState.Chat);
            base.OpenContent(unlockable, appAlreadyOpen);
        }
    }
}
