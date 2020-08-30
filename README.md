# BetterCommandMenu
Various improvements to the RoR2 command and scrapper menus.
## Features
- Shows item names and statistics on hover
- Displays a counter to show how many of each item you have
- Allows you to close menus with the escape key
- Optional protection settings to keep you safe while deciding at the command menu (configurable)
## Configuration
Creates a config file in `BepInEx/config` on first load. Configurable includes...

- Font size, color, border, and alignment options for item counters

    - ![font examples](ReadmeResources/IconGrid.png)
- Enable/Disable flags for every feature
- Various options to configure protection. Each clients protection settings are honored in multiplayer.
## Integrations
-  [ItemStatsMod](https://thunderstore.io/package/ontrigger/ItemStatsMod/) - Will display the statistics from ItemStatsMod if enabled
## Changelog
### 1.3.0
- Added protection system with 3 initial protection types.
- Added many more configuration options, and enable/disbale flags for all features
### 1.2.0
- Added abilty to press escape to close menus
### 1.1.0
- Tweaks to configuration options
### 1.0.1
- Fixed tooltip not showing up on equipment
### 1.0.0
- Initial release. Moved from HoverStats. Added configuration options.