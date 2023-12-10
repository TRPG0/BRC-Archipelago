using Reptile;
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

        public RandoLocalizer(SystemLanguage localizationLanguage, SystemLanguage defaultLanguage, PlatformLanguages platformLanguages, LocalizationData localizationData)
        {
            this.defaultLanguage = defaultLanguage;
            this.platformLanguages = platformLanguages;
            this.localizationData = localizationData;
            if (IsLanguageAvailable(localizationLanguage)) UpdateLocalization(localizationLanguage);
            else UpdateLocalization(defaultLanguage);
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
    }
}
