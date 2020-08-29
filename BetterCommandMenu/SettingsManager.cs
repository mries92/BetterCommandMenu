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
        public static ConfigEntry<bool> tooltipEnabled, stackTextEnabled, closeWithEscape;
        public static ConfigEntry<string> alignment, prefix;
        public static ConfigEntry<int> fontSize;
        public static ConfigEntry<float> borderSize;
        public static ConfigEntry<Color> fontColor, borderColor;

        public static void Init()
        {
            int defaultFontSize = 24;
            string defaultPrefix = "";
            bool defaultTooltipEnabled = true;
            bool defaultStackTextEnabled = true;
            bool defaultCloseWithEscape = true;
            string defaultAlignment = "br";
            Color defaultFontColor = Color.white;
            Color defaultBorderColor = Color.black;
            float defaultBorderSize = .5f;

            configFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, modGuid + ".cfg"), true);
            // Tooltips
            tooltipEnabled = configFile.Bind<bool>("tooltips", "tooltipEnabled", defaultTooltipEnabled, new ConfigDescription("Should tooltips be enabled in the command menu"));
            // Escape
            closeWithEscape = configFile.Bind<bool>("escape", "closeWithEscape", defaultCloseWithEscape, new ConfigDescription("Should escape close the command menu (instead of pausing)"));
            // Counters
            fontSize = configFile.Bind<int>("counters", "fontSize", defaultFontSize, new ConfigDescription("How big should the text be"));
            prefix = configFile.Bind<string>("counters", "prefix", defaultPrefix, new ConfigDescription("The text that should prefix the item count (eg. 'x' for 'x3')"));
            stackTextEnabled = configFile.Bind<bool>("counters", "stackTextEnabled", defaultStackTextEnabled, new ConfigDescription("Should item counters be shown in the command menu"));
            string[] acceptableAlignmentValues = { "br", "bl", "tr", "tl", "c" };
            alignment = configFile.Bind<string>("counters", "alignment", defaultAlignment, new ConfigDescription("Where should the text be positioned on the buttons (bl - bottom left, tr - top right, etc.)", new AcceptableValueList<string>(acceptableAlignmentValues)));
            fontColor = configFile.Bind<Color>("counters", "fontColor", defaultFontColor, new ConfigDescription("What color should the item count font be. This is a hex string in the format of RGBA"));
            borderColor = configFile.Bind<Color>("counters", "borderColor", defaultBorderColor, new ConfigDescription("What color should the item count font borders be. This is a hex string in the format of RGBA"));
            borderSize = configFile.Bind<float>("counters", "borderSize", defaultBorderSize, new ConfigDescription("How wide should the border on the item count font be. '.5' is a good default for the default font size."));
        }
    }
}
