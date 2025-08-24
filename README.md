# AlwaysDisplayPlayerName

[English](README.md) | [中文](README_CN.md)

A BepInEx mod for PEAK game that always displays other players' names and distance information.

## Features

- **Always Display Player Names**: Shows other players' names regardless of whether you're looking at them
- **Distance Display**: Shows the distance between current player and other players
- **Configurable Options**: 
  - Enable/disable mod
  - Set visible angle
  - Control display when blind
  - Toggle distance display
- **Performance Optimized**: Uses interval update mechanism to reduce performance overhead

## Installation

### Prerequisites
- PEAK game
- BepInEx 5.4.2403 or higher

### Installation Steps
1. Download the mod files
2. Place `com.github.yueby.AlwaysDisplayPlayerName.dll` into the game's `BepInEx/plugins/` directory
3. Launch the game, the mod will load automatically

### Recommended Installation
We recommend using the Thunderstore mod manager for installation, or manually download from the [Thunderstore mod page](https://thunderstore.io/c/peak/p/Yueby/AlwaysDisplayPlayerName/).

## Configuration

The mod provides the following configuration options (adjustable in-game or via config files):

- **Enable**: Enable/disable the mod
- **VisibleAngle**: Set visible angle (default 52 degrees)
- **DisplayWhenBlind**: Whether to display names when blind
- **ShowDistance**: Whether to show distance information

## Technical Implementation

- Uses Harmony library for game code patching
- Implements distance display based on Unity UI system
- Adopts component-based design for easy maintenance and extension

## Contributing

Issues and Pull Requests are welcome to improve this mod!

## Related Links

- [Thunderstore Mod Page](https://thunderstore.io/c/peak/p/Yueby/AlwaysDisplayPlayerName/) - Download, rate, and community discussions
- [PEAK Modding Guide](https://peakmodding.github.io/getting-started/overview/) - Learn how to create mods for PEAK game
