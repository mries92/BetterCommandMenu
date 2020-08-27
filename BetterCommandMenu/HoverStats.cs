using BepInEx;
using RoR2;
using RoR2.UI;
using System.Collections.ObjectModel;
using R2API.Utils;
using TMPro;
using UnityEngine;
using BepInEx.Configuration;

namespace HoverStats
{
    [BepInPlugin(ModGuid, "BetterCommandMenu", "1.0.0")]
    [BepInProcess("Risk of Rain 2.exe")]
    [BepInDependency("ontrigger-ItemStatsMod-1.5.0", BepInDependency.DependencyFlags.SoftDependency)]
    public class BetterCommandMenu : BaseUnityPlugin
    {
        public const string ModGuid = "org.mries92.BetterCommandMenu";
        ConfigFile configFile;

        ConfigEntry<bool> tooltipEnabled_, stackTextEnabled_;
        ConfigEntry<string> fontSize_, alignment_;

        void Awake()
        {
            On.RoR2.UI.PickupPickerPanel.SetPickupOptions += SetPickupOptions;
            // Create config
            CreateSettingsFile();
        }

        public void CreateSettingsFile()
        {
            string fontSize = "m";
            bool tooltipEnabled = true;
            bool stackTextEnabled = true;
            string alignment = "br";

            string[] acceptableSizeValues = { "s", "m", "l" };
            string[] acceptableAlignmentValues = { "br", "bl", "tr", "tl", "c" };

            configFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);
            AcceptableValueList<string> acceptableValueList = new AcceptableValueList<string>(acceptableSizeValues);
            ConfigDescription description = new ConfigDescription("How big should the text be", acceptableValueList);
            fontSize_ = configFile.Bind<string>("main", "fontSize", fontSize, description);
            tooltipEnabled_ = configFile.Bind<bool>("main", "tooltipEnabled", tooltipEnabled);
            stackTextEnabled_ = configFile.Bind<bool>("main", "stackTextEnabled", stackTextEnabled);
            acceptableValueList = new AcceptableValueList<string>(acceptableAlignmentValues);
            description = new ConfigDescription("Where should the text be positioned on the buttons", acceptableValueList);
            alignment_ = configFile.Bind<string>("main", "alignment", alignment, description);
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
                int count = inv.GetItemCount(def.itemIndex);

                // Add the item count text
                GameObject gameObject = new GameObject("CommandCounter" + i);
                gameObject.transform.parent = button.transform;
                gameObject.AddComponent<CanvasRenderer>();

                RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
                HGTextMeshProUGUI text = gameObject.AddComponent<HGTextMeshProUGUI>();

                text.enableWordWrapping = false;
                rectTransform.localPosition = Vector2.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.localScale = Vector3.one;
                rectTransform.sizeDelta = Vector2.zero;

                text.text = count.ToString();
                switch(fontSize_.Value)
                {
                    case "s":
                        text.fontSize = 16;
                        break;
                    case "m":
                        text.fontSize = 24;
                        break;
                    case "l":
                        text.fontSize = 32;
                        break;
                    default:
                        break;
                }
                text.color = Color.white;
                switch(alignment_.Value)
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
                        text.text = count.ToString();
                    }
                    else
                    {
                        content.titleColor = def.darkColor;
                        content.titleToken = edef.nameToken;
                        content.bodyToken = edef.descriptionToken;
                        text.enabled = false;
                    }
                    button.gameObject.AddComponent<TooltipProvider>().SetContent(content);
                }
            }
        }
    }
}
