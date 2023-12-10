using Archipelago.MultiClient.Net.Enums;
using Archipelago.Stages;
using Archipelago.Structures;
using HarmonyLib;
using Reptile;
using Reptile.Phone;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;
using Notification = Archipelago.Structures.Notification;

namespace Archipelago
{
    public class LocationManager
    {
        public Dictionary<string, Location> locations = new Dictionary<string, Location>();

        public List<AItem> itemQueue = new List<AItem>();
        public List<Notification> notifQueue = new List<Notification>();

        public void CheckLocation(string loc)
        {
            Core.Logger.LogInfo($"Checking location \"{loc}\"");
            if (!Core.Instance.Data.@checked.Contains(loc)) Core.Instance.Data.@checked.Add(loc);

            if (locations.ContainsKey(loc))
            {
                if (Core.Instance.Multiworld.Authenticated) Core.Instance.Multiworld.Session.Locations.CompleteLocationChecks(locations[loc].ap_id);
                if (!locations[loc].@checked)
                {
                    if (locations[loc].item is BRCItem && locations[loc].item.player_name == Core.Instance.Data.slot_name)
                    {
                        Core.Logger.LogInfo($"Item at location {loc}: {locations[loc].item.item_name} | Type: {((BRCItem)locations[loc].item).type}");
                    }
                    GetItem(locations[loc].item);
                }
                locations[loc].@checked = true;
            }
            else Core.Logger.LogWarning($"Location \"{loc}\" does not exist.");
        }

        public void GetItem(AItem item, bool playSound = true)
        {
            Core.Instance.PhoneManager.appArchipelago.UpdateText();
            if (playSound) Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.MenuSfx, AudioClipID.unlockNotification);
            if (item is BRCItem brcitem && item.player_name == Core.Instance.Data.slot_name)
            {
                string substring;
                switch (brcitem.type)
                {
                    case BRCType.Music:
                        substring = brcitem.item_name.Substring(7, brcitem.item_name.Length-8);
                        MusicTrack track = GetMusicTrack(GetMusicAssetName(substring));
                        Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(track.Uid).IsUnlocked = true;
                        Core.Instance.PhoneManager.Phone.GetAppInstance<AppMusicPlayer>().GameMusicPlayer.AddMusicTrack(track);
                        notifQueue.Add(new Notification("AppMusicPlayer", $"{Core.Instance.RandoLocalizer.GetRawTextValue("COLLECTIBLE_MUSIC")} ({track.Title})", track));
                        break;
                    case BRCType.GraffitiM:
                    case BRCType.GraffitiL:
                    case BRCType.GraffitiXL:
                        if (brcitem.type == BRCType.GraffitiXL) substring = brcitem.item_name.Substring(15, brcitem.item_name.Length-16);
                        else substring = brcitem.item_name.Substring(14, brcitem.item_name.Length - 15);

                        /*
                        if (brcitem.type == BRCType.GraffitiM) Core.Instance.stageManager.YesGraffitiM();
                        else if (brcitem.type == BRCType.GraffitiL) Core.Instance.stageManager.YesGraffitiL();
                        else if (brcitem.type == BRCType.GraffitiXL) Core.Instance.stageManager.YesGraffitiXL();
                        */
                        if (brcitem.type == BRCType.GraffitiM) Core.Instance.stageManager.YesGraffiti(GraffitiSize.M);
                        else if (brcitem.type == BRCType.GraffitiL) Core.Instance.stageManager.YesGraffiti(GraffitiSize.L);
                        else if (brcitem.type == BRCType.GraffitiXL) Core.Instance.stageManager.YesGraffiti(GraffitiSize.XL);

                        if (Core.Instance.Data.to_lock.Contains(substring)) Core.Instance.Data.to_lock.Remove(substring);

                        GraffitiAppEntry graffiti = WorldHandler.instance.graffitiArtInfo.FindByTitle(substring).unlockable;
                        Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(graffiti.Uid).IsUnlocked = true;

                        if (Core.Instance.Data.limitedGraffiti) Core.Instance.Data.grafUses[graffiti.Uid] = 0;
                        notifQueue.Add(new Notification("AppGraffiti", $"{Core.Instance.RandoLocalizer.GetRawTextValue("COLLECTIBLE_GRAFFITI")} ({graffiti.Size} - {graffiti.Title})", graffiti));
                        break;
                    case BRCType.Skateboard:
                        substring = brcitem.item_name.Substring(12, brcitem.item_name.Length - 13);
                        Core.Instance.Data.skateboardUnlocked = true;
                        if (Core.Instance.stageManager is HideoutManager) ((HideoutManager)Core.Instance.stageManager).SetSkateboardGarage(true);
                        MoveStyleSkin skateboard = GetSkateboardSkin(GetSkateboardAssetName(substring));
                        Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(skateboard.Uid).IsUnlocked = true;
                        notifQueue.Add(new Notification("AppArchipelago", $"{Core.Instance.RandoLocalizer.GetRawTextValue("VALUE_MOVESTYLE_SKATEBOARD")} {brcitem.item_name.Substring(11)}", null));
                        break;
                    case BRCType.InlineSkates:
                        substring = brcitem.item_name.Substring(15, brcitem.item_name.Length - 16);
                        Core.Instance.Data.inlineUnlocked = true;
                        if (Core.Instance.stageManager is HideoutManager) ((HideoutManager)Core.Instance.stageManager).SetInlineGarage(true);
                        MoveStyleSkin inlineskates = GetInlineSkin(GetInlineAssetName(substring));
                        Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(inlineskates.Uid).IsUnlocked = true;
                        notifQueue.Add(new Notification("AppArchipelago", $"{Core.Instance.RandoLocalizer.GetRawTextValue("VALUE_MOVESTYLE_INLINE")} {brcitem.item_name.Substring(14)}", null));
                        break;
                    case BRCType.BMX:
                        substring = brcitem.item_name.Substring(5, brcitem.item_name.Length - 6);
                        Core.Instance.Data.bmxUnlocked = true;
                        if (Core.Instance.stageManager is HideoutManager) ((HideoutManager)Core.Instance.stageManager).SetBMXGarage(true);
                        MoveStyleSkin bmx = GetBMXSkin(GetBMXAssetName(substring));
                        Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(bmx.Uid).IsUnlocked = true;
                        notifQueue.Add(new Notification("AppArchipelago", $"{Core.Instance.RandoLocalizer.GetRawTextValue("VALUE_MOVESTYLE_BMX")} {brcitem.item_name.Substring(4)}", null));
                        break;
                    case BRCType.Outfit:
                        substring = brcitem.item_name.Substring(8, brcitem.item_name.Length - 9);
                        OutfitUnlockable outfit = GetOutfitUnlockable(GetOutfitAssetName(substring));
                        Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(outfit.Uid).IsUnlocked = true;
                        notifQueue.Add(new Notification("AppArchipelago", $"{Core.Instance.RandoLocalizer.GetRawTextValue("COLLECTIBLE_OUTFIT")} {brcitem.item_name.Substring(7)}", null));
                        break;
                    case BRCType.Character:
                        Core.Instance.SaveManager.CurrentSaveSlot.characterSelectLocked = false;
                        Characters character = NameToCharacter(brcitem.item_name);
                        if (Core.Instance.Data.firstCharacter == Characters.NONE) Core.Instance.Data.firstCharacter = character;
                        if (Core.Instance.Data.limitedGraffiti) Core.Instance.Data.grafUses[character.ToString()] = 0;
                        if (character == Characters.dummy) Core.Instance.Data.dummyUnlocked = true;
                        Core.Instance.SaveManager.UnlockCharacter(character);
                        Core.Instance.PhoneManager.Phone.GetAppInstance<AppGraffiti>().OnAppRefresh();
                        notifQueue.Add(new Notification("AppArchipelago", brcitem.item_name, null));
                        break;
                    case BRCType.REP:
                        int rep = int.Parse(brcitem.item_name.Substring(0, brcitem.item_name.Length - 4));
                        Core.Instance.Data.fakeRep += rep;
                        if (Reptile.Core.Instance.BaseModule.IsPlayingInStage)
                        {
                            Traverse.Create(WorldHandler.instance.GetCurrentPlayer()).Field<float>("rep").Value = Core.Instance.Data.fakeRep;
                            Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().reputation = Core.Instance.Data.fakeRep;
                            //WorldHandler.instance.GetCurrentPlayer().ShowAddRep(rep);
                        }
                        notifQueue.Add(new Notification("AppArchipelago", brcitem.item_name, null));
                        break;
                    default:
                        break;
                }
                Reptile.Core.Instance.SaveManager.SaveCurrentSaveSlot();
            }
            else if (item is APItem apitem)
            {
                //string color = $"<color={ColorUtility.ToHtmlStringRGBA(GetAPItemColor(apitem.type))}>";
                //notifQueue.Add(new Notification("AppArchipelago", $"{color}{apitem.item_name}</color> ({apitem.player_name})", null));
                notifQueue.Add(new Notification("AppArchipelago", $"{apitem.item_name} ({apitem.player_name})", null));
            }
            else
            {
                notifQueue.Add(new Notification("AppArchipelago", $"{item.item_name} ({item.player_name})", null));
            }
        }

        public void CountAndCheckSpray()
        {
            if (Core.Instance.Data.sprayCount < 389) Core.Instance.Data.sprayCount++;
            Core.Logger.LogInfo($"Spray count is {Core.Instance.Data.sprayCount}");
            if (Core.Instance.Data.sprayCount == 389) CheckLocation("graf379");
            else if (Core.Instance.Data.sprayCount % 5 == 0) CheckLocation($"graf{Core.Instance.Data.sprayCount}");
        }

        public BRCType GetItemType(string name)
        {
            if (name.Contains("Music (")) return BRCType.Music;
            else if (name.Contains("Graffiti (M")) return BRCType.GraffitiM;
            else if (name.Contains("Graffiti (L")) return BRCType.GraffitiL;
            else if (name.Contains("Graffiti (XL")) return BRCType.GraffitiXL;
            else if (name.Contains("Skateboard (")) return BRCType.Skateboard;
            else if (name.Contains("Inline Skates (")) return BRCType.InlineSkates;
            else if (name.Contains("BMX (")) return BRCType.BMX;
            else if (name.Contains("Outfit (")) return BRCType.Outfit;
            else if (name.Contains(" REP")) return BRCType.REP;
            else return BRCType.Character;
        }

        public MusicTrack GetMusicTrack(string name)
        {
            return Reptile.Core.Instance.Assets.LoadAssetFromBundle<MusicTrack>("coreassets", $"assets/games/phone/appassets/musicplayer/musictrackunlocks/musictrack_{name}.asset");
        }

        public string GetMusicAssetName(string name)
        {
            switch (name)
            {
                case "GET ENUF":
                    return "getenuf";
                case "Chuckin Up":
                    return "chuckinup";
                case "Spectres":
                    return "spectres";
                case "You Can Say Hi":
                    return "youcansayhi";
                case "JACK DA FUNK":
                    return "jackdafunk";
                case "Feel The Funk (Computer Love)":
                    return "feelthefunk";
                case "Big City Life":
                    return "bigcitylife";
                case "I Wanna Kno":
                    return "wannakno";
                case "Plume":
                    return "plume";
                case "Two Days Off":
                    return "twodaysoff";
                case "Scraped On The Way Out":
                    return "scrapedonthewayout";
                case "Last Hoorah":
                    return "lasthoorah";
                case "State of Mind":
                    return "stateofmind";
                case "AGUA":
                    return "agua";
                case "Condensed milk":
                    return "condensedmilk";
                case "Light Switch":
                    return "lightswitch";
                case "Hair Dun Nails Dun":
                    return "hairdunnailsdun";
                case "Precious Thing":
                    return "preciousthing";
                case "Next To Me":
                    return "nexttome";
                case "Refuse":
                    return "refuse";
                case "Iridium":
                    return "iridium";
                case "Funk Express":
                    return "funkexpress";
                case "In The Pocket":
                    return "inthepocket";
                case "Bounce Upon A Time":
                    return "bounceuponatime";
                case "hwbouths":
                    return "hwbouths";
                case "Morning Glow":
                    return "morningglow";
                case "Chromebies":
                    return "chromebies";
                case "watchyaback!":
                    return "watchyaback";
                case "Anime Break":
                    return "missingbreak";
                case "DA PEOPLE":
                    return "dapeople";
                case "Trinitron":
                    return "trinitron";
                case "Operator":
                    return "operator";
                case "Sunshine Popping Mixtape":
                    return "chapter1mixtape";
                case "House Cats Mixtape":
                    return "chapter2mixtape";
                case "Breaking Machine Mixtape":
                    return "chapter3mixtape";
                case "Beastmode Hip Hop Mixtape":
                    return "chapter4mixtape";
                default:
                    return "?";
            }
        }

        public MoveStyleSkin GetSkateboardSkin(string name)
        {
            return Reptile.Core.Instance.Assets.LoadAssetFromBundle<MoveStyleSkin>("in_game_assets", $"assets/common assets/vfx/unlockables/skateboardunlockables/{name}.asset");
        }

        public string GetSkateboardAssetName(string name)
        {
            switch (name)
            {
                case "Devon":
                    return "SkateboardDeck2";
                case "Terrence":
                    return "SkateboardDeck3";
                case "Maceo":
                    return "SkateboardDeck4";
                case "Lazer Accuracy":
                    return "SkateboardDeck5";
                case "Death Boogie":
                    return "SkateboardDeck6";
                case "Sylk":
                    return "SkateboardDeck7";
                case "Taiga":
                    return "SkateboardDeck8";
                case "Just Swell":
                    return "SkateboardDeck9";
                case "Mantra":
                    return "SkateboardDeck10";
                default:
                    return "?";
            }
        }

        public MoveStyleSkin GetInlineSkin(string name)
        {
            return Reptile.Core.Instance.Assets.LoadAssetFromBundle<MoveStyleSkin>("in_game_assets", $"assets/common assets/vfx/unlockables/inlineunlockables/{name}.asset");
        }

        public string GetInlineAssetName(string name)
        {
            switch (name)
            {
                case "Glaciers":
                    return "InlineSkates2";
                case "Sweet Royale":
                    return "InlineSkates3";
                case "Strawberry Missiles":
                    return "InlineSkates4";
                case "Ice Cold Killers":
                    return "InlineSkates5";
                case "Red Industry":
                    return "InlineSkates6";
                case "Mech Adversary":
                    return "InlineSkates7";
                case "Orange Blasters":
                    return "InlineSkates8";
                case "ck":
                    return "InlineSkates9";
                case "Sharpshooters":
                    return "InlineSkates10";
                default:
                    return "?";
            }
        }

        public MoveStyleSkin GetBMXSkin(string name)
        {
            return Reptile.Core.Instance.Assets.LoadAssetFromBundle<MoveStyleSkin>("in_game_assets", $"assets/common assets/vfx/unlockables/bmxunlockables/{name}.asset");
        }

        public string GetBMXAssetName(string name)
        {
            switch (name)
            {
                case "Mr. Taupe":
                    return "BMXBike2";
                case "Gum":
                    return "BMXBike3";
                case "Steel Wheeler":
                    return "BMXBike4";
                case "oyo":
                    return "BMXBike5";
                case "Rigid No.6":
                    return "BMXBike6";
                case "Ceremony":
                    return "BMXBike7";
                case "XXX":
                    return "BMXBike8";
                case "Terrazza":
                    return "BMXBike9";
                case "Dedication":
                    return "BMXBike10";
                default:
                    return "?";
            }
        }

        public OutfitUnlockable GetOutfitUnlockable(string name)
        {
            return Reptile.Core.Instance.Assets.LoadAssetFromBundle<OutfitUnlockable>("in_game_assets", $"assets/common assets/vfx/unlockables/playeroutfits/{name}.asset");
        }

        public string GetOutfitAssetName(string name)
        {
            switch (name)
            {
                case "Red - Autumn":
                    return "MetalheadOutfit3";
                case "Red - Winter":
                    return "MetalheadOutfit4";
                case "Tryce - Autumn":
                    return "BlockGuyOutfit3";
                case "Tryce - Winter":
                    return "BlockGuyOutfit4";
                case "Bel - Autumn":
                    return "SpacegirlOutfit3";
                case "Bel - Winter":
                    return "SpacegirlOutfit4";
                case "Vinyl - Autumn":
                    return "VinylOutfit3";
                case "Vinyl - Winter":
                    return "VinylOutfit4";
                case "Solace - Autumn":
                    return "DummyOutfit3";
                case "Solace - Winter":
                    return "DummyOutfit4";
                case "Felix - Autumn":
                    return "LegendFaceOutfit3";
                case "Felix - Winter":
                    return "LegendFaceOutfit4";
                case "Rave - Autumn":
                    return "AngelOutfit3";
                case "Rave - Winter":
                    return "AngelOutfit4";
                case "Mesh - Autumn":
                    return "WideKidOutfit3";
                case "Mesh - Winter":
                    return "WideKidOutfit4";
                case "Shine - Autumn":
                    return "BunGirlOutfit3";
                case "Shine - Winter":
                    return "BunGirlOutfit4";
                case "Rise - Autumn":
                    return "PufferGirlOutfit3";
                case "Rise - Winter":
                    return "PufferGirlOutfit4";
                case "Coil - Autumn":
                    return "RingDudeOutfit3";
                case "Coil - Winter":
                    return "RingDudeOutfit4";
                default:
                    return "?";
            }
        }

        public Characters NameToCharacter(string name)
        {
            switch (name)
            {
                case "Tryce":
                    return Characters.blockGuy;
                case "Bel":
                    return Characters.spaceGirl;
                case "Vinyl":
                    return Characters.girl1;
                case "Solace":
                    return Characters.dummy;
                case "Felix":
                    return Characters.legendFace;
                case "Rave":
                    return Characters.angel;
                case "Mesh":
                    return Characters.wideKid;
                case "Shine":
                    return Characters.bunGirl;
                case "Rise":
                    return Characters.pufferGirl;
                case "Coil":
                    return Characters.ringdude;
                case "Frank":
                    return Characters.frank;
                case "Rietveld":
                    return Characters.jetpackBossPlayer;
                case "DJ Cyber":
                    return Characters.dj;
                case "Eclipse":
                    return Characters.medusa;
                case "DOT.EXE":
                    return Characters.eightBall;
                case "Devil Theory":
                    return Characters.boarder;
                case "Flesh Prince":
                    return Characters.prince;
                case "Futurism":
                    return Characters.futureGirl;
                case "Oldhead":
                    return Characters.oldheadPlayer;
                default:
                    return Characters.metalHead;
            }
        }

        public Color GetAPItemColor(ItemFlags flag)
        {
            switch (flag)
            {
                case ItemFlags.Advancement:
                    return Core.configColorItemAdvancement.Value;
                case ItemFlags.NeverExclude:
                    return Core.configColorItemNeverExclude.Value;
                case ItemFlags.Trap:
                    return Core.configColorItemTrap.Value;
                default:
                    return Core.configColorItemFiller.Value;
            }
        }
    }
}
