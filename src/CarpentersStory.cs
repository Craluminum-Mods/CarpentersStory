using Vintagestory.API.Common;

[assembly: ModInfo("Carpenter's Story",
    Authors = new[] { "Craluminum2413" })]

namespace CarpentersStory
{
    class CarpentersStory : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockBehaviorClass("CarpentersStory_PlacementPrism", typeof(BlockBehaviorPlacementPrism));
            api.RegisterBlockBehaviorClass("CarpentersStory_PlacementQuarterA", typeof(BlockBehaviorPlacementQuarterA));
            api.RegisterBlockBehaviorClass("CarpentersStory_PlacementQuarterB", typeof(BlockBehaviorPlacementQuarterB));
            api.RegisterBlockClass("CarpentersStory_BlockFramed", typeof(BlockFramed));
            api.RegisterBlockEntityClass("CarpentersStory_BlockEntityFramed", typeof(BlockEntityFramed));
            api.World.Logger.Event("started \"Carpenter's Story\" mod");
        }
    }
}