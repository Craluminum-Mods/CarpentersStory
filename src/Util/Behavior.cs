using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarpentersStory.Util
{
    public static class Behavior
    {
        public static JsonItemStack GetDrop(this Block block, JsonObject properties)
        {
            return properties["drop"].Exists
                ? properties["drop"].AsObject<JsonItemStack>(null, block.Code.Domain)
                : null;
        }

        public static void ResolveDrop(this JsonItemStack drop, ICoreAPI api, Block block, string behaviorName)
        {
            drop?.Resolve(api.World, behaviorName + " drop for " + block.Code);
        }
    }
}