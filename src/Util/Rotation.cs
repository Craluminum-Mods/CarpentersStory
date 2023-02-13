using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CarpentersStory.Util
{
    public static class Rotation
    {
        public static BlockFacing[] SuggestedHVOrientation(this BlockSelection blockSel, IPlayer byPlayer)
        {
            BlockPos targetPos = blockSel.DidOffset ? blockSel.Position.AddCopy(blockSel.Face.Opposite) : blockSel.Position;

            double dx = byPlayer.Entity.Pos.X + byPlayer.Entity.LocalEyePos.X - (targetPos.X + blockSel.HitPosition.X);
            double dy = byPlayer.Entity.Pos.Y + byPlayer.Entity.LocalEyePos.Y - (targetPos.Y + blockSel.HitPosition.Y);
            double dz = byPlayer.Entity.Pos.Z + byPlayer.Entity.LocalEyePos.Z - (targetPos.Z + blockSel.HitPosition.Z);

            float angleHor = (float)Math.Atan2(dx, dz) + GameMath.PIHALF;

            double a = dy;
            float b = (float)Math.Sqrt((dx * dx) + (dz * dz));

            float angleVer = (float)Math.Atan2(a, b);

            BlockFacing verticalFace = angleVer < -Math.PI / 4 ? BlockFacing.DOWN : (angleVer > Math.PI / 4 ? BlockFacing.UP : null);
            BlockFacing horizontalFace = BlockFacing.HorizontalFromAngle(angleHor);

            return new BlockFacing[] { horizontalFace, verticalFace };
        }

        public static Block RotatedByQuarterA(this Block block, IPlayer byPlayer, BlockSelection blockSel)
        {
            var horVer = new List<BlockFacing>() { null, null };
            var x = blockSel.HitPosition.X;
            var y = blockSel.HitPosition.Y;
            var z = blockSel.HitPosition.Z;

            if (blockSel.Face.IsHorizontal)
            {
                horVer[1] = y <= 0.5 ? BlockFacing.DOWN : BlockFacing.UP;

                if (blockSel.Face == BlockFacing.NORTH)
                    horVer[0] = x >= 0.5 ? blockSel.Face.Opposite : blockSel.Face.Opposite.GetCW();
                else if (blockSel.Face == BlockFacing.SOUTH)
                    horVer[0] = x >= 0.5 ? blockSel.Face.Opposite.GetCW() : blockSel.Face.Opposite;
                else if (blockSel.Face == BlockFacing.EAST)
                    horVer[0] = z >= 0.5 ? blockSel.Face.Opposite : blockSel.Face.Opposite.GetCW();
                else if (blockSel.Face == BlockFacing.WEST)
                    horVer[0] = z >= 0.5 ? blockSel.Face.Opposite.GetCW() : blockSel.Face.Opposite;
            }
            else if (blockSel.Face.IsVertical)
            {
                horVer[1] = y >= 0.5 ? BlockFacing.DOWN : BlockFacing.UP;

                if (x <= 0.5 && z <= 0.5)
                    horVer[0] = BlockFacing.NORTH;
                else if (x >= 0.5 && z >= 0.5)
                    horVer[0] = BlockFacing.SOUTH;
                else if (x > 0.5 && z < 0.5)
                    horVer[0] = BlockFacing.EAST;
                else if (x < 0.5 && z > 0.5)
                    horVer[0] = BlockFacing.WEST;
            }

            var variants = new string[] { horVer[1].Code, horVer[0].Code };
            var blockCode = block.CodeWithVariants(new string[] { "ver", "hor" }, variants);

            return byPlayer.Entity.World.GetBlock(blockCode);
        }

        public static Block RotatedByQuarterB(this Block block, IPlayer byPlayer, BlockSelection blockSel)
        {
            var orientations = new List<string>() { null, null };

            var x = blockSel.HitPosition.X;
            var y = blockSel.HitPosition.Y;
            var z = blockSel.HitPosition.Z;

            switch (blockSel.Face.Axis)
            {
                case EnumAxis.X:
                    if (z < 0.5 && y < 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "west")
                        {
                            orientations[0] = "center-down-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "east")
                        {
                            orientations[0] = "center-down-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    else if (z > 0.5 && y > 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "west")
                        {
                            orientations[0] = "center-up-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "east")
                        {
                            orientations[0] = "center-up-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    else if (z > 0.5 && y < 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "west")
                        {
                            orientations[0] = "center-down-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "east")
                        {
                            orientations[0] = "center-down-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    else if (z < 0.5 && y > 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "west")
                        {
                            orientations[0] = "center-up-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "east")
                        {
                            orientations[0] = "center-up-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    break;

                case EnumAxis.Y:
                    if (z < 0.5 && x < 0.5)
                    {
                        orientations[0] = blockSel.HitPosition.Y < 0.5 ? "up" : "down";
                        orientations[1] = "north";
                    }
                    if (z > 0.5 && x > 0.5)
                    {
                        orientations[0] = blockSel.HitPosition.Y < 0.5 ? "up" : "down";
                        orientations[1] = "south";
                    }
                    if (z > 0.5 && x < 0.5)
                    {
                        orientations[0] = blockSel.HitPosition.Y < 0.5 ? "up" : "down";
                        orientations[1] = "west";
                    }
                    if (z < 0.5 && x > 0.5)
                    {
                        orientations[0] = blockSel.HitPosition.Y < 0.5 ? "up" : "down";
                        orientations[1] = "east";
                    }
                    break;

                case EnumAxis.Z:
                    if (x < 0.5 && y < 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "north")
                        {
                            orientations[0] = "center-down-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "south")
                        {
                            orientations[0] = "center-down-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    else if (x > 0.5 && y > 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "north")
                        {
                            orientations[0] = "center-up-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "south")
                        {
                            orientations[0] = "center-up-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    else if (x > 0.5 && y < 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "north")
                        {
                            orientations[0] = "center-down-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "south")
                        {
                            orientations[0] = "center-down-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    else if (x < 0.5 && y > 0.5)
                    {
                        if (blockSel.Face.Opposite.Code == "north")
                        {
                            orientations[0] = "center-up-left";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                        else if (blockSel.Face.Opposite.Code == "south")
                        {
                            orientations[0] = "center-up-right";
                            orientations[1] = blockSel.Face.Opposite.Code;
                        }
                    }
                    break;
            }

            var variants = new string[] { orientations[0], orientations[1] };
            var blockCode = block.CodeWithVariants(new string[] { "orientation1", "orientation2" }, variants);

            return byPlayer.Entity.World.GetBlock(blockCode);
        }

        public static Block RotatedPrism(this Block block, IPlayer byPlayer, BlockSelection blockSel)
        {
            var horVar = SuggestedHVOrientation(blockSel, byPlayer);
            var orientations = new List<string>() { null, null };

            var x = Math.Abs(blockSel.HitPosition.X - 0.5);
            var y = Math.Abs(blockSel.HitPosition.Y - 0.5);
            var z = Math.Abs(blockSel.HitPosition.Z - 0.5);

            switch (blockSel.Face.Axis)
            {
                case EnumAxis.X:
                    if (z < 0.3 && y < 0.3)
                    {
                        orientations[0] = "ver";
                        orientations[1] = blockSel.Face.Opposite.Code;
                    }
                    else if (z > y)
                    {
                        orientations[0] = "hor";
                        orientations[1] = blockSel.HitPosition.Z < 0.5 ? "north" : "south";
                    }
                    else
                    {
                        orientations[0] = "hor";
                        orientations[1] = blockSel.HitPosition.Y < 0.5 ? "we-down" : "we-up";
                    }
                    break;

                case EnumAxis.Y:
                    if (z < 0.3 && x < 0.3)
                    {
                        if (horVar[0].Code is "north" or "south")
                        {
                            orientations[0] = "hor";
                            orientations[1] = blockSel.HitPosition.Y < 0.5 ? "ns-up" : "ns-down";
                        }
                        else
                        {
                            orientations[0] = "hor";
                            orientations[1] = blockSel.HitPosition.Y < 0.5 ? "we-up" : "we-down";
                        }
                    }
                    else if (z > x)
                    {
                        orientations[0] = "ver";
                        orientations[1] = blockSel.HitPosition.Z < 0.5 ? "north" : "south";
                    }
                    else
                    {
                        orientations[0] = "ver";
                        orientations[1] = blockSel.HitPosition.X < 0.5 ? "west" : "east";
                    }
                    break;

                case EnumAxis.Z:
                    if (x < 0.3 && y < 0.3)
                    {
                        orientations[0] = "ver";
                        orientations[1] = blockSel.Face.Opposite.Code;
                    }
                    else if (x > y)
                    {
                        orientations[0] = "hor";
                        orientations[1] = blockSel.HitPosition.X < 0.5 ? "west" : "east";
                    }
                    else
                    {
                        orientations[0] = "hor";
                        orientations[1] = blockSel.HitPosition.Y < 0.5 ? "ns-down" : "ns-up";
                    }
                    break;
            }

            var variants = new string[] { orientations[0], orientations[1] };
            var blockCode = block.CodeWithVariants(new string[] { "orientation1", "orientation2" }, variants);

            return byPlayer.Entity.World.GetBlock(blockCode);
        }

        public static Block RotatedWedge(this Block block, IPlayer byPlayer, BlockSelection blockSel)
        {
            var horVar = SuggestedHVOrientation(blockSel, byPlayer);
            var orientations = new List<string>() { null, null };

            var x = Math.Abs(blockSel.HitPosition.X - 0.5);
            var y = Math.Abs(blockSel.HitPosition.Y - 0.5);
            var z = Math.Abs(blockSel.HitPosition.Z - 0.5);
            var opposite = blockSel.Face.Opposite.Code;

            switch (blockSel.Face.Axis)
            {
                case EnumAxis.X:
                    if (z < 0.3 && y < 0.3)
                    {
                        orientations[0] = "center-down";
                        orientations[1] = opposite;
                    }
                    else if (z > y)
                    {
                        orientations[0] = blockSel.HitPosition.Z < 0.5 ? "left" : "right";
                        orientations[1] = opposite;
                    }
                    else
                    {
                        orientations[0] = "center" + (blockSel.HitPosition.Y < 0.5 ? "-down" : "-up");
                        orientations[1] = opposite;
                    }
                    break;

                case EnumAxis.Y:
                    if (z < 0.3 && x < 0.3)
                    {
                        orientations[0] = blockSel.HitPosition.Y < 0.5 ? "center-up" : "center-down";
                        orientations[1] = horVar[0].Code;
                    }
                    else if (z > x)
                    {
                        orientations[0] = opposite;
                        orientations[1] = blockSel.HitPosition.Z < 0.5 ? "north" : "south";
                    }
                    else
                    {
                        orientations[0] = opposite;
                        orientations[1] = blockSel.HitPosition.X < 0.5 ? "west" : "east";
                    }
                    break;

                case EnumAxis.Z:
                    if (x < 0.3 && y < 0.3)
                    {
                        orientations[0] = "center-down";
                        orientations[1] = opposite;
                    }
                    else if (x > y)
                    {
                        orientations[0] = blockSel.HitPosition.X < 0.5 ? "left" : "right";
                        orientations[1] = opposite;
                    }
                    else
                    {
                        orientations[0] = "center" + (blockSel.HitPosition.Y < 0.5 ? "-down" : "-up");
                        orientations[1] = opposite;
                    }
                    break;
            }

            var variants = new string[] { orientations[0], orientations[1] };
            var blockCode = block.CodeWithVariants(new string[] { "orientation1", "orientation2" }, variants);

            return byPlayer.Entity.World.GetBlock(blockCode);
        }
    }
}