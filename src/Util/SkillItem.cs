using System.Collections.Generic;
using Newtonsoft.Json;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace CarpentersStory.Utils
{
    [JsonObject]
    public class ModesData
    {
        public Dictionary<string, List<ToolMode>> Modes;
    }

    [JsonObject]
    public class ToolMode
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RenderFromCode { get; set; }
        public string ToBlockCode { get; set; }
        public bool Linebreak { get; set; }
    }

    public static class ToolModes
    {
        public static RenderSkillItemDelegate RenderItemStack(this ItemStack stack, ICoreClientAPI capi)
        {
            return (AssetLocation code, float dt, double posX, double posY) =>
            {
                var size = GuiElementPassiveItemSlot.unscaledSlotSize + GuiElementItemSlotGridBase.unscaledSlotPadding;
                var scsize = GuiElement.scaled(size - 5);

                capi?.Render.RenderItemstackToGui(
                    new DummySlot(stack),
                    posX + (scsize / 2),
                    posY + (scsize / 2),
                    100,
                    (float)GuiElement.scaled(GuiElementPassiveItemSlot.unscaledItemSize),
                    ColorUtil.WhiteArgb,
                    showStackSize: true);
            };
        }

        public static List<ToolMode> GetModesFromGroup(this ICoreAPI api, string groupName)
        {
            var groups = api.ModLoader.GetModSystem<CarpentersStory>().toolModes;

            return groups.Modes.ContainsKey(groupName) ? groups.Modes[groupName] : null;
        }

        public static SkillItem[] GetToolModes(this ICoreAPI api, CollectibleObject collobj)
        {
            var customModes = collobj.GetBehavior<CollectibleBehaviorToolModes>()?.toolModes;
            var modes = new List<SkillItem>();

            for (int i = 0; i < customModes.Count; i++)
            {
                var mode = customModes[i];

                modes.Add(
                    new SkillItem
                    {
                        Name = Lang.GetMatching(mode?.Name ?? ""),
                        Description = Lang.GetMatching(mode?.Description ?? ""),
                        Linebreak = mode.Linebreak
                    }
                );

                if (api.Side.IsServer()) continue;
                var capi = (ICoreClientAPI)api;

                if (!string.IsNullOrEmpty(mode.RenderFromCode))
                {
                    var stack = new ItemStack(api.World.GetBlock(new AssetLocation(mode.RenderFromCode)));
                    modes[i].RenderHandler = stack.RenderItemStack(capi);
                }
            }

            return modes.ToArray();
        }

        public static void ToBlockCode(this ItemSlot slot, ICoreAPI api, int index, List<ToolMode> modes)
        {
            var blockCode = modes[index].ToBlockCode;

            if (string.IsNullOrEmpty(blockCode)) return;

            var newBlock = api.World.GetBlock(new AssetLocation(blockCode));

            if (newBlock == null) return;

            slot.Itemstack.SetFrom(new ItemStack(block: newBlock, stacksize: slot.Itemstack.StackSize));
        }
    }
}
