using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using WiitaMod.Tiles;

namespace WiitaMod.World.CrystalCaverns
{
    public class CrystalCavernsGenPass : GenPass
    {
        public CrystalCavernsGenPass(string name, double loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Growing crystal caverns";
            TryGenerateCrystalCave();
        }

        private void TryGenerateCrystalCave()
        {
            int radius = (int)(Main.maxTilesX * 0.03f);
            radius += WorldGen.genRand.Next(-10, 11);
            radius = Utils.Clamp(radius, 120, 260);

            int minX = radius + 100;
            int maxX = Main.maxTilesX - radius - 100;

            int minY = (int)Main.worldSurface + radius + 100;
            int maxY = (int)Main.rockLayer + 400;

            for (int stage = 0; stage < 4; stage++)
            {
                for (int attempt = 0; attempt < 500; attempt++)
                {
                    float minPercent;
                    float maxPercent;

                    if (stage == 0)
                    {
                        if (WorldGen.genRand.NextBool())
                        {
                            minPercent = 0.20f;
                            maxPercent = 0.40f;
                        }
                        else
                        {
                            minPercent = 0.60f;
                            maxPercent = 0.80f;
                        }
                    }
                    else if (stage == 1)
                    {
                        minPercent = 0.40f;
                        maxPercent = 0.60f;
                    }
                    else if (stage == 2)
                    {
                        if (WorldGen.genRand.NextBool())
                        {
                            minPercent = 0.05f;
                            maxPercent = 0.15f;
                        }
                        else
                        {
                            minPercent = 0.85f;
                            maxPercent = 0.95f;
                        }
                    }
                    else
                    {
                        minPercent = 0f;
                        maxPercent = 1f;
                    }

                    int x = (int)(Main.maxTilesX * WorldGen.genRand.NextFloat(minPercent, maxPercent));
                    x = Utils.Clamp(x, minX, maxX);

                    int y = WorldGen.genRand.Next(minY, maxY);

                    if (!IsSuitableLocation(x, y, radius))
                        continue;

                    if (!HasLargeCave(x, y, radius))
                        continue;

                    GenerateCave(x, y, radius);
                    return;
                }
            }
        }

        private bool IsSuitableLocation(int x, int y, int radius)
        {
            if (!WorldGen.InWorld(x, y, 30))
                return false;

            if (y < Main.worldSurface)
                return false;

            if (y > Main.maxTilesY - 300)
                return false;

            Tile tile = Framing.GetTileSafely(x, y);

            if (tile.HasTile)
                return false;

            if (TouchesDungeonOrDesertOrJungleOrSnow(x, y, radius))
                return false;

            return true;
        }

        private bool TouchesDungeonOrDesertOrJungleOrSnow(int centerX, int centerY, int radius)
        {
            int checkRadius = radius + 10;

            for (int angle = 0; angle < 360; angle++)
            {
                float radians = MathHelper.ToRadians(angle);
                int x = centerX + (int)(Math.Cos(radians) * checkRadius);
                int y = centerY + (int)(Math.Sin(radians) * checkRadius);

                if (!WorldGen.InWorld(x, y, 30))
                    continue;

                Tile tile = Framing.GetTileSafely(x, y);

                if (tile.TileType == TileID.BlueDungeonBrick || tile.TileType == TileID.GreenDungeonBrick || tile.TileType == TileID.PinkDungeonBrick
                    || tile.TileType == TileID.Sand || tile.TileType == TileID.JungleGrass || tile.TileType == TileID.SnowBlock)
                    return true;
            }

            return false;
        }

        private bool HasLargeCave(int centerX, int centerY, int radius)
        {
            int openTiles = 0;
            int checkRadius = Math.Min(radius / 3, 50);

            for (int x = centerX - checkRadius; x <= centerX + checkRadius; x++)
            {
                for (int y = centerY - checkRadius; y <= centerY + checkRadius; y++)
                {
                    if (!WorldGen.InWorld(x, y, 30))
                        continue;

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        openTiles++;
                }
            }

            return openTiles >= 100;
        }

        private void GenerateCave(int centerX, int centerY, int radius)
        {
            ConvertTerrain(centerX, centerY, radius);
            AddOuterPatches(centerX, centerY, radius);
            ClearLongMoss(centerX, centerY, radius);
            GenerateShrine(centerX, centerY, radius);
            PlaceGemstones(centerX, centerY, radius);

        }

        private void ConvertTerrain(int centerX, int centerY, int radius)
        {
            float frequency1 = WorldGen.genRand.NextFloat(3f, 6f);
            float frequency2 = WorldGen.genRand.NextFloat(6f, 10f);

            float phase1 = WorldGen.genRand.NextFloat(MathHelper.TwoPi);
            float phase2 = WorldGen.genRand.NextFloat(MathHelper.TwoPi);

            float amplitude1 = WorldGen.genRand.NextFloat(7f, 10f);
            float amplitude2 = WorldGen.genRand.NextFloat(4f, 6f);


            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (!WorldGen.InWorld(x, y, 30))
                        continue;

                    int dx = x - centerX;
                    int dy = y - centerY;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    float angle = (float)Math.Atan2(dy, dx);
                    float noise = (float)Math.Sin(angle * frequency1 + phase1) * amplitude1 + (float)Math.Sin(angle * frequency2 + phase2) * amplitude2;

                    if (distance > radius + noise)
                        continue;

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        continue;

                    if (!IsNormalCaveBlock(tile.TileType))
                        continue;

                    tile.TileType = (ushort)ModContent.TileType<Gabbro>();
                }
            }
        }

        private void AddOuterPatches(int centerX, int centerY, int radius)
        {
            int patches = radius / 3;

            for (int i = 0; i < patches; i++)
            {
                float angle = WorldGen.genRand.NextFloat(MathHelper.TwoPi);
                float distance = WorldGen.genRand.NextFloat(radius * 0.95f, radius * 1.1f);

                int x = centerX + (int)(Math.Cos(angle) * distance);
                int y = centerY + (int)(Math.Sin(angle) * distance);

                if (!WorldGen.InWorld(x, y, 30))
                    continue;

                int patchRadius = WorldGen.genRand.Next(radius / 18, radius / 14);
                int patchLength = WorldGen.genRand.Next(4, 9);

                GeneratePatch(x, y, patchRadius, patchLength);
            }
        }

        private void GeneratePatch(int centerX, int centerY, int radius, int length)
        {
            float angle = WorldGen.genRand.NextFloat(MathHelper.TwoPi);

            for (int i = 0; i < length; i++)
            {
                float progress = i / (float)length;

                int x = centerX + (int)(Math.Cos(angle) * i * (radius / 3));
                int y = centerY + (int)(Math.Sin(angle) * i * (radius / 3));

                GeneratePatchCircle(x, y, radius, progress);
            }
        }

        private void GeneratePatchCircle(int centerX, int centerY, int radius, float progress)
        {
            int currentRadius = (int)(radius * (1f - progress * 0.5f));

            for (int x = centerX - currentRadius; x <= centerX + currentRadius; x++)
            {
                for (int y = centerY - currentRadius; y <= centerY + currentRadius; y++)
                {
                    if (!WorldGen.InWorld(x, y, 30))
                        continue;

                    int dx = x - centerX;
                    int dy = y - centerY;

                    if (dx * dx + dy * dy > currentRadius * currentRadius)
                        continue;

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        continue;

                    if (!IsNormalCaveBlock(tile.TileType))
                        continue;

                    tile.TileType = (ushort)ModContent.TileType<Gabbro>();
                }
            }
        }


        private void PlaceGemstones(int centerX, int centerY, int radius)
        {
            for (int x = centerX - radius + 10; x <= centerX + radius - 10; x++)
            {
                for (int y = centerY - radius + 10; y <= centerY + radius - 10; y++)
                {
                    if (!WorldGen.InWorld(x, y, 20))
                        continue;

                    if (WorldGen.genRand.NextBool(18) == false)
                        continue;

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (tile.HasTile)
                        continue;

                    bool touchingCrystal =
                        IsGabbro(x + 1, y) ||
                        IsGabbro(x - 1, y) ||
                        IsGabbro(x, y + 1) ||
                        IsGabbro(x, y - 1);

                    if (!touchingCrystal)
                        continue;

                    WorldGen.PlaceTile(x, y, TileID.ExposedGems, true, true, style: WorldGen.genRand.Next(0,7));
                }
            }
        }

        private bool IsGabbro(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return false;

            Tile tile = Framing.GetTileSafely(x, y);

            return tile.HasTile && tile.TileType == ModContent.TileType<Gabbro>();
        }

        private void GenerateShrine(int centerX, int centerY, int radius)
        {
            ShapeData slimeShapeData = new ShapeData();
            ShapeData moundShapeData = new ShapeData();

            int shrineX = centerX + WorldGen.genRand.Next(-radius / 10, radius / 10 + 1);
            int shrineY = centerY + WorldGen.genRand.Next(-radius / 10, radius / 10 + 1);

            Point shrinePoint = new Point(shrineX, shrineY);

            float xScale = 1f + WorldGen.genRand.NextFloat() * 0.3f;

            int chamberSize = 15 + radius / 15;
            int outerSize = chamberSize + 5;

            Point moundPoint = new Point(shrineX, shrineY + chamberSize / 2);

            WorldUtils.Gen(shrinePoint, new Shapes.Slime(outerSize, xScale, 1f), Actions.Chain(new Modifiers.Blotches(2, 0.4f), new Actions.SetTile((ushort)ModContent.TileType<Gabbro>()), new Actions.SetFrames(frameNeighbors: true)));

            WorldUtils.Gen(shrinePoint, new Shapes.Slime(chamberSize, xScale, 1f), Actions.Chain(new Modifiers.Blotches(2, 0.4f), new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));

            WorldUtils.Gen(moundPoint, new Shapes.Mound((int)(chamberSize * 0.9f), (int)(chamberSize * 0.4f)), Actions.Chain(new Modifiers.Blotches(2, 1f), new Actions.SetTile((ushort)ModContent.TileType<Gabbro>()), new Actions.SetFrames(frameNeighbors: true).Output(moundShapeData)));

            slimeShapeData.Subtract(moundShapeData, shrinePoint, moundPoint);
        }

        private void ClearLongMoss(int centerX, int centerY, int radius)
        {
            int checkRadius = radius;

            for (int x = centerX - checkRadius; x <= centerX + checkRadius; x++)
            {
                for (int y = centerY - checkRadius; y <= centerY + checkRadius; y++)
                {
                    if (!WorldGen.InWorld(x, y, 30))
                        continue;

                    int dx = x - centerX;
                    int dy = y - centerY;

                    if (dx * dx + dy * dy > checkRadius * checkRadius)
                        continue;

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        continue;

                    if (!ShouldBeCleared(tile.TileType))
                        continue;

                    WorldGen.KillTile(x, y, false, false, true);
                }
            }
        }

        private bool ShouldBeCleared(ushort type)
        {
            return type == TileID.Vines || type == TileID.JungleVines || type == TileID.LongMoss || type == TileID.MushroomVines || type == TileID.MushroomPlants;
        }

        private bool IsNormalCaveBlock(ushort type)
        {
            return type == TileID.Stone || type == TileID.Dirt || type == TileID.ClayBlock || type == TileID.Mud || type == TileID.BlueMoss || type == TileID.BrownMoss || type == TileID.GreenMoss
                || type == TileID.ArgonMoss || type == TileID.KryptonMoss || type == TileID.LavaMoss || type == TileID.PurpleMoss || type == TileID.RedMoss || type == TileID.XenonMoss
                || type == TileID.VioletMoss || type == TileID.MushroomGrass || type == TileID.Silt || type == TileID.Granite || type == TileID.Marble || type == TileID.Sand
                || type == TileID.Sandstone;
        }
    }
}