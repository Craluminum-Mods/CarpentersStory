using HarmonyLib;

namespace CarpentersStory.Util
{
    public static class HarmonyExtensions
    {
        public static T GetField<T>(this object instance, string fieldName)
        {
            return (T)AccessTools.Field(instance.GetType(), fieldName).GetValue(instance);
        }
    }
}
