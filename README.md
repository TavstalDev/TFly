# TFly

![Release (latest by date)](https://img.shields.io/github/v/release/TavstalDev/TFly?style=plastic-square)
![Workflow Status](https://img.shields.io/github/actions/workflow/status/TavstalDev/TFly/release.yml?branch=stable&label=build&style=plastic-square)
![License](https://img.shields.io/github/license/TavstalDev/TFly?style=plastic-square)
![Downloads](https://img.shields.io/github/downloads/TavstalDev/TFly/total?style=plastic-square)
![Issues](https://img.shields.io/github/issues/TavstalDev/TFly?style=plastic-square)

## What is TFly?

TFly is a server-side plugin for [Unturned](https://store.steampowered.com/app/304930/Unturned/) 3.24.x+ servers that allows players to defy gravity and take flight. Built on the [Rocket Mod](https://rocketmod.net/) framework.

## Features

- **Flight Mode** - Toggle flight on/off with a single command
- **Speed Control** - Adjust flight speed in real-time using the mouse scroll wheel
- **God Mode** - Optional invincibility while flying
- **Swim Animation** - Realistic swimming animation during flight
- **Admin Controls** - Manage flight for individual players or the entire server
- **Cooldown System** - Configurable cooldown between flight toggles
- **Auto-Heal** - Automatically heals broken legs from landing
- **Localization** - Supports multiple languages via TLibrary

## Requirements

- Unturned 3.24.x or later
- [RocketMod](https://rocketmod.net/) installed on the server

## Installation

1. Download the latest release and its libraries from the [Releases](https://github.com/TavstalDev/TFly/releases) page.
2. Place `TFly.dll` into your server's `Rocket/Plugins/` directory.
3. Extract the libraries archive into `Rocket/Libraries` directory.
4. Start or restart the server. The plugin will generate a default YAML configuration file on first load.
5. Edit the configuration file to your liking, then reload the plugin or restart the server.

## Configuration

On first load, TFly generates a YAML configuration file. Below are the available settings:

| Setting                 | Type | Default | Description                                     |
|-------------------------|---|------|-------------------------------------------------|
| `FlyAnimationEnabled`   | `bool` | `true` | Enables swimming animation while flying         |
| `GodModeWhenFlyEnabled` | `bool` | `true` | Grants invincibility while flying               |
| `CooldownInSeconds`     | `double` | `30` | Cooldown (seconds) between flight toggles       |
| `DefaultFlySpeed`       | `float` | `10` | Base movement speed multiplier                  |
| `FlyUpSpeed`            | `float` | `0.3` | Vertical movement per tick when ascending       |
| `Gravity`               | `float` | `0`  | Gravity multiplier while flying (0 = no gravity) |

## Commands

| Symbol | Meaning      |
|--------|--------------|
| `-`    | **or**       |
| `[]`   | **required** |
| `<>`   | **optional** |

<details>
<summary><b>/fly</b></summary>

Toggles flight mode for the calling player.

**Aliases:** `/flight`

**Permission(s):** `tfly.commands.fly`, `tfly.commands.fly.admin`

**Notes:**
- Respects the configured cooldown (bypassed with `tfly.commands.fly.admin` permission)
- Resets fly speed to default on each toggle
- While flying, use the mouse scroll wheel to adjust speed
- Press **jump/space** to ascend, **crouch** to descend
</details>

<details>
<summary><b>/flyadmin [player] &lt;on | off&gt; | all &lt;on | off&gt;</b></summary>

Admin command to toggle flight mode for a specific player or all players.

**Aliases:** `/flya`, `/flighta`, `/flightadmin`

**Permission(s):** `tfly.commands.flyadmin`

**Usage:**
- `/flyadmin <player> on` - Enable flight for a player
- `/flyadmin <player> off` - Disable flight for a player
- `/flyadmin all on` - Enable flight for all online players
- `/flyadmin all off` - Disable flight for all online players
</details>

<details>
<summary><b>/vTFly</b></summary>

Displays the plugin's build version and build date.

**Permission(s):** `tfly.version`
</details>

## Permissions

| Permission                | Description                                       |
|---------------------------|---------------------------------------------------|
| `tfly.commands.fly`       | Allows using `/fly` to toggle personal flight     |
| `tfly.commands.fly.admin` | Bypasses flight cooldown                          |
| `tfly.commands.flyadmin`  | Allows using `/flyadmin` to control other players |
| `tfly.version`            | Allows using `/vTFly` to view plugin version      |

## Building from Source

### Prerequisites

- .NET Framework 4.8 SDK / targeting pack

### Build Steps

1. Clone the repository:
   ```
   git clone https://github.com/TavstalDev/TFly.git
   ```
2. Open `TFly.sln` in your IDE.
3. Build the project:
   ```
   dotnet build -c Release
   ```
4. The compiled `TFly.dll` will be in `TFly/bin/Release/`.

## License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for more details.

## Contact

For issues or feature requests, please use the [GitHub issue tracker](https://github.com/TavstalDev/TFly/issues).
