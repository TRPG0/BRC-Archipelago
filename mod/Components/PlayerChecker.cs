using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Archipelago.BRC.Components
{
    public class PlayerChecker : MonoBehaviour
    {
        private Player player;
        private Traverse playerT;
        private Traverse whT;
        public bool CanGetItem => !playerT.Field<bool>("isDisabled").Value && !playerT.Field<bool>("inGraffitiGame").Value && !player.IsDead();
        public bool CanGetNotif
        {
            get
            {
                if (whT.Field<Encounter>("currentEncounter").Value != null)
                {
                    return CanGetItem && whT.Field<Encounter>("currentEncounter").Value.allowPhone;
                }
                else return CanGetItem;
            }
        }

        public void Init()
        {
            player = GetComponent<Player>();
            playerT = Traverse.Create(player);
            whT = Traverse.Create(WorldHandler.instance);
        }

        public void Update()
        {
            if (player == null) return;
            if (Core.Instance.Multiworld.DeathLinkKilling)
            {
                if (CanGetItem) player.ChangeHP(6);
            }
            if (Core.Instance.LocationManager.itemQueue.Count > 0)
            {
                if (CanGetItem)
                {
                    Core.Instance.LocationManager.GetItem(Core.Instance.LocationManager.itemQueue[0], false);
                    Core.Instance.LocationManager.itemQueue.RemoveAt(0);
                }
            }
            if (Core.Instance.LocationManager.notifQueue.Count > 0)
            {
                if (CanGetNotif)
                {
                    string app = Core.Instance.LocationManager.notifQueue[0].app;
                    string message = Core.Instance.LocationManager.notifQueue[0].message;
                    AUnlockable unlockable = Core.Instance.LocationManager.notifQueue[0].unlockable;
                    if (!Core.Instance.PhoneManager.Phone.AppInstances.ContainsKey(app)) Core.Logger.LogWarning($"Tried to push notification to phone, but app \"{app}\" does not exist.");
                    else Core.Instance.PhoneManager.Phone.AppInstances[app].PushNotification(message, unlockable);
                    Core.Instance.LocationManager.notifQueue.RemoveAt(0);
                }
            }
        }

        public void OnDisable()
        {
            try
            {
                Core.Instance.stageManager.contextGraffitiIcon.SetActive(false);
                Core.Instance.stageManager.contextLabel.gameObject.SetActive(false);
            }
            catch { }
        }
    }
}
