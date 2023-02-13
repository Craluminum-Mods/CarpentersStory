using CarpentersStory.Util;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace CarpentersStory
{
    public class BlockBehaviorPlacementWedge : BlockBehavior
    {
        JsonItemStack drop;

        public BlockBehaviorPlacementWedge(Block block) : base(block) { }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
            drop = block.GetDrop(properties);
        }

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            drop.ResolveDrop(api, block, ToString());
        }

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling, ref string failureCode)
        {
            handling = EnumHandling.PreventDefault;

            var orientedBlock = block.RotatedWedge(byPlayer, blockSel);

            if (block.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                orientedBlock?.DoPlaceBlock(world, byPlayer, blockSel, itemstack);
                return true;
            }

            return false;
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref float dropChanceMultiplier, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (drop?.ResolvedItemstack != null)
            {
                return new ItemStack[] { drop?.ResolvedItemstack.Clone() };
            }
            return base.GetDrops(world, pos, byPlayer, ref dropChanceMultiplier, ref handling);
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (drop != null)
            {
                return drop?.ResolvedItemstack.Clone();
            }
            return base.OnPickBlock(world, pos, ref handling);
        }
    }
}