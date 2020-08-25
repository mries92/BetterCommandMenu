using BepInEx;
using RoR2;
using RoR2.UI;
using System.Collections.ObjectModel;
using R2API.Utils;
using TMPro;
using UnityEngine;

namespace HoverStats
{
    [BepInPlugin("org.mries.hoverstats", "HoverStats", "1.0.0")]
    [BepInProcess("Risk of Rain 2.exe")]
    public class HoverStats : BaseUnityPlugin
    {
        AssetBundle bundle;
        void Awake()
        {
            On.RoR2.UI.PickupPickerPanel.SetPickupOptions += SetPickupOptions;
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.bundle);
        }

        public void SetPickupOptions(On.RoR2.UI.PickupPickerPanel.orig_SetPickupOptions orig, RoR2.UI.PickupPickerPanel self, RoR2.PickupPickerController.Option[] options)
        {
            orig(self, options);
            var allocator = self.GetFieldValue<UIElementAllocator<MPButton>>("buttonAllocator");
            ReadOnlyCollection<MPButton> buttons = allocator.elements;
            LocalUser user = LocalUserManager.GetFirstLocalUser();
            CharacterBody body = user.cachedBody;
            Inventory inv = body.inventory;
            for(int i = 0; i < buttons.Count; i++)
            {
                MPButton button = buttons[i];

                TooltipContent content = new TooltipContent();
                var text_object = Instantiate(bundle.LoadAsset<GameObject>("ItemCountText"), button.transform);
                var mesh = text_object.GetComponent<TextMeshProUGUI>();
                text_object.GetComponent<RectTransform>().localPosition += new Vector3(20, -25);

                var def = PickupCatalog.GetPickupDef(options[i].pickupIndex);
                var idef = ItemCatalog.GetItemDef(def.itemIndex);
                var edef = EquipmentCatalog.GetEquipmentDef(def.equipmentIndex);
                if (idef != null)
                {
                    content.titleColor = def.darkColor;
                    content.titleToken = idef.nameToken;
                    content.bodyToken = idef.descriptionToken;
                    mesh.SetText(inv.GetItemCount(def.itemIndex).ToString());
                }
                else
                {
                    content.titleColor = def.darkColor;
                    content.titleToken = edef.nameToken;
                    content.bodyToken = edef.descriptionToken;
                    mesh.SetText("");
                }
                button.gameObject.AddComponent<TooltipProvider>().SetContent(content);
            }
        }
    }
}
