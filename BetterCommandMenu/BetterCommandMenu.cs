using BepInEx;
using RoR2;
using RoR2.UI;
using System.Collections.ObjectModel;
using R2API.Utils;
using TMPro;
using UnityEngine;
using BepInEx.Configuration;
using System;
using UnityEngine.EventSystems;
using System.Reflection;
using System.Collections.Generic;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace HoverStats
{
    [BepInPlugin(ModGuid, "BetterCommandMenu", "1.0.1")]
    [BepInProcess("Risk of Rain 2.exe")]
    [BepInDependency("ontrigger-ItemStatsMod-1.5.0", BepInDependency.DependencyFlags.SoftDependency)]
    public class BetterCommandMenu : BaseUnityPlugin
    {
        public const string ModGuid = "org.mries92.BetterCommandMenu";
        ConfigFile configFile_;
        ConfigEntry<bool> tooltipEnabled_, stackTextEnabled_, closeWithEscape_;
        ConfigEntry<string> alignment_, prefix_;
        ConfigEntry<int> fontSize_;
        IntPtr eventUpdatePointer_; 

        void Awake()
        {
            CreateSettingsFile();
            eventUpdatePointer_ = typeof(EventSystem).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic).MethodHandle.GetFunctionPointer();
            On.RoR2.UI.PickupPickerPanel.SetPickupOptions += SetPickupOptions;
        }

        public void CreateSettingsFile()
        {
            int fontSize = 24;
            string prefix = "";
            bool tooltipEnabled = true;
            bool stackTextEnabled = true;
            bool closeWithEscape = true;
            string alignment = "br";

            string[] acceptableSizeValues = { "s", "m", "l" };
            string[] acceptableAlignmentValues = { "br", "bl", "tr", "tl", "c" };

            configFile_ = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);
            fontSize_ = configFile_.Bind<int>("main", "fontSize", fontSize, new ConfigDescription("How big should the text be"));
            prefix_ = configFile_.Bind<string>("main", "prefix", prefix, new ConfigDescription("The text that should prefix the item count (eg. 'x' for 'x3')"));
            tooltipEnabled_ = configFile_.Bind<bool>("main", "tooltipEnabled", tooltipEnabled, new ConfigDescription("Should tooltips be enabled in the command menu"));
            stackTextEnabled_ = configFile_.Bind<bool>("main", "stackTextEnabled", stackTextEnabled, new ConfigDescription("Should item counters be shown in the command menu"));
            closeWithEscape_ = configFile_.Bind<bool>("main", "closeWithEscape", closeWithEscape, new ConfigDescription("Should escape close the command menu (instead of pausing)"));
            alignment_ = configFile_.Bind<string>("main", "alignment", alignment, new ConfigDescription("Where should the text be positioned on the buttons (bl - bottom left, tr - top right, etc.)", new AcceptableValueList<string>(acceptableAlignmentValues)));
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
                var def = PickupCatalog.GetPickupDef(options[i].pickupIndex);
                var idef = ItemCatalog.GetItemDef(def.itemIndex);
                var edef = EquipmentCatalog.GetEquipmentDef(def.equipmentIndex);
                int count = 0;
                
                // Stack text
                if(stackTextEnabled_.Value)
                {
                    GameObject anchorObject = new GameObject("CommandCounter" + i);
                    anchorObject.transform.parent = button.transform;
                    anchorObject.AddComponent<CanvasRenderer>();
                    RectTransform rectTransform = anchorObject.AddComponent<RectTransform>();
                    HGTextMeshProUGUI text = anchorObject.AddComponent<HGTextMeshProUGUI>();

                    text.enableWordWrapping = false;
                    text.text = "";
                    text.color = Color.white;
                    rectTransform.localPosition = Vector2.zero;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.localScale = Vector3.one;
                    rectTransform.sizeDelta = Vector2.zero;

                    // Item count text
                    text.fontSize = fontSize_.Value;
                    if (idef != null)
                    {
                        count = inv.GetItemCount(def.itemIndex);
                        text.text = String.Format("<size={0}>{1}</size>{2}", text.fontSize / 2, prefix_.Value, count);
                    }

                    // Item count alignment
                    switch (alignment_.Value)
                    {
                        case "br":
                            text.alignment = TextAlignmentOptions.BottomRight;
                            rectTransform.anchoredPosition = new Vector2(-5, 2);
                            break;
                        case "bl":
                            text.alignment = TextAlignmentOptions.BottomLeft;
                            rectTransform.anchoredPosition = new Vector2(5, 2);
                            break;
                        case "tr":
                            rectTransform.anchoredPosition = new Vector2(-5, 0);
                            text.alignment = TextAlignmentOptions.TopRight;
                            break;
                        case "tl":
                            rectTransform.anchoredPosition = new Vector2(5, 0);
                            text.alignment = TextAlignmentOptions.TopLeft;
                            break;
                        case "c":
                            rectTransform.anchoredPosition = new Vector2(0, 0);
                            text.alignment = TextAlignmentOptions.Center;
                            break;
                        default:
                            text.alignment = TextAlignmentOptions.BottomRight;
                            rectTransform.anchoredPosition = new Vector2(-5, 2);
                            break;
                    }
                }
                
                
                // Tooltips
                if (tooltipEnabled_.Value)
                {
                    TooltipContent content = new TooltipContent();
                    if (idef != null)
                    {
                        content.titleColor = def.darkColor;
                        content.titleToken = idef.nameToken;
                        if (ItemStatsMod.enabled)
                            content.overrideBodyText = ItemStatsMod.GetDescription(idef, count);
                        else
                            content.bodyToken = idef.descriptionToken;
                    }
                    else
                    {
                        content.titleColor = def.darkColor;
                        content.titleToken = edef.nameToken;
                        content.bodyToken = edef.descriptionToken;
                    }
                    button.gameObject.AddComponent<TooltipProvider>().SetContent(content);
                }
            }
        }
    }
}
