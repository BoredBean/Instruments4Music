# Instruments4Music
## Introduction：
This plugin aims to introduce some musical instruments to the Lethal Company.

This plugin utilizes [LethalCompany InputUtils](https://thunderstore.io/c/lethal-company/p/Rune580/LethalCompany_InputUtils/) for configuring key binds.

**The developer has limited knowledge of C# or Unity, so this plugin may not work very well.**

## How to use:
Aim at a possible stationary instrument or holding a portable instrument and press **"BackSpace"** for **1 second**.

Possible stationary instruments: **Ship Horn, Light Switch, Pumpkin Head...**

Possible portable instruments: **Clown Horn, Air Horn, Air Blower...**

You can **replace the origin audio** with [CustomSound](https://github.com/clementinise/CustomSounds) and create your own instrument.

<br />

You can then play an instrument with the default key bindings:

| Note Name           | C | D | E | F | G | H | I |
| ------------------- | - | - | - | - | - | - | - |
| **Primary**   |   |   |   |   |   |   |   |
| Higher              | q | w | e | r | t | y | u |
| Mid                 | a | s | d | f | g | h | j |
| Lower               | z | x | c | v | b | n | m |
| **Secondary** |   |   |   |   |   |   |   |
| Higher              | / |   |   |   |   |   |   |
| Mid                 | k | l | ; | n | m | , | . |
| Lower               | y | u | i | o | p | h | j |

* Press **"Shift"** to play **semitone**: C#, D#, F#, G#, A#
* Press **"Ctrl"** as the **soft padel**.
* Press **"Space"** as the **sustain padel**.
* Press **"Tab"** to switch to the **secondary key binding** scheme.
(The player **can't move or interact** in the **primary** mode, but is **free** in the **secondary** mode.)
* Press **"Enter"** to input an autoplay code, **press again** to **start autoplay**.
* Press **"ESC"** to **exit** playing mode.

[A Demo Video](https://www.bilibili.com/video/BV13r421s7rM)

[A Demo Bad Apple](https://www.bilibili.com/video/BV17z421R7CB)

## Auto Play：
The design of autoplay code is more readable than simpler.

This is an example of the code for autoplay. (start from 02:55)

[AutoPlay Demo (start from 02:55)](https://www.bilibili.com/video/BV1hx42117r8/?share_source=copy_web&t=175)

```text
2.7,01030,,,,030,,,,032,,,,042,,,,0b030,,,,030,,,,030,,,,
010,,020,,0a030,,,,030,,,,032,,,,042,,,,0g052,,,,040,,,,030,,,,020,,,,
0f010,,,,010,,,,010,,,,022,,,,0e010,,,,010,,,,010,,,,050,,,,
0d010,,,,010,,020,,030,,020,,010,,,,0g010,,,,010,,020,,030,,020,,010,,020,,
0c030,,0g0,,0e030,,0g0,,0c032,,0g0,,0d042,,0g0,,0b030,,0g0,,0d030,,0g0,,0b030,,0g0,,
0c010,,020,,0a030,,0e0g0,,0c030,,0g0,,0a032,,0e0g0,,0c032,,040,,0g052,,0e0g0,,0c010,,0e0g0,,0g050,,040,,0c030,,0e020,,
0f010,,0c0g0,,0a010,,0c0g0,,0f010,,0c0g0,,0a052,,0c0g0,,0e0C0,,0c010,,0g050,,0c040,,0e030,,020,,0g010,,0c0g0,,
0d010,,0c0g0,,0f010,,0a020,,0d030,,020,,0f010,,0c0g0,,0g010,,0c0g0,,0c010,,0f050,,0g040,,030,,0b020,,,,
0c030,,0g0,,0e030,,0c0g0,,0c032,,0c0g0,,0f042,,0g0,,0b030,,0c0g0,,0d030,,0c0g0,,0e030,,0d0g0,,
0g010,,0d020,,0a030,,0e0a0,,0c030,,0e0a0,,0a032,,0e0a0,,0d042,,0e0a0,,0g052,,0c010,,0a0C0,,0f0,,0c050,,040,,0e030,,0d020,,
0f010,,0c0g0,,0f010,,0c0g0,,0g010,,0c0g0,,0a052,,,,0e0C0,,0d0,,0g050,,0c040,,0e030,,020,,0g010,,0c0g0,,
0d010,,0c0g0,,0f010,,0a020,,0d030,,020,,010,,0g0,,0g010,,0c0g0,,0c010,,0g050,,0f040,,0e033,,0c024,,,,,,
0c017,,0g0,,,,2g0,,,,2g0,,,,,,0c0
```

The first value, **2.7** is the playback speed. The higher the faster (similar to **bmp** somehow).

The other values, for example **0f012GA**:

* The head number **0** controls the soft padel and the sustain padel.

| Value | IsSoft | IsSustaining |
| --- | --- | --- |
| 0 | false | false |
| 1 | false | true |
| 2 | true | false |
| 3 | true | true |

* The **f0** means play **Lower F** for **1** cycle.
* The **12** means play **Mid C** for **3** cycles, at the same time.
* The **GA** means play **Higher G#** for **3** cycles, at the same time.

You can play a single note or multiple notes at the same time.

| Type | The Note |
| --- | --- |
| Lower note: | c, d, e, f, g, a, b |
| Mid note: | 1, 2, 3, 4, 5, 6, 7 |
| Higher note: | C, D, E, F, G, A, B |

* The hex number **0/2/A** following the note represents the **duration** of the note, range **0-7**.

When the note is a **semitone**, the duration number needs to be **increased by 8**.

| Semi? | The Hex Number |
| --- | --- |
| Duration | 0, 1, 2, 3, 4, 5, 6, 7 |
| Duration(#) | 8, 9, A, B, C, D, E, F |

* Use commas **","** to separate the notes played in each cycle.

## Future Plans：
1. ✅Introduce more stationary or portable instruments.
2. ✅Implement a user interface for music playback.
3. ✅Prevent the player from moving or interacting with objects while playing music.
4. Synchronize music playback with other players.
5. ✅Implement auto music playing function.
6. ✅Add more custom configurations.

## This project studied the following repositories:
1. [LethalCompanyInputUtils](https://github.com/Rune580/LethalCompanyInputUtils)(LGPL-3.0 license)
2. [LCBetterSprayPaint](https://github.com/taffyko/LCBetterSprayPaint)(MIT license)
3. [LC-Touchscreen](https://github.com/TheDeadSnake/LC-Touchscreen)(null)
4. [MirrorDecor](https://github.com/quackandcheese/MirrorDecor)(null)
5. [EladsHUD](https://github.com/EladNLG/EladsHUD)(GPL-3.0 license)

I'm not entirely certain about the licensing, but I have studied their code superficially.

Please let me know if there's anything I've overlooked or done incorrectly.