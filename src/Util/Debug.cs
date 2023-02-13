using System;
using System.Text;
using Vintagestory.API.Common;

namespace CarpentersStory.Util
{
    public static class Debug
    {
        public static StringBuilder AppendHitPositionDebugText(this StringBuilder dsc, IPlayer forPlayer)
        {
            var xyzint = string.Format(
                "X: {0}, Y: {1}, Z: {2}",
                Math.Round(forPlayer.CurrentBlockSelection.HitPosition.X, 2),
                Math.Round(forPlayer.CurrentBlockSelection.HitPosition.Y, 2),
                Math.Round(forPlayer.CurrentBlockSelection.HitPosition.Z, 2));

            return dsc.AppendLine().Append(xyzint);
        }
    }
}