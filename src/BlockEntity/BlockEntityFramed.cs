using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Config;

namespace CarpentersStory
{
    public class BlockEntityFramed : BlockEntity
    {
        public ItemStack storedBlock;
        private ICoreClientAPI capi;

        public string MeshesKey => $"{this}BlockMeshes";

        public string MeshCacheKey
        {
            get
            {
                string strings = Block.Code.ToString();

                if (storedBlock?.ResolveBlockOrItem(Api.World) == true)
                {
                    strings += "-" + storedBlock.Collectible.Code.ToString();
                }

                return strings;
            }
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            capi = api as ICoreClientAPI;
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            storedBlock = tree.GetItemstack("storedBlock");
            base.FromTreeAttributes(tree, worldAccessForResolve);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            tree.SetItemstack("storedBlock", storedBlock);
        }

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            var _storedBlock = storedBlock;
            var storedBlockName = "None";

            if (_storedBlock?.ResolveBlockOrItem(Api.World) == true)
            {
                storedBlockName = _storedBlock?.GetName();
            }

            var format = Lang.Get("carps:format-pastelgreen", storedBlockName);
            dsc.AppendLine().Append(Lang.Get("Contents: {0}", format));

            // dsc.AppendHitPositionDebugText(forPlayer);

            base.GetBlockInfo(forPlayer, dsc);
        }

        private MeshData GetMesh(ITesselatorAPI tesselator)
        {
            var blockMeshes = ObjectCacheUtil.GetOrCreate(Api, MeshesKey, () => new Dictionary<string, MeshData>());
            if (Api.World.BlockAccessor.GetBlock(Pos) is not BlockFramed block) return null;
            if (blockMeshes.TryGetValue(MeshCacheKey, out var mesh)) return mesh;
            return blockMeshes[MeshCacheKey] = block.GenMesh(null, capi.BlockTextureAtlas, Pos);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            var ownMesh = GetMesh(tesselator);
            if (ownMesh == null) return false;

            mesher.AddMeshData(ownMesh);
            return true;
        }

        public bool TryPut(IPlayer byPlayer)
        {
            var fromSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (fromSlot.Empty || fromSlot?.Itemstack?.Collectible.ItemClass == EnumItemClass.Item || storedBlock != null)
            {
                return false;
            }

            switch (byPlayer.WorldData.CurrentGameMode)
            {
                case EnumGameMode.Creative:
                    var fromstack = fromSlot.Itemstack.Clone();
                    fromstack.StackSize = 1;

                    storedBlock = fromstack;
                    break;

                case EnumGameMode.Survival:
                    storedBlock = fromSlot.TakeOut(1);
                    break;
            }

            MarkDirty(true);
            return true;
        }

        public bool TryTake(IPlayer byPlayer)
        {
            if (storedBlock == null) return false;
            if (storedBlock?.ResolveBlockOrItem(Api.World) == false) return false;

            if (!byPlayer.InventoryManager.TryGiveItemstack(storedBlock, true))
            {
                Api.World.SpawnItemEntity(storedBlock, byPlayer.Entity.BlockSelection.Position.ToVec3d().Add(0.5, 0.5, 0.5));
            }

            storedBlock = null;

            MarkDirty(true);
            return true;
        }
    }
}