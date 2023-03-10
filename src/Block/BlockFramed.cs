using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.Client.NoObf;
using System.Linq;
using CarpentersStory.Util;
using System;
using Vintagestory.API.Server;
using Vintagestory.API.Config;

namespace CarpentersStory
{
    public class BlockFramed : Block, ITexPositionSource, IContainedMeshSource
    {
        public ICoreClientAPI capi;
        public ITextureAtlasAPI targetAtlas;
        public Size2i AtlasSize => targetAtlas.Size;
        public Dictionary<int, MeshRef> Meshrefs => ObjectCacheUtil.GetOrCreate(api, MeshRefName, () => new Dictionary<int, MeshRef>());
        public TextureAtlasPosition this[string textureCode] => GetOrCreateTexPos(tmpTextures[textureCode]);
        public readonly Dictionary<string, AssetLocation> tmpTextures = new();

        public virtual string MeshRefName => $"carpentersStory_{this}_Meshrefs";

        public WorldInteraction[] interactions;

        protected TextureAtlasPosition GetOrCreateTexPos(AssetLocation texturePath)
        {
            IAsset texAsset = capi.Assets.TryGet(texturePath.Clone().WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png"));
            TextureAtlasPosition texPos = targetAtlas[texturePath];

            if (texPos != null)
            {
                return texPos;
            }

            if (texAsset != null)
            {
                capi.Event.EnqueueMainThreadTask(() => targetAtlas.GetOrInsertTexture(texturePath, out int _, out texPos, () => texAsset.ToBitmap(capi)), "");
            }

            return texPos ?? capi.BlockTextureAtlas.UnknownTexturePosition;
        }

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            capi = api as ICoreClientAPI;

            interactions = ObjectCacheUtil.GetOrCreate(api, this + "BlockInteractions", () =>
            {
                return new WorldInteraction[] {
                    new WorldInteraction()
                    {
                        ActionLangCode = "carps:blockhelp-addBlock",
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCodes = new string[] { "ctrl" }
                    },
                    new WorldInteraction()
                    {
                        ActionLangCode = "carps:blockhelp-removeBlock",
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCodes = new string[] { "ctrl" }
                    }
                };
            });
        }

        public MeshData GenMesh(ItemStack stack, ITextureAtlasAPI targetAtlas, BlockPos atBlockPos)
        {
            if (stack != null)
            {
                capi.Tesselator.TesselateBlock(this, out MeshData mesh);
                return mesh;
            }
            else if (atBlockPos != null)
            {
                if (api.World.BlockAccessor.GetBlockEntity(atBlockPos) is not BlockEntityFramed blockEntity)
                {
                    return null;
                }

                this.targetAtlas = targetAtlas;
                tmpTextures.Clear();

                foreach (KeyValuePair<string, CompositeTexture> key in Textures)
                {
                    tmpTextures[key.Key] = blockEntity.storedBlock?.ResolveBlockOrItem(api.World) == true
                        ? blockEntity.storedBlock.Block.Textures.FirstOrDefault().Value.Base
                        : Textures.FirstOrDefault().Value.Base;
                }

                CompositeShape cshape = Attributes["shape"].AsObject<CompositeShape>();

                MeshData mesh;
                if (cshape.Format == EnumShapeFormat.Obj)
                {
                    TessellatorExtensions.TessellateObj(capi.Tesselator as ShapeTesselator, cshape, out mesh, this);
                }
                else if (cshape.Format == EnumShapeFormat.GltfEmbedded)
                {
                    TessellatorExtensions.TessellateGltf(capi.Tesselator as ShapeTesselator, capi, cshape, out mesh, this);
                }
                else
                {
                    capi.Tesselator.TesselateBlock(this, out mesh);
                }

                return mesh;
            }

            return null;
        }

        public override bool ShouldMergeFace(int facingIndex, Block neighbourblock, int intraChunkIndex3d)
        {
            return this == neighbourblock;
        }

        public string GetMeshCacheKey(ItemStack stack) => Code.ToString();

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            if (world.BlockAccessor.GetBlockEntity(blockSel.Position) is not BlockEntityFramed blockEntity)
            {
                return false;
            }

            return byPlayer.Entity.Controls.CtrlKey
                ? blockEntity.TryPut(byPlayer) || blockEntity.TryTake(byPlayer) || base.OnBlockInteractStart(world, byPlayer, blockSel)
                : base.OnBlockInteractStart(world, byPlayer, blockSel);
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            if (world.BlockAccessor.GetBlockEntity(pos) is BlockEntityFramed blockEntity
                && byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                _ = api.World.SpawnItemEntity(blockEntity.storedBlock, pos.ToVec3d().Add(0.5, 0.5, 0.5));
            }

            base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
        }

        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            return interactions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
        }
    }
}