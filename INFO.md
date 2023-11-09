# BRC Rando Beta

## Info

This is a randomizer for Bomb Rush Cyberfunk made to work with the [Archipelago](https://archipelago.gg/) collection of randomizers, meaning that it can be played solo or in a multiworld with any of the other [supported games](https://archipelago.gg/games) that Archipelago is compatible with.

Several changes have been made to the game for a better experience as a randomizer:

- The prelude in the police station can be skipped.
- The map for each stage is always unlocked.
- Only Red is unlocked when starting a new file.
- No M, L, or XL graffiti is unlocked at the beginning.
- The taxi is unlocked from the beginning, but you will still need to visit the taxi stops before you can use them.
- REP is no longer earned from doing graffiti, and is instead earned by finding it as items in the world.
- One single REP count is used throughout the game, instead of having separate totals for each stage. REP requirements are the original game's requirements, but added together in order. 960 REP is needed to finish the game.

Items can be found by picking up any type of collectible, unlocking characters, and for every 5 graffiti spots tagged. The types of items that can be found are Music, Graffiti (M), Graffiti (L), Graffiti (XL), Skateboards, Inline Skates, BMX, Outfits, Characters, and REP.

Movestyles will also be unlocked by finding a character that has that movestyle as their default. (ex. unlocking Shine will also unlock BMX)

The mod adds two new apps to the phone, an "Encounter" app which lets you quit certain encounters early, and the "Archipelago" app which lets you view chat messages and item history while in game.


## Options

There are several options that can be changed before generating a run:

- `skip_intro`: Skips escaping the police station. Graffiti spots tagged during the intro will not unlock items.
- `skip_dreams`: Skips the dream sequences at the end of each chapter.
- `total_rep`: Change the total amount of REP in the multiworld.
	- 960 REP is needed to finish the game. Reducing the total amount will therefore increase the amount of locations that must be checked to find enough REP to finish.
	- Normal = 1792, Less = 1472, Much Less = 1184
- `starting_movestyle`: Choose which movestyle to start with.
- `limited_graffiti`: Each graffiti design can only be used a limited number of times before being removed from your inventory.
	- M and L graffiti can be used 10 times, XL graffiti can be used 6 times.
	- Small graffiti is not affected.
	- In some cases, such as completing a dream, using graffiti to defeat enemies, or spraying over your own graffiti, uses will not be counted.
	- If this option is enabled, spraying graffiti will be disabled during crew battles to prevent softlocking.
- `harder_crew_battles`: Significantly increases the score requirements for each crew battle.
- `damage_multiplier`: Multiplies all damage received.
	- At 3x, most damage will OHKO the player, including falling into pits. At 6x, all damage will OHKO the player.


## Setup (Server)

**Note that only one player in the multiworld will need to complete these steps.**

1. Download and install [Archipelago](https://github.com/ArchipelagoMW/Archipelago/releases). You can skip installing the games you don't plan on playing, but make sure that `Generator` is checked. Optionally, you can also check `Text, to !command and chat` under Clients if you would like to be able to send chat messages while playing.

2. Download `bomb_rush_cyberfunk.apworld` from the pins in Discord and add it to your worlds folder. (Default path: `C:\ProgramData\Archipelago\lib\worlds`)

3. Set up all `.yaml` files for each player in the multiworld, then add them to the Players folder. A template for BRC can also be downloaded from Discord.

4. Run `ArchipelagoGenerate.exe` to generate a game, which will appear in the output folder.

5. Host a game, either manually by running `ArchipelagoServer.exe` (if installed), or by [uploading](https://archipelago.gg/uploads) it to the Archipelago website and clicking Create New Room. The address to use when connecting will appear above the list of players.


## Setup (Client)

The mod for Bomb Rush Cyberfunk Randomizer can either be installed manually by adding it to your plugins folder after setting up [BepInEx](https://github.com/BepInEx/BepInEx/releases), or by using a mod manager like [r2modman](https://thunderstore.io/package/ebkr/r2modman/), selecting a profile, then going to Settings > Import Local Mod. 

There are some other mods that are optional but recommended for a better experience:

- [CutsceneSkip](https://thunderstore.io/c/bomb-rush-cyberfunk/p/Jay/CutsceneSkip/) by Jay
    - Makes every cutscene skippable. In the mod's description, it mentions that skipping certain cutscenes might break things. The only example of this that I've seen is skipping the cutscene of entering a bathroom will not show the outfit swap menu and will not remove your heat.
- [QuickStyleSwap](https://thunderstore.io/c/bomb-rush-cyberfunk/p/Yuri/QuickStyleSwap/) by Yuri
    - Quickly change movestyles at any time by holding the movestyle equip button and pressing any of the three trick buttons.
- [GimmeMyBoost](https://thunderstore.io/c/bomb-rush-cyberfunk/p/Yuri/GimmeMyBoost/) by Yuri
    - Retains boost when loading into a new stage.
- [DisableAnnoyingCutscenes](https://thunderstore.io/c/bomb-rush-cyberfunk/p/viliger/DisableAnnoyingCutscenes/) by viliger
    - Disables the police cutscenes when increasing your heat level.
- [FastTravel](https://thunderstore.io/c/bomb-rush-cyberfunk/p/tari/FastTravel/) by tari
    - Call for a taxi from anywhere.

After someone has opened a room by following the server setup instructions, and the mod has been installed correctly, you can join a room by clicking the Archipelago button next to a save file. Note that this button won't do anything if clicked on a file that has vanilla save data, it will only work if the save file is empty or was already used to play randomizer before.

When you click the Archipelago button, it will open a menu that looks like this:

![20230914000344_1](https://github.com/TRPG0/BRC-Archipelago/assets/80716066/22bc010a-e643-456e-b777-32495830c6d1)

Enter your name, which is the name used in the `.yaml` file, the address of the server to connect to, and the password if there is one. If not, then the password can be left empty. Then click the checkmark to connect to the server. An easy way to be sure if it's working correctly or not is by opening the phone and checking if the Encounter and Archipelago apps are there.
