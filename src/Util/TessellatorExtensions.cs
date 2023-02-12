using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.Client.NoObf;

namespace CarpentersStory.Util
{
    public static class TessellatorExtensions
    {
        public static void TessellateObj(this ShapeTesselator tessellator, CompositeShape compositeShape, out MeshData modeldata, ITexPositionSource texSource, int generalGlowLevel = 0, byte climateColorMapIndex = 0, byte seasonColorMapIndex = 0)
        {
            var meta = tessellator.GetField<TesselationMetaData>("meta");
            var objTesselator = tessellator.GetField<ObjTesselator>("objTesselator");
            var objs = tessellator.GetField<OrderedDictionary<AssetLocation, IAsset>>("objs");

            meta.usesColorMap = false;
            objTesselator.Load(objs[compositeShape.Base], out modeldata, texSource["obj"], generalGlowLevel, climateColorMapIndex, seasonColorMapIndex, -1);
            tessellator.ApplyCompositeShapeModifiers(ref modeldata, compositeShape);
        }

        public static void TessellateGltf(this ShapeTesselator tessellator, ICoreClientAPI capi, CompositeShape compositeShape, out MeshData modeldata, ITexPositionSource texSource, int generalGlowLevel = 0, byte climateColorMapIndex = 0, byte seasonColorMapIndex = 0)
        {
            var meta = tessellator.GetField<TesselationMetaData>("meta");
            var gltfTesselator = tessellator.GetField<GltfTesselator>("gltfTesselator");
            var gltfs = tessellator.GetField<OrderedDictionary<AssetLocation, GltfType>>("gltfs");

            meta.usesColorMap = false;
            var texPos = (texSource["gltf"] == capi.BlockTextureAtlas[new AssetLocation("unknown")]) ? null : texSource["gltf"];
            gltfTesselator.Load(gltfs[compositeShape.Base], out modeldata, texPos, generalGlowLevel, climateColorMapIndex, seasonColorMapIndex, -1, out var bakedTextures);
            if (compositeShape.InsertBakedTextures)
            {
                gltfs[compositeShape.Base].BaseTextures = new TextureAtlasPosition[bakedTextures.Length];
                gltfs[compositeShape.Base].PBRTextures = new TextureAtlasPosition[bakedTextures.Length];
                gltfs[compositeShape.Base].NormalTextures = new TextureAtlasPosition[bakedTextures.Length];
                for (var i = 0; i < bakedTextures.Length; i++)
                {
                    var bytes = bakedTextures[i];
                    if (bytes[0] != null)
                    {
                        if (!capi.BlockTextureAtlas.InsertTexture(bytes[0], out _, out var position))
                        {
                            capi.Logger.Debug("Failed adding baked in gltf base texture to atlas from: {0}, texture probably too large.", compositeShape.Base);
                            gltfs[compositeShape.Base].BaseTextures[i] = capi.BlockTextureAtlas[new AssetLocation("unknown")];
                        }
                        else
                        {
                            gltfs[compositeShape.Base].BaseTextures[i] = position;
                            if (texPos == null)
                            {
                                modeldata.SetTexPos(position);
                            }
                        }
                    }
                    if (bytes[1] != null)
                    {
                        if (!capi.BlockTextureAtlas.InsertTexture(bytes[1], out _, out var position2))
                        {
                            capi.Logger.Debug("Failed adding baked in gltf pbr texture to atlas from: {0}, texture probably too large.", compositeShape.Base);
                        }
                        else
                        {
                            gltfs[compositeShape.Base].PBRTextures[i] = position2;
                        }
                    }

                    if (bytes[2] == null) continue;
                    if (!capi.BlockTextureAtlas.InsertTexture(bytes[2], out _, out var position3))
                    {
                        capi.Logger.Debug("Failed adding baked in gltf normal texture to atlas from: {0}, texture probably too large.", compositeShape.Base);
                    }
                    else
                    {
                        gltfs[compositeShape.Base].NormalTextures[i] = position3;
                    }
                }
            }
            tessellator.ApplyCompositeShapeModifiers(ref modeldata, compositeShape);
        }
    }
}
