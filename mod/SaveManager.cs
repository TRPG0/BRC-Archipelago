using HarmonyLib;
using Reptile;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Reptile.Phone;

namespace Archipelago
{
    public class SaveManager
    {
        public SaveSlotData CurrentSaveSlot
        {
            get { return Reptile.Core.Instance.SaveManager.CurrentSaveSlot; }
        }

        public string FolderPath
        {
            get { return m_FolderPath; }
        }

        private string m_FolderPath;

        public int currentSlot = -1;

        public void GetSavePath()
        {
            m_FolderPath = Traverse.Create(Reptile.Core.Instance.BaseModule).Field("saveManager").Field("storage").Field<string>("storageDirectory").Value;
        }

        public bool DataExists(int slot = -1)
        {
            int fileSlot = slot;
            if (fileSlot == -1) fileSlot = currentSlot;
            string filePath = Path.Combine(FolderPath, string.Format("archipelago{0}.json", fileSlot));
            return File.Exists(filePath);
        }

        public void SaveData(int slot = -1)
        {
            int fileSlot = slot;
            if (fileSlot == -1) fileSlot = currentSlot;
            string filePath = Path.Combine(FolderPath, string.Format("archipelago{0}.json", fileSlot));
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Core.Instance.Data));
            File.WriteAllBytes(filePath, bytes);
            //Core.Logger.LogInfo($"Saved to {filePath}");
        }

        public void LoadData(int slot = -1)
        {
            int fileSlot = slot;
            if (fileSlot == -1) fileSlot = currentSlot;
            string filePath = Path.Combine(FolderPath, string.Format("archipelago{0}.json", fileSlot));
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    Core.Instance.Data = JsonConvert.DeserializeObject<Data>(reader.ReadToEnd());
                }
                Core.Logger.LogInfo($"Loaded from {filePath}");
            }
            else
            {
                Core.Logger.LogWarning($"File at {filePath} doesn't exist.");
            }
        }

        public void DeleteData(int slot = -1)
        {
            int fileSlot = slot;
            if (fileSlot == -1) fileSlot = currentSlot;
            string filePath = Path.Combine(FolderPath, string.Format("archipelago{0}.json", fileSlot));
            if (File.Exists(filePath)) File.Delete(filePath);
            Core.Instance.Data = new Data();
            Core.Logger.LogInfo($"Deleted data at {filePath}");
        }

        // this only exists so I can patch the original method and use this one for items instead
        public void UnlockCharacter(Characters character)
        {
            CurrentSaveSlot.GetCharacterProgress(character).unlocked = true;
            Reptile.Core.Instance.SaveManager.SaveCurrentSaveSlot();
        }
    }
}
