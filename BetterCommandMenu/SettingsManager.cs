﻿using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace BetterCommandMenu
{
    class SettingsManager
    {
        public static string modGuid = "org.mries92.BetterCommandMenu";
        public static ConfigFile configFile;

        // UI
        public static ConfigEntry<bool> disableBlur, disableBackground, disableSpinners, disableColoredOverlay, disableCancelButton, disableLabel;
        public static ConfigEntry<float> menuXOffset, menuYOffset;
        public static ConfigEntry<Color> buttonBorderColor, buttonHoverBorderColor, buttonColor, buttonHoverColor, buttonDisabledColor, buttonPressedColor;
        // Tooltips
        public static ConfigEntry<bool> tooltipEnabled, showItemStatsMod;
        // Escape
        public static ConfigEntry<bool> closeWithEscape;
        // Counters
        public static ConfigEntry<bool> countersEnabled, showEmptyStacks;
        public static ConfigEntry<string> alignment, prefix;
        public static ConfigEntry<int> fontSize;
        public static ConfigEntry<float> borderSize;
        public static ConfigEntry<Color> fontColor, borderColor;
        public static ConfigEntry<float> counterXOffset, counterYOffset;
        
        private static Color NormalizeRGBA(int r, int g, int b, int a)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

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
            Color defaultButtonBorderColor = NormalizeRGBA(255, 255, 255, 73);
            Color defaultButtonHoverBorderColor = NormalizeRGBA(255, 255, 255, 255);
            Color defaultButtonColor = NormalizeRGBA(36, 27, 28, 255);
            Color defaultButtonHoverColor = NormalizeRGBA(115, 51, 51, 255);
            Color defaultButtonDisabledColor = NormalizeRGBA(19, 16, 16, 255);
            Color defaultButtonPressedColor = NormalizeRGBA(46, 49, 51, 251);
            float defaultMenuXOffset = 0, defaultMenuYOffset = 0;
            disableBlur = configFile.Bind<bool>("ui", "disableBlur", defaultDisableBlur, new ConfigDescription("Disable the blur behind the command menu. Allows you to see your current health and buffs."));
            disableBackground = configFile.Bind<bool>("ui", "disableBackground", defaultDisableBackground, new ConfigDescription("Disable the background behind the command menu."));
            disableSpinners = configFile.Bind<bool>("ui", "disableSpinners", defaultDisableSpinners, new ConfigDescription("Disable the spinning decorations around the command menu."));
            menuXOffset = configFile.Bind<float>("ui", "menuXOffset", defaultMenuXOffset, new ConfigDescription("Allows you to move the command menus location (does not apply to the scrapper menu). This is the X offset."));
            menuYOffset = configFile.Bind<float>("ui", "menuYOffset", defaultMenuYOffset, new ConfigDescription("Allows you to move the command menus location (does not apply to the scrapper menu). This is the Y offset."));
            disableColoredOverlay = configFile.Bind<bool>("ui", "disableColoredOverlay", defaultDisableColoredOverlay, new ConfigDescription("Disables the coloration on the command window representing the rarity. You probably want to remove this as well if you are removing the background."));
            disableCancelButton = configFile.Bind<bool>("ui", "disableCancelButton", defaultDisableCancelButton, new ConfigDescription("Removes the cancel button on command windows. Careful with this one, you defnitely want to have `closeWithEscape` enabled or you won't be able to leave a command menu without picking an item. Will probably check for this in the future.)"));
            disableLabel = configFile.Bind<bool>("ui", "disableLabel", defaultDisableLabel, new ConfigDescription("Hide the text at the top of the command and scrapper menus."));
            buttonBorderColor = configFile.Bind<Color>("ui", "buttonBorderColor", defaultButtonBorderColor, new ConfigDescription("The color of the button borders in the command menu."));
            buttonHoverBorderColor = configFile.Bind<Color>("ui", "buttonHoverBorderColor", defaultButtonHoverBorderColor, new ConfigDescription("The color of the button borders in the command menu when you hover over them."));
            buttonColor = configFile.Bind<Color>("ui", "buttonColor", defaultButtonColor, new ConfigDescription("The color of the button backgrounds in the command menu."));
            buttonHoverColor = configFile.Bind<Color>("ui", "buttonHoverColor", defaultButtonHoverColor, new ConfigDescription("The color of buttons you hover over in the command menu."));
            buttonDisabledColor = configFile.Bind<Color>("ui", "buttonDisabledColor", defaultButtonDisabledColor, new ConfigDescription("The color of the button backgrounds for items you do not have unlocked."));
            buttonPressedColor = configFile.Bind<Color>("ui", "buttonPressedColor", defaultButtonPressedColor, new ConfigDescription("The color of the button backgrounds when you click an entry."));


            // Tooltips
            bool defaultTooltipEnabled = true;
            bool defaultShowItemStatsMod = true;
            tooltipEnabled = configFile.Bind<bool>("tooltips", "tooltipEnabled", defaultTooltipEnabled, new ConfigDescription("Toggle tooltips on/off"));
            showItemStatsMod = configFile.Bind<bool>("tooltips", "showItemStatsMod", defaultShowItemStatsMod, new ConfigDescription("Toggle visibility of extra information from ItemStatsMod"));
            
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
        }
    }
}
