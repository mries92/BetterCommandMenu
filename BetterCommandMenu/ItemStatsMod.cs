using ItemStats;
using RoR2;
using System.Runtime.CompilerServices;

namespace BetterCommandMenu
{
    static class ItemStatsMod
    {
        private static bool? _enabled;
        internal static bool enabled
        {
            get
            {
                if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.ontrigger.itemstats");
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static string GetDescription(RoR2.ItemDef itemDef, int itemCount)
        {
            return Language.GetString(itemDef.descriptionToken) + "\n\n" + ItemStatProvider.ProvideStatsForItem(itemDef.itemIndex, itemCount);
        }
    }
}
