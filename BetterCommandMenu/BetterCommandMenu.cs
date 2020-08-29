﻿using BepInEx;
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
using MonoMod.Utils;
using MonoMod.Utils.Cil;
using BetterCommandMenu;

namespace HoverStats
{
    [BepInPlugin(modGuid, "BetterCommandMenu", "1.0.1")]
    [BepInProcess("Risk of Rain 2.exe")]
    [BepInDependency("ontrigger-ItemStatsMod-1.5.0", BepInDependency.DependencyFlags.SoftDependency)]
    public class BetterCommandMenu : BaseUnityPlugin
    {
        public const string modGuid = "org.mries92.BetterCommandMenu";
        void Awake()
        {
            SettingsManager.Init();
            On.RoR2.UI.PickupPickerPanel.SetPickupOptions += SetPickupOptions;
            IL.RoR2.UI.MPEventSystem.Update += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(x => x.MatchCall<RoR2.Console>("get_instance"));
                var label = c.DefineLabel();
                c.MarkLabel(label);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<MPEventSystem>>((eventSystem) =>
                {
                    if (SettingsManager.closeWithEscape.Value == true)
                    {
                        PickupPickerPanel[] panels = (PickupPickerPanel[])FindObjectsOfType(typeof(PickupPickerPanel));
                        if (panels.Length > 0)
                            foreach (var panel in panels)
                                Destroy(panel.gameObject);
                        else
                            RoR2.Console.instance.SubmitCmd(null, "pause");
                    }
                    else
                        RoR2.Console.instance.SubmitCmd(null, "pause");
                });
                c.Emit(OpCodes.Ret);
                c.GotoPrev(x => x.MatchBrfalse(out _));
                c.Next.Operand = label;
                c.Next.OpCode = OpCodes.Brfalse;
            };
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
                if(SettingsManager.stackTextEnabled.Value)
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
                    text.fontSize = SettingsManager.fontSize.Value;
                    text.color = SettingsManager.fontColor.Value;
                    text.outlineColor = SettingsManager.borderColor.Value;
                    text.outlineWidth = SettingsManager.borderSize.Value;
                    if (idef != null)
                    {
                        count = inv.GetItemCount(def.itemIndex);
                        text.text = String.Format("<size={0}>{1}</size>{2}", text.fontSize / 2, SettingsManager.prefix.Value, count);
                    }

                    // Item count alignment
                    switch (SettingsManager.alignment.Value)
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
                if (SettingsManager.tooltipEnabled.Value)
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
