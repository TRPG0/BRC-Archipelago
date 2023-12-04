using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.Structures;
using Newtonsoft.Json.Linq;
using Archipelago.Components;
using UnityEngine;
using Reptile;
using System.Linq;
using Random = UnityEngine.Random;

namespace Archipelago
{
    public class Multiworld
    {
        public static int[] AP_VERSION = new int[] { 0, 4, 3 };

        public bool Authenticated;
        public ArchipelagoSession Session;

        public static List<string> messages = new List<string>()
        {
            "> Test message one",
            "> Test msg 2",
            "> A third test message"
        };

        public DeathLinkService DeathLinkService = null;
        public bool DeathLinkKilling = false;
        public string DeathLinkReason { get; private set; }

        public bool Connect(int slotId, string name, string address, string password = null)
        {
            if (Authenticated) return true;
            if (slotId == -1) return false;
            if (Core.forbiddenModsLoaded > 0)
            {
                if (Core.forbiddenModsLoaded == 1) Core.Instance.UIManager.SetResult($"A forbidden mod is loaded. ({Core.forbiddenGUIDs})");
                else Core.Instance.UIManager.SetResult($"A forbidden mod is loaded. ({Core.forbiddenModsLoaded} mods)");

                Core.Logger.LogWarning($"A forbidden mod is loaded. ({Core.forbiddenGUIDs})");
                Core.Instance.UIManager.slotButtons[slotId].ChangeState(APSlotButton.SlotState.Disconnected);
                Core.Instance.UIManager.SetStatus(APSlotButton.SlotState.Disconnected);
                return false;
            }
            Core.Logger.LogInfo($"Attempting connection - Slot: {slotId} | Name: {name}");

            Session = ArchipelagoSessionFactory.CreateSession(address);
            Session.Socket.SocketClosed += SocketClosed;
            Session.Socket.ErrorReceived += ErrorReceived;
            Session.Socket.PacketReceived += PacketReceived;
            Session.Items.ItemReceived += ItemReceived;

            LoginResult loginResult = Session.TryConnectAndLogin(
                "Bomb Rush Cyberfunk",
                name,
                ItemsHandlingFlags.IncludeStartingInventory,
                new Version(AP_VERSION[0], AP_VERSION[1], AP_VERSION[2]),
                null,
                null,
                password == "" ? null : password,
                true
                );

            if (loginResult is LoginSuccessful success)
            {
                Core.Instance.Data.skipIntro = bool.Parse(success.SlotData["skip_intro"].ToString());
                Core.Instance.Data.skipDreams = bool.Parse(success.SlotData["skip_dreams"].ToString());
                Core.Instance.Data.totalRep = (TotalRep)int.Parse(success.SlotData["total_rep"].ToString());
                Core.Instance.Data.startingMovestyle = (MoveStyle)int.Parse(success.SlotData["starting_movestyle"].ToString());
                Core.Instance.Data.limitedGraffiti = bool.Parse(success.SlotData["limited_graffiti"].ToString());

                if (!Core.Instance.SaveManager.DataExists(slotId))
                {
                    if (Core.Instance.Data.startingMovestyle == MoveStyle.SKATEBOARD)
                    {
                        Core.Instance.Data.skateboardUnlocked = true;
                        Core.Instance.Data.inlineUnlocked = false;
                        Core.Instance.Data.bmxUnlocked = false;
                    }
                    else if (Core.Instance.Data.startingMovestyle == MoveStyle.INLINE)
                    {
                        Core.Instance.Data.skateboardUnlocked = false;
                        Core.Instance.Data.inlineUnlocked = true;
                        Core.Instance.Data.bmxUnlocked = false;
                    }
                    else if (Core.Instance.Data.startingMovestyle == MoveStyle.BMX)
                    {
                        Core.Instance.Data.skateboardUnlocked = false;
                        Core.Instance.Data.inlineUnlocked = false;
                        Core.Instance.Data.bmxUnlocked = true;
                    }

                    if (Core.Instance.Data.limitedGraffiti) Core.Instance.Data.grafUses["metalHead"] = 0;

                    Core.Instance.Data.damageMultiplier = int.Parse(success.SlotData["damage_multiplier"].ToString());
                    Core.Instance.Data.scoreDifficulty = (ScoreDifficulty)int.Parse(success.SlotData["score_difficulty"].ToString());
                    Core.Instance.Data.deathLink = bool.Parse(success.SlotData["death_link"].ToString());
                }

                Core.Logger.LogInfo($"Skip intro is {Core.Instance.Data.skipIntro}");
                Core.Logger.LogInfo($"Skip dreams is {Core.Instance.Data.skipDreams}");
                Core.Logger.LogInfo($"Total REP is {Core.Instance.Data.totalRep}");
                Core.Logger.LogInfo($"Starting movestyle is {Core.Instance.Data.startingMovestyle}");
                Core.Logger.LogInfo($"Limited graffiti is {Core.Instance.Data.limitedGraffiti}");
                Core.Logger.LogInfo($"Score difficulty is {Core.Instance.Data.scoreDifficulty}");
                Core.Logger.LogInfo($"Damage multiplier is {Core.Instance.Data.damageMultiplier}");
                Core.Logger.LogInfo($"Death link is {Core.Instance.Data.deathLink}");


                foreach (RawLocationData data in ((JArray)success.SlotData["locations"]).ToObject<RawLocationData[]>())
                {
                    AItem item = null;
                    if (data.brcitem)
                    {
                        item = new BRCItem()
                        {
                            item_name = data.item_name,
                            player_name = data.player_name,
                            type = (BRCType)int.Parse(data.item_type)
                        };
                    }
                    else
                    {
                        item = new APItem()
                        {
                            item_name = data.item_name,
                            player_name = data.player_name,
                            type = (ItemFlags)int.Parse(data.item_type)
                        };
                    }

                    Core.Instance.LocationManager.locations.Add(data.id, new Location()
                    {
                        ap_id = data.ap_id,
                        item = item,
                        @checked = Core.Instance.Data.@checked.Contains(data.id)
                    });
                }

                Authenticated = true;
                if (Core.Instance.Data.deathLink) EnableDeathLink();
                messages.Clear();
                Core.Instance.Data.slot_name = name;
                Core.Instance.Data.host_name = address;
                Core.Instance.Data.password = password;
                Core.Instance.SaveManager.currentSlot = slotId;
                Core.Instance.SaveManager.SaveData();
                Core.Instance.UIManager.slotButtons[slotId].ChangeState(APSlotButton.SlotState.Connected);
                Core.Instance.UIManager.SetResult($"Successfully connected to server as player \"{name}\".");
                Core.Instance.UIManager.SetStatus(APSlotButton.SlotState.Connected);
                Core.Instance.UIManager.APMenuChat.gameObject.SetActive(true);
                Core.Logger.LogInfo($"Successfully connected to server as player \"{name}\".");
            }
            else if (loginResult is LoginFailure failure)
            {
                Authenticated = false;
                Core.Logger.LogError(string.Join("\n", failure.Errors));
                Core.Instance.UIManager.SetResult(string.Join("\n", failure.Errors));
                Session.Socket.DisconnectAsync();
                Session = null;
                DeathLinkService = null;
                Core.Instance.UIManager.slotButtons[slotId].ChangeState(APSlotButton.SlotState.Disconnected);
                Core.Instance.UIManager.SetStatus(APSlotButton.SlotState.Disconnected);
            }

            return loginResult.Successful;
        }

        public void Disconnect()
        {
            if (Session != null && Session.Socket != null) Session.Socket.DisconnectAsync();
            messages.Clear();
            Core.Instance.UIManager.APMenuChat.gameObject.SetActive(false);
            Session = null;
            DeathLinkService = null;
            Authenticated = false;
        }

        public void SocketClosed(string reason)
        {
            Core.Logger.LogError($"Lost connection to Archipelago server. {reason}");
            Disconnect();
        }

        public void ErrorReceived(Exception e, string message)
        {
            Core.Logger.LogError(message);
            if (e != null) Core.Logger.LogError(e.ToString());
        }

        public void PacketReceived(ArchipelagoPacketBase packet)
        {
            if (packet.PacketType == ArchipelagoPacketType.PrintJSON)
            {
                if (messages.Count >= PhoneManager.maxMessages) messages.RemoveAt(0);

                var p = packet as PrintJsonPacket;

                string text = "";
                string color = "<color=#FFFFFFFF>";

                foreach (var messagePart in p.Data)
                {
                    switch (messagePart.Type)
                    {
                        case JsonMessagePartType.PlayerId:
                            if (Session.Players.GetPlayerName(int.Parse(messagePart.Text)) == Core.Instance.Data.slot_name) color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorPlayerSelf.Value)}>";
                            else color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorPlayerOther.Value)}>";
                            if (int.TryParse(messagePart.Text, out int playerSlot))
                            {
                                string playerName = Session.Players.GetPlayerAlias(playerSlot) ?? $"Slot: {playerSlot}";
                                text += color + playerName + "</color>";
                            }
                            else text += $"{color}{messagePart.Text}</color>";
                            break;
                        case JsonMessagePartType.ItemId:
                            switch (messagePart.Flags)
                            {
                                case ItemFlags.Advancement:
                                    color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorItemAdvancement.Value)}>";
                                    break;
                                case ItemFlags.NeverExclude:
                                    color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorItemNeverExclude.Value)}>";
                                    break;
                                case ItemFlags.Trap:
                                    color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorItemTrap.Value)}>";
                                    break;
                                default:
                                    color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorItemFiller.Value)}>";
                                    break;
                            }
                            if (int.TryParse(messagePart.Text, out int itemId))
                            {
                                string itemName = Session.Items.GetItemName(itemId) ?? $"Item: {itemId}";
                                text += color + itemName + "</color>";
                            }
                            else text += $"{color}{messagePart.Text}</color>";
                            break;
                        case JsonMessagePartType.LocationId:
                            color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorLocation.Value)}>";
                            if (int.TryParse(messagePart.Text, out int locationId))
                            {
                                string locationName = Session.Locations.GetLocationNameFromId(locationId) ?? $"Location: {locationId}";
                                text += color + locationName + "</color>";
                            }
                            else text += $"{color}{messagePart.Text}</color>";
                            break;
                        default:
                            text += messagePart.Text;
                            break;
                    }
                }

                messages.Add(text);
                Core.Instance.UIManager.APMenuChat.text = string.Join("\n", messages.ToArray());
                if (Core.Instance.PhoneManager.appArchipelago != null) Core.Instance.PhoneManager.appArchipelago.UpdateText();
            }
        }

        public void ItemReceived(ReceivedItemsHelper helper)
        {
            if (helper.Index > Core.Instance.Data.index)
            {
                string player = (Session.Players.GetPlayerAlias(helper.PeekItem().Player) == "") ? "?" : Session.Players.GetPlayerAlias(helper.PeekItem().Player);
                Core.Logger.LogInfo($"Received item: {helper.PeekItemName()} | Type: {Core.Instance.LocationManager.GetItemType(helper.PeekItemName())} | Player: {player}");

                BRCItem item = new BRCItem()
                {
                    item_name = helper.PeekItemName(),
                    type = Core.Instance.LocationManager.GetItemType(helper.PeekItemName()),
                    player_name = Core.Instance.Data.slot_name
                };


                Core.Instance.LocationManager.itemQueue.Add(item);

                Core.Instance.Data.index++;
            }
            helper.DequeueItem();
        }

        public void GetRandomHint()
        {
            if (!Authenticated) return;

            var missing = Session.Locations.AllMissingLocations;
            var alreadyHinted = Session.DataStorage.GetHints()
                .Where(h => h.FindingPlayer == Session.ConnectionInfo.Slot)
                .Select(h => h.LocationId);
            var available = missing.Except(alreadyHinted).ToArray();

            if (available.Any())
            {
                var locationId = available[Random.Range(0, available.Length)];

                Session.Locations.ScoutLocationsAsync(true, locationId);
                Core.Instance.LocationManager.notifQueue.Add(new Notification("AppArchipelago", "Hint unlocked!", null));
            }
            else Core.Logger.LogWarning("No locations available to hint.");
        }

        public void EnableDeathLink()
        {
            if (Session == null) return;
            if (DeathLinkService == null)
            {
                DeathLinkService = Session.CreateDeathLinkService();
                DeathLinkService.OnDeathLinkReceived += DeathLinkReceived;
            }
            DeathLinkService.EnableDeathLink();
        }

        public void DisableDeathLink()
        {
            if (DeathLinkService != null) DeathLinkService.DisableDeathLink();
        }

        public void DeathLinkReceived(DeathLink dl)
        {
            if (Reptile.Core.Instance.BaseModule.IsPlayingInStage && !Reptile.Core.Instance.BaseModule.IsLoading && !DeathLinkKilling)
            {
                DeathLinkKilling = true;
                string reason = dl.Cause;
                if (reason == null) reason = $"{dl.Source} has died.";
                DeathLinkReason = reason;
            }
            else Core.Logger.LogWarning("Received DeathLink, but player cannot be killed right now.");
        }

        public void SendCompletion()
        {
            if (!Authenticated) return;
            var packet = new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal };
            Session.Socket.SendPacket(packet);
        }
    }
}
