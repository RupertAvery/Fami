# Fami - Famicom/NES Emulator

**Fami** is a NES emulator written in C#.  The name is derived from the Japanese model of the NES, the Family Computer or Famicom.

The main goal was to build a fast NES emulator with sound in C#.

When running in Debug mode, the emulator manages around 40-50 fps, but runs full speed when running in Release mode on a Intel(R) Core(TM) i7-9750H CPU @ 2.60GHz.

# Screenshots

![image](https://user-images.githubusercontent.com/1910659/132545071-78baf290-197d-4dd0-b9b5-a9d4fb778295.png)

![image](https://user-images.githubusercontent.com/1910659/132545227-e33430e0-f5b6-483b-b72a-4693ef096053.png)

![image](https://user-images.githubusercontent.com/1910659/132545346-a13d6b1e-859b-49eb-88da-f3ea6cb9ce98.png)

![image](https://user-images.githubusercontent.com/1910659/132545470-067b202f-ff32-473e-9d10-f5c6e52aa412.png)


# Prerequisites

* .NET 5 SDK
* Visual Studio 2019 v16+

# Features

* Zapper/Light-gun support
* Support for using controllers
* Save/Load States (only slot 1 right now)
* Rewind (up to 8 seconds)

# Todo

* Button mapping through a configuration file
* Menus and dialogs
* Implement more mappers
* Fullscreen

# Usage

```
fami.exe "<path to rom>"
```

# Key mapping

The mappings don't use Space or Enter or Escape since Steam maps the controller to these buttons.

|  Key        |  Function    |
|-------------|--------------|
|  Up         |  D-Pad Up    |
|  Down       |  D-Pad Down  |
|  Left       |  D-Pad Left  |
|  Right      |  D-Pad Right |
|  V          |  Start       |
|  C          |  Select      |
|  Z          |  Button A    |
|  X          |  Button B    |
|  R          |  Reset       |
|  F2         |  Save State  |
|  F4         |  Load State  |
|  Backspace  |  Rewind      |

# Mappers

* NROM (000)
* MMC1 (001)
* UxROM (002)
* MMC3 (004)
* AxROM (007)

# PPU

The PPU is a port of the C++ PPU used in One Lone Coders NES Emulation Tutorial and retains all the comments.

# APU

The APU is taken from the BizHawk NES APU found here: https://github.com/TASVideos/BizHawk/blob/master/src/BizHawk.Emulation.Cores/Consoles/Nintendo/NES/APU.cs

I decided to use it as it gave the best audio output.

It uses blargg's `blip_buf` library to effectively generate samples that are timed against an emulation clock (it coincides with every tick of a CPU cycle) which greatly simplifies synchronization of audio.  

# Known Issues

## Zapper/Light gun 

If you move the mouse immediately after firing, the sensor position will move, causing your aim to be off from where you clicked. 

Right-clicking to trigger "outside" of the screen kind of works.  It's good enough for game mode selection screens, but doesn't work well during actual gameplay. It still registers as a trigger "inside" the screen. This is probably caused by timing issues.
