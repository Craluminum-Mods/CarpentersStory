using System.Collections.Generic;
using CarpentersStory.Utils;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace CarpentersStory
{
    class CollectibleBehaviorToolModes : CollectibleBehavior
    {
        public ICoreAPI api;
        public string groupName;
        public List<ToolMode> toolModes;
        public SkillItem[] skillItems;

        public CollectibleBehaviorToolModes(CollectibleObject collObj) : base(collObj) { }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
            groupName = properties["groupName"].AsString();
        }

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            this.api = api;

            toolModes = api.GetModesFromGroup(groupName);
            skillItems = api.GetToolModes(collObj);
        }

        public override void OnUnloaded(ICoreAPI api)
        {
            for (int i = 0; skillItems != null && i < skillItems.Length; i++)
            {
                skillItems[i]?.Dispose();
            }
        }

        public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSelection, int toolMode)
        {
            slot.ToBlockCode(byPlayer.Entity.World.Api, toolMode, toolModes);
            slot.MarkDirty();
        }

        public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel) => skillItems;

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot, ref EnumHandling handling)
        {
            if (skillItems == null) return base.GetHeldInteractionHelp(inSlot, ref handling);

            return base.GetHeldInteractionHelp(inSlot, ref handling).Append(new WorldInteraction
            {
                ActionLangCode = "carps:heldhelp-selectshape",
                HotKeyCode = "toolmodeselect",
                MouseButton = EnumMouseButton.None
            });
        }
    }
}