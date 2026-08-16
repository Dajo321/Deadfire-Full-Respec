# Deadfire Full Respec

Pillars of Eternity II: Deadfire mod using the BepInEx framework.

This mod turns the shop character respec system into a full character recreation from level 1 with default selections pulled from the character's existing data.

This is currently only tested for the Player character and custom Adventurer characters. I have a strong feeling this currently breaks Companion and Sidekick character respec.

## ⚠️ Disclaimer

This mod is in early development. Please make sure to backup your save files before installing and read the [**Known Gaps**](#known-gaps-im-working-on) and [**To Be Tested**](#to-be-tested) sections to learn more about what works and what doesn't.

## Defaults From Existing Character

Full Respec will use much of the existing player's data for its default selections:

1. Gender
2. Race
3. Subrace
4. Attribute point allocation
5. Culture
6. Background
7. Customization (face, hair, colors, voice, pose, portrait)

## Known Gaps I'm Working On

These are things I'm super aware of and are first on the ever growing todo list:

1. Equipment wiped on completion
2. Watcher abilities (and other "granted" abilities) wiped on completion
3. I broke non-custom character respec :(
   
## To Be Tested

I'm still discovering the full ramifications of my hubris:

1. "Granted" passives
2. Companions and Sidekicks
   - I'd really like to figure out full class selection options for non-custom characters

## Not Planned

This is functionality that I recognize I could try to implement but either doesn't really feel necessary or convenient for the user experience (at least to me), and/or might introduce unnecessary complexity and instability to the mod:

1. Class(es) and class starting abilities
2. Weapon proficiencies 

## Install Instructions

This mod requires the BepInEx framework. As far as I'm aware, Deadfire runs on the Mono-based Unity v5.6 engine. This should be supported by the latest version of [BepInEx 5.X](https://github.com/bepinex/bepinex/releases). It was only tested with the BepInEx_win_x64 5.4.23.5 version.

1. Download [**BepInEx 5.X**](https://github.com/bepinex/bepinex/releases).
2. Extract the .zip to your game directory - e.g. "C:\...\steamapps\common\Pillars of Eternity II".
   - You should end up with a "BepInEx" folder (and potentially others) in your game directory.
4. Run the game once for BepInEx setup.
5. Download **Deadfire Full Respec**.
6. Extract the .zip contents to the BepInEx **plugins** folder - e.g. "C:\...\steamapps\common\Pillars of Eternity II\BepInEx\plugins".
   - This should result in one **DeadfireFullRespec.dll** in the plugins folder.
6. Run the game and try it out!
