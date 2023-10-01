using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Archipelago.Components
{
    public class PlayerChecker : MonoBehaviour
    {
        public Player player;
        public Traverse playerT;
        public bool canGetItem => !playerT.Field<bool>("isDisabled").Value && !playerT.Field<bool>("inGraffitiGame").Value;
        public bool canGetNotif => canGetItem && !playerT.Field<bool>("phoneLocked").Value;

        public void Init()
        {
            player = GetComponent<Player>();
            playerT = Traverse.Create(player);
        }

        public void Update()
        {
            if (player == null) return;
            if (canGetItem)
            {
                if (Core.Instance.LocationManager.itemQueue.Count > 0)
                {
                    Core.Instance.LocationManager.GetItem(Core.Instance.LocationManager.itemQueue[0], false);
                    Core.Instance.LocationManager.itemQueue.RemoveAt(0);
                }
            }
            if (canGetNotif)
            {
                if (Core.Instance.LocationManager.notifQueue.Count > 0 && !playerT.Field<bool>("phoneLocked").Value)
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
    }
}
