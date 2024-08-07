﻿using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.BRC.Structures;
using Newtonsoft.Json.Linq;
using Archipelago.BRC.Components;
using UnityEngine;
using Reptile;
using System.Linq;
using Random = UnityEngine.Random;
using ModLocalizer;
using Archipelago.MultiClient.Net.Models;

namespace Archipelago.BRC
{
    public class Multiworld
    {
        public static int[] AP_VERSION = new int[] { 0, 5, 0 };

        public bool Authenticated;
        public ArchipelagoSession Session;

        public static List<string> messages = new List<string>();

        public DeathLinkService DeathLinkService = null;
        public bool DeathLinkKilling = false;
        public string DeathLinkReason { get; private set; }

        public void TryGetSlotDataValue(ref int option, Dictionary<string, object> slotData, string key, int defaultValue)
        {
            try { option = int.Parse(slotData[key].ToString()); }
            catch (KeyNotFoundException)
            {
                Core.Logger.LogWarning($"No key found for option \"{key}\". Using default value ({defaultValue})");
                option = defaultValue;
            }
        }

        public void TryGetSlotDataValue(ref bool option, Dictionary<string, object> slotData, string key, bool defaultValue)
        {
            try { option = bool.Parse(slotData[key].ToString()); }
            catch (KeyNotFoundException)
            {
                Core.Logger.LogWarning($"No key found for option \"{key}\". Using default value ({defaultValue})");
                option = defaultValue;
            }
        }

        public void TryGetSlotDataValue(ref Logic option, Dictionary<string, object> slotData, string key, Logic defaultValue)
        {
            try { option = (Logic)int.Parse(slotData[key].ToString()); }
            catch (KeyNotFoundException)
            {
                Core.Logger.LogWarning($"No key found for option \"{key}\". Using default value ({defaultValue})");
                option = defaultValue;
            }
        }

        public void TryGetSlotDataValue(ref MoveStyle option, Dictionary<string, object> slotData, string key, MoveStyle defaultValue)
        {
            try { option = (MoveStyle)int.Parse(slotData[key].ToString()); }
            catch (KeyNotFoundException)
            {
                Core.Logger.LogWarning($"No key found for option \"{key}\". Using default value ({defaultValue})");
                option = defaultValue;
            }
        }

        public void TryGetSlotDataValue(ref SGraffiti option, Dictionary<string, object> slotData, string key, SGraffiti defaultValue)
        {
            try { option = (SGraffiti)int.Parse(slotData[key].ToString()); }
            catch (KeyNotFoundException)
            {
                Core.Logger.LogWarning($"No key found for option \"{key}\". Using default value ({defaultValue})");
                option = defaultValue;
            }
        }

        public void TryGetSlotDataValue(ref ScoreDifficulty option, Dictionary<string, object> slotData, string key, ScoreDifficulty defaultValue)
        {
            try { option = (ScoreDifficulty)int.Parse(slotData[key].ToString()); }
            catch (KeyNotFoundException)
            {
                Core.Logger.LogWarning($"No key found for option \"{key}\". Using default value ({defaultValue})");
                option = defaultValue;
            }
        }

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
                ItemsHandlingFlags.AllItems,
                new Version(AP_VERSION[0], AP_VERSION[1], AP_VERSION[2]),
                null,
                null,
                password == "" ? null : password,
                true
                );

            if (loginResult is LoginSuccessful success)
            {
                TryGetSlotDataValue(ref Core.Instance.Data.logic, success.SlotData, "logic", Logic.Glitchless);
                TryGetSlotDataValue(ref Core.Instance.Data.skipIntro, success.SlotData, "skip_intro", true);
                TryGetSlotDataValue(ref Core.Instance.Data.skipDreams, success.SlotData, "skip_dreams", false);
                TryGetSlotDataValue(ref Core.Instance.Data.skipHands, success.SlotData, "skip_statue_hands", false);
                TryGetSlotDataValue(ref Core.Instance.Data.totalRep, success.SlotData, "total_rep", 1400);
                TryGetSlotDataValue(ref Core.Instance.Data.endingRep, success.SlotData, "extra_rep_required", false);
                TryGetSlotDataValue(ref Core.Instance.Data.startingMovestyle, success.SlotData, "starting_movestyle", MoveStyle.SKATEBOARD);
                TryGetSlotDataValue(ref Core.Instance.Data.junkPhotos, success.SlotData, "skip_polo_photos", false);
                TryGetSlotDataValue(ref Core.Instance.Data.limitedGraffiti, success.SlotData, "limited_graffiti", false);
                TryGetSlotDataValue(ref Core.Instance.Data.sGraffiti, success.SlotData, "small_graffiti_uses", SGraffiti.Separate);

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

                    if (Core.Instance.Data.limitedGraffiti)
                    {
                        if (Core.Instance.Data.sGraffiti == SGraffiti.Separate) Core.Instance.Data.grafUses[Characters.metalHead.ToString()] = 0;
                        else Core.Instance.Data.grafUses["S"] = 0;
                    }

                    TryGetSlotDataValue(ref Core.Instance.Data.damageMultiplier, success.SlotData, "damage_multiplier", 1);
                    TryGetSlotDataValue(ref Core.Instance.Data.scoreDifficulty, success.SlotData, "score_difficulty", ScoreDifficulty.Normal);
                    // can't use properties in ref methods 
                    try { Core.configDontSavePhotos.Value = bool.Parse(success.SlotData["dont_save_photos"].ToString()); }
                    catch (KeyNotFoundException)
                    {
                        Core.Logger.LogWarning($"No key found for option \"dont_save_photos\". Ignored.");
                    }
                    TryGetSlotDataValue(ref Core.Instance.Data.deathLink, success.SlotData, "death_link", false);
                }

                Core.Instance.LocationManager.locations = ((JObject)success.SlotData["locations"]).ToObject<Dictionary<string, long>>();

                Authenticated = true;
                if (Core.Instance.Data.deathLink) EnableDeathLink();
                messages.Clear();
                Core.Instance.Data.slot_name = name;
                Core.Instance.Data.host_name = address;
                Core.Instance.Data.password = password;
                Core.Instance.Data.fakeRep = 0;
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

                if (Core.configPlayerOtherNotifs.Value
                    && p.Data[0].Type == JsonMessagePartType.PlayerId
                    && Session.Players.GetPlayerName(int.Parse(p.Data[0].Text)) == Core.Instance.Data.slot_name
                    && p.Data[1].Text == " sent ")
                {
                    string itemName = Session.Items.GetItemName(long.Parse(p.Data[2].Text), Session.Players.GetPlayerInfo(p.Data[2].Player.Value).Game);
                    string forPlayer = Session.Players.GetPlayerAlias(int.Parse(p.Data[4].Text));

                    Core.Instance.LocationManager.notifQueue.Add(new Notification("AppArchipelago", $"{itemName} ({forPlayer})", null));
                }

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
                            if (long.TryParse(messagePart.Text, out long itemId))
                            {
                                string itemName = Session.Items.GetItemName(itemId, Session.Players.GetPlayerInfo(messagePart.Player.Value).Game) ?? $"Item: {itemId}";
                                text += color + itemName + "</color>";
                            }
                            else text += $"{color}{messagePart.Text}</color>";
                            break;
                        case JsonMessagePartType.LocationId:
                            color = $"<color=#{ColorUtility.ToHtmlStringRGBA(Core.configColorLocation.Value)}>";
                            if (long.TryParse(messagePart.Text, out long locationId))
                            {
                                string locationName = Session.Locations.GetLocationNameFromId(locationId, Session.Players.GetPlayerInfo(messagePart.Player.Value).Game) ?? $"Location: {locationId}";
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
            ItemInfo item = helper.PeekItem();
            if (helper.Index > Core.Instance.Data.index)
            {
                string player = (Session.Players.GetPlayerAlias(item.Player) == "") ? "?" : Session.Players.GetPlayerAlias(item.Player);
                string log = $"Received item: {item.ItemName} | Type: {Core.Instance.LocationManager.GetItemType(item.ItemName)}";
                if (player != Core.Instance.Data.slot_name) log += $" | Player: {player}";
                Core.Logger.LogInfo(log);
            }

            BRCItem brcitem = new BRCItem()
            {
                item_name = item.ItemName,
                type = Core.Instance.LocationManager.GetItemType(item.ItemName),
                player_name = Core.Instance.Data.slot_name,
                received = helper.Index <= Core.Instance.Data.index
            };

            Core.Instance.LocationManager.itemQueue.Add(brcitem);

            if (helper.Index > Core.Instance.Data.index) Core.Instance.Data.index++;
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
                Core.Instance.LocationManager.notifQueue.Add(new Notification("AppArchipelago", Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_HINT"), null));
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
