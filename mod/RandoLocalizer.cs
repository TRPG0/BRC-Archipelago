using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Archipelago
{
    public class RandoLocalizer
    {
        private SystemLanguage localizationLanguage;
        private SystemLanguage defaultLanguage;
        private PlatformLanguages platformLanguages;
        private LocalizationLookupTable localizationLookupTable;
        private LocalizationData localizationData;

        public GameFontType MainFont { get; private set; }
        public GameFontType EnglishOnlyFont { get; private set; }

        public RandoLocalizer(SystemLanguage localizationLanguage, SystemLanguage defaultLanguage, PlatformLanguages platformLanguages, LocalizationData localizationData)
        {
            this.defaultLanguage = defaultLanguage;
            this.platformLanguages = platformLanguages;
            this.localizationData = localizationData;
            if (IsLanguageAvailable(localizationLanguage)) UpdateLocalization(localizationLanguage);
            else UpdateLocalization(defaultLanguage);

            MainFont = ScriptableObject.CreateInstance<GameFontType>();
            EnglishOnlyFont = ScriptableObject.CreateInstance<GameFontType>();
            GameFontType defaultFont = LoadFontType("DefaultText");
            Traverse mainT = Traverse.Create(MainFont);
            Traverse engT = Traverse.Create(EnglishOnlyFont);
            Traverse defaultT = Traverse.Create(defaultFont);
            mainT.Field<TextMeshProFont>("defaultFont").Value = defaultT.Field<TextMeshProFont>("defaultFont").Value;
            mainT.Field<TextMeshProFont[]>("fonts").Value = defaultT.Field<TextMeshProFont[]>("fonts").Value;
            MainFont = RemoveUnusedFonts(MainFont);
            engT.Field<TextMeshProFont>("defaultFont").Value = defaultT.Field<TextMeshProFont>("defaultFont").Value;
            engT.Field<TextMeshProFont[]>("fonts").Value = defaultT.Field<TextMeshProFont[]>("fonts").Value;
            EnglishOnlyFont = RemoveFontsExceptEnglish(EnglishOnlyFont);
        }

        private LocalizationLookupTable GenerateLocalizationTable(SystemLanguage language)
        {
            string filePath = Path.Combine(Core.AssemblyPath, "Languages", language.ToString() + ".fods");
            //Core.Logger.LogInfo(filePath);
            LocalizationTableGenerator localizationTableGenerator = new LocalizationTableGenerator();
            LocalizationLookupTable result = localizationTableGenerator.GetLocalizationLookupTable(filePath, localizationData, language);
            return result;
        }

        private LocalizationLookupTable HandleNoLocalizationFoundForLanguage(SystemLanguage systemLanguage)
        {
            return CreateDefaultLocalizationTable();
        }

        private LocalizationLookupTable CreateDefaultLocalizationTable()
        {
            return GenerateLocalizationTable(defaultLanguage);
        }

        private bool IsLanguageAvailable(SystemLanguage systemLanguage)
        {
            bool result = false;
            for (int i = 0; i < platformLanguages.availableLanguages.Length; i++)
            {
                if (platformLanguages.availableLanguages[i] == systemLanguage) result = true;
            }
            return result;
        }

        private void GenerateLocalization()
        {
            localizationLookupTable = GenerateLocalizationTable(localizationLanguage);
            if (localizationLookupTable == null) localizationLookupTable = HandleNoLocalizationFoundForLanguage(localizationLanguage);
        }

        private void DisposeCurrentLocalizationLookupTable()
        {
            if (localizationLookupTable != null) localizationLookupTable.Dispose();
            localizationLookupTable = null;
        }

        public void UpdateLocalization(SystemLanguage localizationLanguage)
        {
            if (this.localizationLanguage == localizationLanguage) return;

            DisposeCurrentLocalizationLookupTable();
            this.localizationLanguage = localizationLanguage;
            GenerateLocalization();
        }

        public string GetRawTextValue(string localizationKey)
        {
            return localizationLookupTable.GetLocalizationValueFromSubgroup("Text", localizationKey);
        }

        public GameFontType LoadFontType(string fontType)
        {
            return Reptile.Core.Instance.Assets.LoadAssetFromBundle<GameFontType>("coreassets", $"assets/gamemanagement/localization/scriptableobjects/fonts/gamefonttypes/{fontType}.asset");
        }

        private GameFontType RemoveUnusedFonts(GameFontType gameFontType)
        {
            Traverse traverse = Traverse.Create(gameFontType);
            TextMeshProFont[] oldArray = traverse.Field<TextMeshProFont[]>("fonts").Value;
            List<TextMeshProFont> newList = new List<TextMeshProFont>();

            for (int i = 0; i < oldArray.Length; i++)
            {
                if (IsLanguageAvailable(oldArray[i].Language)) newList.Add(oldArray[i]);
            }

            if (newList.Count < 1) throw new System.Exception("No valid fonts found for this GameFontType. This should never happen.");

            traverse.Field<TextMeshProFont[]>("fonts").Value = newList.ToArray();
            return gameFontType;
        }

        private GameFontType RemoveFontsExceptEnglish(GameFontType gameFontType)
        {
            Traverse traverse = Traverse.Create(gameFontType);
            TextMeshProFont[] oldArray = traverse.Field<TextMeshProFont[]>("fonts").Value;
            List<TextMeshProFont> newList = new List<TextMeshProFont>();

            for (int i = 0; i < oldArray.Length; i++)
            {
                if (oldArray[i].Language == SystemLanguage.English) newList.Add(oldArray[i]);
            }

            if (newList.Count < 1) throw new System.Exception("No valid fonts found for this GameFontType. This should never happen.");

            traverse.Field<TextMeshProFont[]>("fonts").Value = newList.ToArray();
            return gameFontType;
        }
    }
}
