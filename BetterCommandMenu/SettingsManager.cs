using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine;

namespace BetterCommandMenu
{
    class SettingsManager
    {
        public static string modGuid = "org.mries92.BetterCommandMenu";
        public static ConfigFile configFile;
        // A server side list to keep track of players cooldown times and when they last clicked an item
        public static List<PlayerCooldownInfo> playerCooldowns = new List<PlayerCooldownInfo>();

        // UI
        public static ConfigEntry<bool> disableBlur, disableBackground, disableSpinners, disableColoredOverlay, disableCancelButton, disableLabel;
        public static ConfigEntry<float> menuXOffset, menuYOffset;
        // Tooltips
        public static ConfigEntry<bool> tooltipEnabled;
        // Escape
        public static ConfigEntry<bool> closeWithEscape;
        // Counters
        public static ConfigEntry<bool> countersEnabled, showEmptyStacks;
        public static ConfigEntry<string> alignment, prefix;
        public static ConfigEntry<int> fontSize;
        public static ConfigEntry<float> borderSize;
        public static ConfigEntry<Color> fontColor, borderColor;
        public static ConfigEntry<float> counterXOffset, counterYOffset;
        // Protection
        public static ConfigEntry<bool> protectionEnabled, forceClientSettings;
        public static ConfigEntry<string> protectionType;
        public static ConfigEntry<int> protectionTime, protectionCooldown, protectionShieldAmount;
        

        public static void Init()
        {
            // Base config file
            configFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, modGuid + ".cfg"), true);

            // UI tweaks
            bool defaultDisableBlur = false;
            bool defaultDisableBackground = false;
            bool defaultDisableSpinners = false;
            bool defaultDisableColoredOverlay = false;
            bool defaultDisableCancelButton = false;
            bool defaultDisableLabel = false;
            float defaultMenuXOffset = 0, defaultMenuYOffset = 0;
            disableBlur = configFile.Bind<bool>("ui", "disableBlur", defaultDisableBlur, new ConfigDescription("Disable the blur behind the command menu. Allows you to see your current health and buffs."));
            disableBackground = configFile.Bind<bool>("ui", "disableBackground", defaultDisableBackground, new ConfigDescription("Disable the background behind the command menu."));
            disableSpinners = configFile.Bind<bool>("ui", "disableSpinners", defaultDisableSpinners, new ConfigDescription("Disable the spinning decorations around the command menu."));
            menuXOffset = configFile.Bind<float>("ui", "menuXOffset", defaultMenuXOffset, new ConfigDescription("Allows you to move the command menus location (does not apply to the scrapper menu). This is the X offset."));
            menuYOffset = configFile.Bind<float>("ui", "menuYOffset", defaultMenuYOffset, new ConfigDescription("Allows you to move the command menus location (does not apply to the scrapper menu). This is the Y offset."));
            disableColoredOverlay = configFile.Bind<bool>("ui", "disableColoredOverlay", defaultDisableColoredOverlay, new ConfigDescription("Disables the coloration on the command window representing the rarity. You probably want to remove this as well if you are removing the background."));
            disableCancelButton = configFile.Bind<bool>("ui", "disableCancelButton", defaultDisableCancelButton, new ConfigDescription("Removes the cancel button on command windows. Careful with this one, you defnitely want to have `closeWithEscape` enabled or you won't be able to leave a command menu without picking an item. Will probably check for this in the future.)"));
            disableLabel = configFile.Bind<bool>("ui", "disableLabel", defaultDisableLabel, new ConfigDescription("Hide the text at the top of the command and scrapper menus."));

            // Tooltips
            bool defaultTooltipEnabled = true;
            tooltipEnabled = configFile.Bind<bool>("tooltips", "tooltipEnabled", defaultTooltipEnabled, new ConfigDescription("Toggle tooltips on/off"));
            
            // Escape
            bool defaultCloseWithEscape = true;
            closeWithEscape = configFile.Bind<bool>("escape", "closeWithEscape", defaultCloseWithEscape, new ConfigDescription("Toggle escape closing menus"));
            
            // Counters
            bool defaultCountersEnabled = true;
            bool defaultShowEmptyStacks = true;
            int defaultFontSize = 24;
            Color defaultFontColor = Color.white;
            Color defaultBorderColor = Color.black;
            float defaultBorderSize = .5f;
            string defaultPrefix = "";
            string defaultAlignment = "br";
            float defaultCounterXOffset = 0, defaultCounterYOffset = 0;
            countersEnabled = configFile.Bind<bool>("counters", "countersEnabled", defaultCountersEnabled, new ConfigDescription("Toggle the visibility of item counters."));
            showEmptyStacks = configFile.Bind<bool>("counters", "showEmptyStacks", defaultShowEmptyStacks, new ConfigDescription("Should item counters be shown if you don't have any of that item?"));
            fontSize = configFile.Bind<int>("counters", "fontSize", defaultFontSize, new ConfigDescription("The font size for item counters."));
            prefix = configFile.Bind<string>("counters", "prefix", defaultPrefix, new ConfigDescription("The text that should prefix the item count (eg. 'x' for 'x3')."));
            string[] acceptableAlignmentValues = { "br", "bl", "tr", "tl", "c" };
            alignment = configFile.Bind<string>("counters", "alignment", defaultAlignment, new ConfigDescription("The alignment of the text on the buttons (bl - bottom left, tr - top right, etc.)", new AcceptableValueList<string>(acceptableAlignmentValues)));
            fontColor = configFile.Bind<Color>("counters", "fontColor", defaultFontColor, new ConfigDescription("The color of the item counter font. This is a hex string in the format of RGBA."));
            borderColor = configFile.Bind<Color>("counters", "borderColor", defaultBorderColor, new ConfigDescription("The border color of the item counter font. This is a hex string in the format of RGBA."));
            borderSize = configFile.Bind<float>("counters", "borderSize", defaultBorderSize, new ConfigDescription("The border width of the item counter font. '.5' is a good default for the default font size."));
            counterXOffset = configFile.Bind<float>("counters", "counterXOffset", defaultCounterXOffset, new ConfigDescription("Offset the item counters by a given amount. This may need to be used if you use extreme text sizes to realign the text. This is the X offset."));
            counterYOffset = configFile.Bind<float>("counters", "counterYOffset", defaultCounterYOffset, new ConfigDescription("Offset the item counters by a given amount. This may need to be used if you use extreme text sizes to realign the text. This is the Y offset."));

            // Protection
            bool defaultProtectionEnabled = false;
            bool defaultForceClientSettings = false;
            string defaultProtectionType = "invisible";
            int defaultProtectionTime = 6;
            int defaultProtectionCooldown = 30;
            int defaultProtectionShieldAmount = 100;
            protectionEnabled = configFile.Bind<bool>("protection", "protectionEnabled", defaultProtectionEnabled, new ConfigDescription("Toggle protection on/off"));
            forceClientSettings = configFile.Bind<bool>("protection", "forceClientSettings", defaultForceClientSettings, new ConfigDescription("Should clients be forced to use the servers/hosts protection settings?"));
            string[] acceptableProtectionValues = { "invisible", "shield", "invincible" };
            protectionType = configFile.Bind<string>("protection", "protectionType", defaultProtectionType, new ConfigDescription("The type of protection used when a command menu is opened.", new AcceptableValueList<string>(acceptableProtectionValues)));
            protectionTime = configFile.Bind<int>("protection", "protectionTime", defaultProtectionTime, new ConfigDescription("The lengh of time in seconds protection should be enabled for. (Does not apply to `shield` protection type)."));
            protectionCooldown = configFile.Bind<int>("protection", "protectionCooldown", defaultProtectionCooldown, new ConfigDescription("Limit how often protection can be granted"));
            protectionShieldAmount = configFile.Bind<int>("protection", "protectionShieldAmount", defaultProtectionShieldAmount, new ConfigDescription("If protection type is set to `shield` this is how much should be applied. This is a percentage of your max shield, from 1-100."));
        }

        public static BuffIndex GetProtectionBuffIndex()
        {
            switch(protectionType.Value)
            {
                case "invisible":
                    return BuffIndex.Cloak;
                case "invincible":
                    return BuffIndex.HiddenInvincibility;
                default:
                    return BuffIndex.None;
            }
        }
    }
}
