using BepInEx;
using BepInEx.Logging;
using ItemStats;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BetterCommandMenu
{
    static class ItemStatsMod
    {
        private static bool _enabled = false;
        internal static bool enabled
        {
            get
            {
                var defaultPair = default(KeyValuePair<string, PluginInfo>);
                var pluginInfo = BepInEx.Bootstrap.Chainloader.PluginInfos.FirstOrDefault(x => x.Key == "dev.ontrigger.itemstats");
                if(!pluginInfo.Equals(defaultPair))
                {
                    _enabled = pluginInfo.Value.Metadata.Version >= Version.Parse("2.0.0");
                    if (!_enabled && SettingsManager.showItemStatsMod.Value)
                        UnityEngine.Debug.LogWarning("You need to have ItemStatsMod v2.0.0 or higher for integration with BCM.", BetterCommandMenu.Instance);
                }
                return _enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static string GetDescription(RoR2.ItemDef itemDef, int itemCount, CharacterMaster master)
        {
            return Language.GetString(itemDef.descriptionToken) + ItemStats.ItemStatsMod.GetStatsForItem(itemDef.itemIndex, itemCount, new StatContext(master));
        }
    }
}
