using CarpentersStory.Utils;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

[assembly: ModInfo("Carpenter's Story",
    Authors = new[] { "Craluminum2413" })]

namespace CarpentersStory
{
    class CarpentersStory : ModSystem
    {
        public ModesData toolModes;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockBehaviorClass("CarpentersStory_PlacementWedge", typeof(BlockBehaviorPlacementWedge));
            api.RegisterBlockBehaviorClass("CarpentersStory_PlacementPrism", typeof(BlockBehaviorPlacementPrism));
            api.RegisterBlockBehaviorClass("CarpentersStory_PlacementQuarterA", typeof(BlockBehaviorPlacementQuarterA));
            api.RegisterBlockBehaviorClass("CarpentersStory_PlacementQuarterB", typeof(BlockBehaviorPlacementQuarterB));
            api.RegisterCollectibleBehaviorClass("CarpentersStory_ToolModes", typeof(CollectibleBehaviorToolModes));
            api.RegisterBlockClass("CarpentersStory_BlockFramed", typeof(BlockFramed));
            api.RegisterBlockEntityClass("CarpentersStory_BlockEntityFramed", typeof(BlockEntityFramed));
            api.World.Logger.Event("started \"Carpenter's Story\" mod");
        }

        public override void AssetsLoaded(ICoreAPI api)
        {
            toolModes = LoadObjectFromJson<ModesData>(api, "carps:config/toolmodes.json");
        }

        private T LoadObjectFromJson<T>(ICoreAPI api, string path)
        {
            var jsonObject = new JsonObject(api.Assets.Get<JToken>(new AssetLocation(path)));
            return jsonObject.Token.ToObject<T>();
        }
    }
}