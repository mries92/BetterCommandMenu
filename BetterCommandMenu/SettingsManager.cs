using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BetterCommandMenu
{
    class SettingsManager
    {
        public static string modGuid = "org.mries92.BetterCommandMenu";
        public static ConfigFile configFile;
        public static ConfigEntry<bool> tooltipEnabled, closeWithEscape, countersEnabled, protectionEnabled;
        public static ConfigEntry<string> alignment, prefix, protectionType;
        public static ConfigEntry<int> fontSize, protectionTime, protectionCooldown, protectionShieldAmount;
        public static ConfigEntry<float> borderSize;
        public static ConfigEntry<Color> fontColor, borderColor;

        public static void Init()
        {
            // Base config file
            configFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, modGuid + ".cfg"), true);
            
            // Tooltips
            bool defaultTooltipEnabled = true;
            tooltipEnabled = configFile.Bind<bool>("tooltips", "tooltipEnabled", defaultTooltipEnabled, new ConfigDescription("Toggle tooltips on/off"));
            
            // Escape
            bool defaultCloseWithEscape = true;
            closeWithEscape = configFile.Bind<bool>("escape", "closeWithEscape", defaultCloseWithEscape, new ConfigDescription("Toggle escape closing menus"));
            
            // Counters
            bool defaultCountersEnabled = true;
            int defaultFontSize = 24;
            Color defaultFontColor = Color.white;
            Color defaultBorderColor = Color.black;
            float defaultBorderSize = .5f;
            string defaultPrefix = "";
            string defaultAlignment = "br";
            countersEnabled = configFile.Bind<bool>("counters", "countersEnabled", defaultCountersEnabled, new ConfigDescription("Toggle the visibility of item counters."));
            fontSize = configFile.Bind<int>("counters", "fontSize", defaultFontSize, new ConfigDescription("The font size for item counters."));
            prefix = configFile.Bind<string>("counters", "prefix", defaultPrefix, new ConfigDescription("The text that should prefix the item count (eg. 'x' for 'x3')."));
            string[] acceptableAlignmentValues = { "br", "bl", "tr", "tl", "c" };
            alignment = configFile.Bind<string>("counters", "alignment", defaultAlignment, new ConfigDescription("The alignment of the text on the buttons (bl - bottom left, tr - top right, etc.)", new AcceptableValueList<string>(acceptableAlignmentValues)));
            fontColor = configFile.Bind<Color>("counters", "fontColor", defaultFontColor, new ConfigDescription("The color of the item counter font. This is a hex string in the format of RGBA."));
            borderColor = configFile.Bind<Color>("counters", "borderColor", defaultBorderColor, new ConfigDescription("The border color of the item counter font. This is a hex string in the format of RGBA."));
            borderSize = configFile.Bind<float>("counters", "borderSize", defaultBorderSize, new ConfigDescription("The border width of the item counter font. '.5' is a good default for the default font size."));
            
            // Protection
            bool defaultProtectionEnabled = false;
            string defaultProtectionType = "invisible";
            int defaultProtectionTime = 6;
            int defaultProtectionCooldown = 30;
            int defaultProtectionShieldAmount = 100;
            protectionEnabled = configFile.Bind<bool>("protection", "protectionEnabled", defaultProtectionEnabled, new ConfigDescription("Toggle protection on/off"));
            string[] acceptableProtectionValues = { "invisible", "shield", "invincible" };
            protectionType = configFile.Bind<string>("protection", "protectionType", defaultProtectionType, new ConfigDescription("The type of protection used when a command menu is opened.", new AcceptableValueList<string>(acceptableProtectionValues)));
            protectionTime = configFile.Bind<int>("protection", "protectionTime", defaultProtectionTime, new ConfigDescription("The lengh of time in seconds protection should be enabled for. (Does not apply to `shield` protection type)."));
            protectionCooldown = configFile.Bind<int>("protection", "protectionCooldown", defaultProtectionCooldown, new ConfigDescription("Limit how often protection can be granted"));
            protectionShieldAmount = configFile.Bind<int>("protection", "protectionShieldAmount", defaultProtectionShieldAmount, new ConfigDescription("If protection type is set to `shield` this is how much should be applied. This is a percentage of your health bar, from 1-100."));
        }
    }
}
