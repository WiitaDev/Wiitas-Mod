using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using WiitaMod.Tiles;
using WiitaMod.Walls;

namespace WiitaMod.World
{
    class TropicalOceanGeneration : ModSystem /// I hope that calamity people don't sue me or something lol
    {

        public const int TotalSandBeforeWaterMin = 22;

        public const int TotalSandBeforeWaterMax = 32;

        public const float TopWaterDepthPercentage = 0.175f;

        public const float TopWaterDescentSmoothnessMin = 0.19f;

        public const float TopWaterDescentSmoothnessMax = 0.20f;

        public const int DepthForWater = 5;

        public static int MaxTopWaterDepth => (int)(BlockDepth * TopWaterDepthPercentage);

        public static int BiomeWidth
        {
            get
            {
                return Main.maxTilesX switch
                {
                    // Small worlds.
                    4200 => 400,

                    // Medium worlds.
                    6400 => 475,

                    // Large worlds. This also accounts for worlds of an unknown size, such as extra large worlds.
                    _ => (int)(Main.maxTilesX / 16.8f),
                };
            }
        }

        public static int BlockDepth
        {
            get
            {
                float depthFactor = Main.maxTilesX switch
                {
                    // Small worlds.
                    4200 => 0.8f,

                    // Medium worlds.
                    6400 => 0.85f,

                    // Large worlds.
                    _ => 0.925f
                };
                return (int)((Main.rockLayer + 112 - YStart) * depthFactor);
            }
        }

        public static int YStart
        {
            get;
            set;
        }

        public static int CaveStart
        {
            get;
            set;
        }

        public static int GetActualX(int x)
        {
            if (Main.dungeonX > Main.maxTilesX / 2) //Jungle side (opposite of dungeon)
                return x;

            return Main.maxTilesX - 1 - x;
        }

        public static void DetermineYStart()
        {
            int xCheckPosition = GetActualX(BiomeWidth + 1);
            var searchCondition = Searches.Chain(new Searches.Down(3000), new Conditions.IsSolid());
            Point determinedPoint;

            WorldUtils.Find(new Point(xCheckPosition, (int)GenVars.worldSurfaceLow - 10), searchCondition, out determinedPoint);
            YStart = determinedPoint.Y;
        }
        private void Generate() 
        {
            DetermineYStart();

            GenerateSand();
            GenerateWater();
            GenerateCaveTunnel();

            RemoveTilesAbove();
            SandstoneLine();
            SurfaceMounds();

            GenerateBeach();
            PreventSandFalling();
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int Index = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids Again"));
            if (Index != -1)
            {
                tasks.Insert(Index + 1, new PassLegacy("Tropical Ocean", (progress, configuration) =>
                {
                    progress.Message = "Creating Tropical Ocean";
                    Generate();
                }));
            }
        }

        public void GenerateSand()
        {
            int width = BiomeWidth + 1;
            int maxDepth = BlockDepth;
            ushort sandID = (ushort)ModContent.TileType<TropicalSand>();
            ushort sandstoneID = TileID.SmoothSandstone;
            ushort wallID = (ushort)ModContent.WallType<TropicalSandstoneWall>();

            for (int i = 1; i < width; i++)
            {
                int x = GetActualX(i);

                // Calculate the 0-1 factor that determines how far down a vertical strip of the sea should descend.
                float depthFactor = (float)Math.Pow(Math.Sin((1f - i / (float)width) * MathHelper.PiOver2), 0.19f);

                // Determine the top and botton of the strip.
                int top = YStart;
                int bottom = top + (int)(maxDepth * depthFactor);
                for (int y = top; y < bottom; y++)
                {
                    float ditherChance = CalculateDitherChance(width, top, bottom, i, y);
                    if (WorldGen.genRand.NextFloat() >= ditherChance)
                    {
                        Main.tile[x, y].TileType = sandID;

                        if (!Main.tile[x, y + 1].HasTile && Main.tile[x, y].TileType == sandID && y >= (int)GenVars.worldSurfaceLow) // Check for floating sand blocks
                        {
                            Main.tile[x, y].TileType = sandstoneID;
                        }

                        if (y >= top + 45)
                            Main.tile[x, y].WallType = wallID;
                    }

                    // Ensure that the sand pops into existence if there is no chance that dithering will occur.
                    // This doesn't happen if there is dithering to ensure that there aren't stray sand tiles in the middle of open
                    // caves that have nothing to connect to.
                    if (ditherChance <= 0f)
                    {
                        Main.tile[x, y].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[x, y].Get<TileWallWireStateData>().IsHalfBlock = false;
                        Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    }
                }

                if (!Main.remixWorld)
                {
                    for (int y = top - 75; y < top + 50; y++)
                    {
                        if (Main.tile[x, y].TileType == TileID.PalmTree)
                            WorldGen.KillTile(x, y);
                    }
                }
            }
        }

        public void GenerateWater()
        {
            int maxDepth = MaxTopWaterDepth;
            int totalSandTilesBeforeWater = WorldGen.genRand.Next(TotalSandBeforeWaterMin, TotalSandBeforeWaterMax);
            int width = (int)((BiomeWidth - totalSandTilesBeforeWater) * 0.895f);
            float descentSmoothness = WorldGen.genRand.NextFloat(TopWaterDescentSmoothnessMin, TopWaterDescentSmoothnessMax);


            for (int i = 1; i < width; i++)
            {
                int x = GetActualX(i);

                // Calculate the 0-1 factor that determines how far down a vertical strip of water should descend.
                float depthFactor = (float)Math.Pow(Math.Sin((1f - i / (float)width) * MathHelper.PiOver2), descentSmoothness * 1.5f);

                // Determine the top and botton of the water strip.
                int top = YStart;
                int bottom = top + (int)(maxDepth * depthFactor * 2);
                for (int y = top; y < bottom; y++)
                {
                    /*if (y >= top + DepthForWater)
                        Main.tile[x, y + WorldGen.genRand.Next(22, 25)].WallType = (ushort)ModContent.WallType<TropicalSandstoneWall>();*/

                    Main.tile[x, y].LiquidAmount = byte.MaxValue;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;

                    if(i == width / 4 && y == bottom -1 ) 
                    {
                        CaveStart = y;
                    }
                }

                // Clear water that's above the level for some reason.
                for (int y = top - 150; y < top + DepthForWater; y++)
                    Main.tile[x, y].LiquidAmount = 0;

            }
        }

        public void GenerateCaveTunnel() 
        {
            int x = GetActualX(BiomeWidth / 5);
            int y = CaveStart + 40 + BlockDepth / 3;

            int dir = x * 2 > Main.maxTilesX ? -1 : 1;
            WorldGen.digTunnel(x, y, dir * 0.5f, -BiomeWidth / 150, 125, WorldGen.genRand.Next(3, 5), Wet: true); // entrance tunnels
            WorldGen.digTunnel(x, y, dir, 2.74f, 5, WorldGen.genRand.Next(6, 9), Wet: true); // ^

            var v = WorldGen.digTunnel(x, y, 0, 0, 5, WorldGen.genRand.Next(6, 9), Wet: true); // ^
            for (int i = 1; i <= 5; i++)
                WorldGen.digTunnel(v.X, v.Y, dir * -2, WorldGen.genRand.NextFloat(-0.12f, 0.2f), 20, WorldGen.genRand.Next(3, 5), Wet: true); //random center tunnels

            v = WorldGen.digTunnel(v.X, v.Y, dir * 2, WorldGen.genRand.NextFloat(-0.1f, 0.02f), 70, 2, Wet: true); //tiny leading tunnel

            for (int i = 1; i <= 3; i++)
                v = WorldGen.digTunnel(v.X, v.Y, dir, WorldGen.genRand.NextFloat(-0.3f, 0.3f), 10, WorldGen.genRand.Next(3, 6), Wet: true); //continue from tiny
        }

        private void GenerateBeach()
        {
            int beachWidth = WorldGen.genRand.Next(180, 240 + 1);
            var searchCondition = Searches.Chain(new Searches.Down(3000), new Conditions.IsSolid());
            ushort sandID = (ushort)ModContent.TileType<TropicalSand>();
            ushort sandstoneID = TileID.SmoothSandstone;
            ushort wallID = (ushort)ModContent.WallType<TropicalSandstoneWall>();


            // Stop immediately if for some strange reason a valid tile could not be located for the beach starting point.
            if (!WorldUtils.Find(new Point(BiomeWidth + 4, (int)GenVars.worldSurfaceLow - 10), searchCondition, out Point determinedPoint))
                return;

            // Transform the landscape.
            for (int i = BiomeWidth - 10; i <= BiomeWidth + beachWidth; i++)
            {
                int x = GetActualX(i);
                float xRatio = Utils.GetLerpValue(BiomeWidth - 10, BiomeWidth + beachWidth, i, true);
                float ditherChance = Utils.GetLerpValue(0.92f, 0.99f, xRatio, true);
                int depth = (int)(Math.Sin((1f - xRatio) * MathHelper.PiOver2) * 75f + 1f);
                for (int y = YStart - 80; y < YStart + depth; y++)
                {
                    Tile tileAtPosition = SafeTile(x, y);
                    if (tileAtPosition.HasTile && ValidBeachDestroyTiles.Contains(tileAtPosition.TileType))
                    {
                        // Kill trees manually so that no leftover tiles are present.
                        if (Main.tile[x, y].TileType == TileID.Trees)
                            WorldGen.KillTile(x, y);
                        else
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                    }
                    else if (tileAtPosition.HasTile && ValidBeachConvertTiles.Contains(tileAtPosition.TileType) && WorldGen.genRand.NextFloat() >= ditherChance)
                        Main.tile[x, y].TileType = sandID;

                    if (tileAtPosition.WallType > WallID.None)
                        Main.tile[x, y].WallType = wallID;
                }
            }
        }

        public void PreventSandFalling()
        {
            int width = BiomeWidth + 1;
            int maxDepth = BlockDepth;
            ushort sandID = (ushort)ModContent.TileType<TropicalSand>();
            ushort sandstoneID = TileID.SmoothSandstone;

            int top = YStart - 90;
            int bottom = top + maxDepth + 1;

            for (int i = 1; i < width + 240; i++) // +240 is the beach max lenght
            {
                int x = GetActualX(i);

                for (int y = top; y < bottom; y++)
                {
                    if (!WorldGen.SolidTile(x, y + 1) && Main.tile[x, y].TileType == sandID) // Check for floating sand blocks
                    {
                        Main.tile[x, y].TileType = sandstoneID;
                    }
                }
            }
        }

        public void RemoveTilesAbove()
        {
            for (int i = 0; i < BiomeWidth; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart - 140; y < YStart + 40; y++)
                {
                    int type = SafeTile(x, y).TileType;
                    if (YStartWhitelist.Contains(type) ||
                        OtherTilesForDestroy.Contains(type))
                        SafeTile(x, y).Get<TileWallWireStateData>().HasTile = false;
                    if (WallsForDestroy.Contains(SafeTile(x, y).WallType))
                        SafeTile(x, y).WallType = 0;
                }
            }
        }
        public void SandstoneLine()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;

            int sandstoneSeed = WorldGen.genRand.Next();
            ushort blockTypeToReplace = (ushort)ModContent.TileType<TropicalSand>();
            ushort blockTypeToPlace = TileID.SmoothSandstone;
            ushort wallID = (ushort)ModContent.WallType<TropicalSandstoneWall>();

            for (int i = 0; i < width; i++)
            {
                for (int y = YStart; y < YStart + depth; y++)
                {
                    int sandstoneLineOffset = (int)(FractalBrownianMotion(i * 0.00115f, y * 0.00115f, sandstoneSeed, 7) * 30) + (int)(depth * 0.58f);


                    sandstoneLineOffset -= (int)(Math.Pow(Utils.GetLerpValue(width * 0.1f, width * 0.8f, i, true), 1.72f) * 67f);

                    Point p = new(GetActualX(i), y);
                    Tile t = SafeTile(p.X, p.Y);
                    if (y >= YStart + sandstoneLineOffset && t.HasTile && t.TileType == blockTypeToReplace)
                    {
                        WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetTile(blockTypeToPlace, true),
                            new Actions.PlaceWall(wallID, true),
                            new Actions.SetLiquid()
                        }));
                    }
                }
            }
        }

        public void SurfaceMounds()
        {
            int y = YStart;
            int width = BiomeWidth;
            int heightSeed = WorldGen.genRand.Next();
            ushort blockTileType = (ushort)ModContent.TileType<TropicalSand>();
            ushort wallID = (ushort)ModContent.WallType<TropicalSandstoneWall>();

            for (int i = 2; i < width; i++)
            {
                int x = GetActualX(i);
                Tile t = SafeTile(x, y);

                if (t.HasTile)
                {
                    float noise = FractalBrownianMotion(i * 0.0079f, y * 0.0079f, heightSeed, 5) * 0.5f + 0.5f;
                    noise = MathHelper.Lerp(noise, 0.5f, Utils.GetLerpValue(width - 13f, width - 1f, i, true));

                    int heightOffset = -(int)Math.Round(MathHelper.Lerp(-6, 10, noise));
                    for (int dy = 0; dy != heightOffset; dy += Math.Sign(heightOffset))
                    {
                        WorldUtils.Gen(new(x, y + dy), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                        {
                            heightOffset > 0 ? new Actions.ClearTile() : new Actions.SetTile(blockTileType, true),
                            new Actions.PlaceWall(MathHelper.Distance(dy, heightOffset) >= 3f && heightOffset < 0f ? wallID : WallID.None, true),
                            new Actions.SetLiquid(),
                            new Actions.Smooth(true)
                        }));
                    }
                }
            }
        }

        public Tile SafeTile(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return new Tile();

            return Main.tile[x, y];
        }

        public static float CalculateDitherChance(int width, int top, int bottom, int x, int y)
        {
            float verticalCompletion = Utils.GetLerpValue(top, bottom, y, true);
            float horizontalDitherChance = Utils.GetLerpValue(0.9f, 1f, x / (float)width, true);
            float verticalDitherChance = Utils.GetLerpValue(0.9f, 1f, verticalCompletion, true);
            float ditherChance = horizontalDitherChance + verticalDitherChance;
            if (ditherChance > 1f)
                ditherChance = 1f;

            // Make the dither chance fizzle out at low vertical completion values.
            // This is done so that there isn't dithering on the surface of the sea.
            ditherChance -= Utils.GetLerpValue(0.56f, 0.5f, verticalCompletion, true);
            if (ditherChance < 0f)
                ditherChance = 0f;
            return ditherChance;
        }

        public static float FractalBrownianMotion(float x, float y, int seed, int octaves, float gain = 0.5f, float lacunarity = 2f)
        {
            float result = 0f;
            float frequency = 1f;
            float amplitude = 0.5f;
            x += seed * 0.00489937f % 10f;

            for (int i = 0; i < octaves; i++)
            {
                float noise = NoiseHelper.GetStaticNoise(new Vector2(x, y) * frequency) * 2f - 1f;
                result += noise * amplitude;
                amplitude *= gain;
                frequency *= lacunarity;
            }

            return result;
        }

        #region Lists
        public static readonly List<int> ValidBeachConvertTiles = new()
        {
            TileID.Dirt,
            TileID.Stone,
            TileID.Crimstone,
            TileID.Ebonstone,
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.CrimsonGrass,
            TileID.ClayBlock,
            TileID.Mud,
        };

        public static readonly List<int> ValidBeachDestroyTiles = new()
        {
            TileID.Vines,
            TileID.VineFlowers,
            TileID.Coral,
            TileID.BeachPiles,
            TileID.Plants,
            TileID.Plants2,
            TileID.SmallPiles,
            TileID.LargePiles,
            TileID.LargePiles2,
            TileID.JungleGrass,
            TileID.JungleVines,
            TileID.JunglePlants,
            TileID.JunglePlants2,
            TileID.PlantDetritus,
            TileID.CorruptJungleGrass,
            TileID.CrimsonJungleGrass,
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.CorruptPlants,
            TileID.CrimsonPlants,
            TileID.DyePlants,
            TileID.Trees,
            TileID.Sunflower,
            TileID.LilyPad,
            TileID.SeaOats,
            TileID.ImmatureHerbs,
            TileID.MatureHerbs,
            TileID.BloomingHerbs,
            TileID.VanityTreeSakura,
            TileID.VanityTreeYellowWillow,
        };

        public static readonly List<int> OtherTilesForDestroy = new()
        {
            TileID.PalmTree,
            TileID.Sunflower,
            TileID.JungleGrass,
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.CorruptGrass,
            TileID.CorruptPlants,
            TileID.Stalactite,
            TileID.ImmatureHerbs,
            TileID.MatureHerbs,
            TileID.Pots,
            TileID.Pumpkins,
            TileID.FallenLog,
            TileID.LilyPad,
            TileID.VanityTreeSakura,
            TileID.VanityTreeYellowWillow,
            TileID.ShellPile,
        };

        public static readonly List<int> WallsForDestroy = new()
        {
            WallID.Dirt,
            WallID.DirtUnsafe,
            WallID.DirtUnsafe1,
            WallID.DirtUnsafe2,
            WallID.DirtUnsafe3,
            WallID.DirtUnsafe4,
            WallID.Cave6Unsafe, // Rocky dirt wall
            WallID.Grass,
            WallID.GrassUnsafe,
            WallID.Flower,
            WallID.FlowerUnsafe,
            WallID.CorruptGrassUnsafe,
            WallID.EbonstoneUnsafe,
            WallID.CrimstoneUnsafe,
        };

        public static readonly List<int> YStartWhitelist = new()
        {
            TileID.Stone,
            TileID.Dirt,
            TileID.Sand,
            TileID.Ebonsand,
            TileID.Crimsand,
            TileID.Grass,
            TileID.CorruptGrass,
            TileID.CrimsonGrass,
            TileID.ClayBlock,
            TileID.Mud,
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Silver,
            TileID.Tungsten,
            TileID.Crimstone,
            TileID.Ebonstone,
            TileID.HardenedSand,
            TileID.CorruptHardenedSand,
            TileID.CrimsonHardenedSand,
            TileID.Coral,
            TileID.BeachPiles,
            TileID.Plants,
            TileID.Plants2,
            TileID.SmallPiles,
            TileID.LargePiles,
            TileID.LargePiles2,
            TileID.Vines,
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.CrimsonVines,
            TileID.Containers,
            TileID.DyePlants,
            TileID.JungleGrass,
            TileID.SeaOats
        };
        #endregion Lists
    }
}