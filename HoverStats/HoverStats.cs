using BepInEx;
using RoR2;
using RoR2.UI;
using System.Collections.ObjectModel;
using R2API.Utils;

namespace HoverStats
{
    [BepInPlugin("org.mries.hoverstats", "HoverStats", "0.0.1")]
    [BepInProcess("Risk of Rain 2.exe")]
    public class HoverStats : BaseUnityPlugin
    {
        void Awake()
        {
            On.RoR2.UI.PickupPickerPanel.SetPickupOptions += SetPickupOptions;
        }

        public void SetPickupOptions(On.RoR2.UI.PickupPickerPanel.orig_SetPickupOptions orig, RoR2.UI.PickupPickerPanel self, RoR2.PickupPickerController.Option[] options)
        {
            orig(self, options);
            var allocator = self.GetFieldValue<UIElementAllocator<MPButton>>("buttonAllocator");
            ReadOnlyCollection<MPButton> buttons = allocator.elements;
            TooltipProvider provider = new TooltipProvider();

            int i = 0;
            foreach (var button in buttons)
            {
                TooltipContent content = new TooltipContent();
                var def = PickupCatalog.GetPickupDef(options[i].pickupIndex);
                var idef = ItemCatalog.GetItemDef(def.itemIndex);
                var edef = EquipmentCatalog.GetEquipmentDef(def.equipmentIndex);
                if (idef != null)
                {
                    content.titleColor = def.darkColor;
                    content.titleToken = idef.nameToken;
                    content.bodyToken = idef.descriptionToken;
                }
                else
                {
                    content.titleColor = def.darkColor;
                    content.titleToken = edef.nameToken;
                    content.bodyToken = edef.descriptionToken;
                }

                button.gameObject.AddComponent<TooltipProvider>().SetContent(content);
                i++;
            }
        }
    }
}
