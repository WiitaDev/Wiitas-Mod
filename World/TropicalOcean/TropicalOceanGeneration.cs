using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using WiitaMod.Tiles;
using WiitaMod.Walls;

namespace WiitaMod.World.TropicalOcean
{
    class TropicalOceanGeneration : ModSystem /// Totally not blatantly stolen from calamitys sulphurous sea code
    {
        public override void SaveWorldData(TagCompound tag)
        {
            if (CaveStart != 0)
            {
                tag["tropicalCaveStart"] = CaveStart; // real important for tropical caverns *biome* background and music stuff
            }
        }
        public override void LoadWorldData(TagCompound tag)
        {
            CaveStart = tag.GetAsInt("tropicalCaveStart");
        }

        public override void NetSend(BinaryWriter writer)
        {
            if(CaveStart != 0)
                writer.Write(CaveStart);
        }

        public override void NetReceive(BinaryReader reader)
        {
            CaveStart = reader.ReadInt32();
        }


        public const int TotalSandBeforeWater = 30;

        public const float TopWaterDepthPercentage = 0.175f;

        public const float TopWaterDescentSmoothness = 0.19f;

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
                    _ => (int)(Main.maxTilesX / 15f), // large is 560
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
                    _ => 0.9f
                };
                return (int)((Main.rockLayer + 180 - YStart) * depthFactor);
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

        public static int BeachWidth
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
            int ypos = 0;
            for (int y = 200; y < GenVars.worldSurfaceHigh; y++)
            {
                if (Main.tile[xCheckPosition, y].HasTile && ValidBeachConvertTiles.Contains(Main.tile[xCheckPosition, y].TileType))
                {
                    ypos = y;
                    break;
                }
            }

            WorldUtils.Find(new Point(xCheckPosition, ypos), searchCondition, out determinedPoint);
            YStart = determinedPoint.Y;
        }
        public void Generate()
        {
            DetermineYStart();

            GenerateSand();
            GenerateWater();
            SandstoneLine();
            GenerateCaveTunnel();

            RemoveTilesAbove();

            GenerateBeach();
            GenerateBeachLedge();
            SurfaceMounds();

            RemoveAloneAndSmoothBlocks();
            PreventSandFalling();

            CreateTrees();

            GenVars.structures.AddProtectedStructure(new(GetActualX(2) - BiomeWidth / 2 - 10, YStart - BlockDepth / 2 - 10, BiomeWidth + 20, BlockDepth + 20));
        }

        public void GenerateSand()
        {
            int width = BiomeWidth + 1;
            int maxDepth = BlockDepth;
            ushort sandID = (ushort)ModContent.TileType<TropicalSand>();
            ushort compsandstoneID = (ushort)ModContent.TileType<CompressedSandstone>();
            ushort sandstoneID = (ushort)TileID.HardenedSand;
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

                        if (!WorldGen.SolidTile(x, y + 1) && Main.tile[x, y].TileType == sandID) // Check for floating sand blocks
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
                        Main.tile[x, y].LiquidAmount = 0;
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
            int width = (int)((BiomeWidth - TotalSandBeforeWater) * 0.895f);
            float descentSmoothness = TopWaterDescentSmoothness;

            int heightSeed = WorldGen.genRand.Next();

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

                    Main.tile[x, y].WallType = WallID.None;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                    WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain([new Actions.SetLiquid(0, 255)]));


                    if (i == width / 4 && y == bottom - 1) //Set Cavestart
                    {
                        CaveStart = y;
                    }

                    // rough up the ocean bottom
                    if (y == bottom - 1 && i >= 2)
                    {
                        Tile t = SafeTile(x, y + 1);
                        ushort blockTileType = (ushort)ModContent.TileType<TropicalSand>();
                        if (t.HasTile)
                        {
                            float noise = FractalBrownianMotion(i * 0.0069f, y * 0.0069f, heightSeed, 5) * 0.5f + 0.5f;
                            noise = MathHelper.Lerp(noise, 0.5f, Utils.GetLerpValue(width - 13f, width - 1f, i, true));

                            int heightOffset = -(int)Math.Round(MathHelper.Lerp(-6, 10, noise));
                            for (int dy = 0; dy != heightOffset; dy += Math.Sign(heightOffset))
                            {
                                WorldUtils.Gen(new(x, y + dy), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                                {
                                    heightOffset > 0 ? new Actions.ClearTile() : new Actions.SetTile(blockTileType, true),
                                    new Actions.SetLiquid(),
                                    new Actions.Smooth(true)
                                }));
                            }
                        }

                    }

                }

                // Clear water that's above the level for some reason.
                for (int y = top - 150; y < top; y++)
                    SafeTile(x, y).LiquidAmount = 0;

            }

        }

        public void GenerateCaveTunnel()
        {
            int startX = GetActualX(BiomeWidth / 5);
            int dir = Main.dungeonX > Main.maxTilesX / 2 ? 1 : -1;
            int startY = CaveStart + 20 + BlockDepth / 3;

            Point bossCaveStart = new(GetActualX((int)((BiomeWidth - TotalSandBeforeWater) * 0.8f)), CaveStart + (BiomeWidth - 400) / 10 + BlockDepth / 8);
            int bossYRadius = 30;
            int bossXRadius = 55;
            var bossCircle = new Shapes.Circle(bossXRadius, bossYRadius);

            WorldUtils.Gen(bossCaveStart, bossCircle, Actions.Chain([new Actions.ClearTile(), new Actions.SetLiquid()])); // haha funni boss area
            for (int i = bossCaveStart.Y; i <= bossCaveStart.Y + bossYRadius; i++)
            {
                for (int x = bossCaveStart.X - bossXRadius; x <= bossCaveStart.X + bossXRadius; x++)
                    WorldUtils.Gen(new(x, i), new Shapes.Rectangle(1, 1), Actions.Chain([new Actions.ClearTile(), new Actions.SetLiquid()]));

                WorldGen.PlaceObject(bossCaveStart.X, i, (ushort)ModContent.TileType<CrabBossAltar>());
            }

            var v = WorldGen.digTunnel(startX, startY, 0, 0, 5, WorldGen.genRand.Next(6, 9), Wet: true); //spawn point
            int offset = 0;
            for (int y = YStart; y < startY; y++)
            {
                //Main.tile[startX, y].Get<TileWallWireStateData>().HasTile = false;
                //Main.tile[startX, y].LiquidAmount = byte.MaxValue;

                offset += WorldGen.genRand.Next(-1, 2);

                if (y % 4 == 1)
                    WorldGen.digTunnel(startX + offset, y, 0, BiomeWidth / 150, 7, WorldGen.genRand.Next(2, 4), Wet: true); // entrance tunnel
            }


            for (int i = 1; i <= 4; i++)
                WorldGen.digTunnel(v.X, v.Y, dir * -2, WorldGen.genRand.NextFloat(-0.12f, 0.3f), 25, WorldGen.genRand.Next(3, 5) + (BiomeWidth - 400) / 30, Wet: true); //backwards expanding tunnels

            for (int i = 1; i <= 6; i++)
                WorldGen.digTunnel(v.X, v.Y, dir * 1.5, WorldGen.genRand.NextFloat(0.12f, 0.2f) * -1, 35, WorldGen.genRand.Next(3, 5) + (BiomeWidth - 400) / 30, Wet: true); //random leading tunnels

            v = WorldGen.digTunnel(v.X, v.Y, dir * 2, -0.2f, 70, 3, Wet: true); //tiny leading tunnel

            Vector2 bossDir = bossCaveStart.ToVector2() - new Vector2((int)v.X, (int)v.Y);
            bossDir.Normalize();

            for (int i = 1; i <= 3; i++)
                v = WorldGen.digTunnel(v.X, v.Y, bossDir.X, bossDir.Y, 15, WorldGen.genRand.Next(3, 6), Wet: true); //continue from tiny

            var m = WorldGen.digTunnel(v.X, v.Y, dir * 1.5, WorldGen.genRand.NextFloat(0.05f, 0.015f), 40, WorldGen.genRand.Next(3, 5), Wet: true); //random middle tunnels
            for (int i = 1; i <= 5; i++)
                m = WorldGen.digTunnel(v.X, v.Y, dir * 1.5, WorldGen.genRand.NextFloat(0.05f, 0.015f), 40, WorldGen.genRand.Next(3, 5), Wet: true); //random middle tunnels

            if (Main.maxTilesX != 4200)   //medium & large world
            {
                for (int i = 1; i <= 7; i++)
                    WorldGen.digTunnel(m.X, m.Y, Main.rand.NextFloat(-0.75f, 0.75f), WorldGen.genRand.NextFloat(0.015f, 0.45f), 60, WorldGen.genRand.Next(5, 7), Wet: true); //Expand from middle
            }

            for (int i = 1; i <= 12 + (BiomeWidth - 400) / 30; i++)
            {
                bossDir = bossCaveStart.ToVector2() - new Vector2((int)v.X, (int)v.Y);
                bossDir.Normalize();
                if (i <= 6)
                {
                    bossDir = new(0.1f * -dir, -0.5f); //for the first 6 tunnels go straight up
                }

                v = WorldGen.digTunnel(v.X, v.Y, bossDir.X, bossDir.Y, 30, WorldGen.genRand.Next(3, 5), Wet: true); //finally go to boss room

            }
        }

        private void GenerateBeachLedge()
        {
            int LedgeStart = (int)((BiomeWidth - TotalSandBeforeWater) * 0.895f);
            int LedgeWidth = (int)(BiomeWidth * 0.42f);
            ushort sandID = (ushort)ModContent.TileType<TropicalSand>();
            ushort sandstoneID = TileID.HardenedSand;
            ushort wallID = (ushort)ModContent.WallType<TropicalSandstoneWall>();


            // Create the beach ledge
            for (int i = LedgeStart; i >= LedgeStart - LedgeWidth; i--)
            {
                int x = GetActualX(i);
                float xRatio = Utils.GetLerpValue(BiomeWidth, BiomeWidth - LedgeWidth, i, true);
                int depth = (int)(Math.Sin((1f - xRatio) * MathHelper.PiOver2) * 50f + 1f);
                for (int y = YStart; y < YStart + depth; y++)
                {
                    if (i <= BiomeWidth - LedgeWidth + 3) { break; }
                    Tile t = SafeTile(x, y);

                    if (y > YStart + depth - WorldGen.genRand.Next(5, 8))
                    {
                        WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
{
                            new Actions.SetTile(sandstoneID, true),
                        }));
                    }
                    else
                    {
                        WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                        {
                            new Actions.SetTile(sandID, true),
                        }));
                    }
                }
            }
        }

        private void GenerateBeach()
        {
            BeachWidth = WorldGen.genRand.Next(200, 240 + 1);
            var searchCondition = Searches.Chain(new Searches.Down(3000), new Conditions.IsSolid());
            ushort sandID = (ushort)ModContent.TileType<TropicalSand>();
            ushort wallID = (ushort)ModContent.WallType<TropicalSandstoneWall>();


            // Stop immediately if for some strange reason a valid tile could not be located for the beach starting point.
            if (!WorldUtils.Find(new Point(BiomeWidth + 4, (int)GenVars.worldSurfaceLow - 10), searchCondition, out Point determinedPoint))
                return;

            // Transform the landscape.
            for (int i = BiomeWidth - 10; i <= BiomeWidth + BeachWidth; i++)
            {
                int x = GetActualX(i);
                float xRatio = Utils.GetLerpValue(BiomeWidth - 10, BiomeWidth + BeachWidth, i, true);
                float ditherChance = Utils.GetLerpValue(0.92f, 0.99f, xRatio, true);
                int depth = (int)(Math.Sin((1f - xRatio) * MathHelper.PiOver2) * 110f + 1f);
                for (int y = YStart - 80; y < YStart + depth; y++)
                {
                    Tile tileAtPosition = SafeTile(x, y);
                    if (tileAtPosition.LiquidAmount > 0)
                    {
                        tileAtPosition.LiquidAmount = 0; // remove any excess water
                    }

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

        public void CreateTrees()
        {
            for (int i = 1; i < BiomeWidth; i++)
            {
                if (!WorldGen.genRand.NextBool(4)) // 1 in 4 chance
                    continue;

                int x = GetActualX(i);
                int y = YStart - 30;

                if (!WorldUtils.Find(
                    new Point(x, y),
                    Searches.Chain(
                        new Searches.Down(40),
                        new Conditions.IsTile((ushort)ModContent.TileType<TropicalSand>())),
                    out Point groundPoint)
                )
                    continue;

                x = groundPoint.X;
                y = groundPoint.Y - 1; // Position above sand

                if (SafeTile(x, y).LiquidAmount > 0)
                    continue;

                WorldGen.PlaceTile(x, y, ModContent.TileType<TropicalPalmSapling>(), true);

                // Attempt to grow IMMEDIATELY using MOD-SPECIFIC method
                if (!WorldGen.GrowTree(x, y))
                {
                    // Fallback to palm growth if needed
                    WorldGen.GrowPalmTree(x, y);
                }
            }
        }

        public void PreventSandFalling()
        {
            int width = BiomeWidth + 1;
            ushort sandID = (ushort)ModContent.TileType<TropicalSand>();
            ushort smoothsandstoneID = TileID.HardenedSand;

            int top = YStart - 90;
            int bottom = top + BlockDepth + 1;

            for (int i = 1; i < width + BeachWidth * 2; i++)
            {
                int x = GetActualX(i);

                for (int y = top; y < bottom; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (!WorldGen.SolidTile(Main.tile[x, y + 1])) // Check for floating sand blocks
                    {
                        if (tile.TileType == sandID)
                            tile.TileType = smoothsandstoneID;
                    }
                }
            }
        }

        public void RemoveTilesAbove()
        {
            for (int i = 1; i < BiomeWidth + BeachWidth; i++)
            {
                int x = GetActualX(i);
                for (int y = YStart - 140; y < YStart + 40; y++)
                {
                    int type = SafeTile(x, y).TileType;
                    if (YStartWhitelist.Contains(type) || OtherTilesForDestroy.Contains(type))
                        SafeTile(x, y).Get<TileWallWireStateData>().HasTile = false;
                    if (WallsForDestroy.Contains(SafeTile(x, y).WallType))
                        SafeTile(x, y).WallType = 0;

                    if (i > BiomeWidth)
                    {
                        if (SafeTile(x, y).LiquidAmount > 0)
                        {
                            SafeTile(x, y).LiquidAmount = 0;
                        }
                    }
                }
            }
        }

        public void RemoveAloneAndSmoothBlocks()
        {
            int top = YStart - 90;
            int bottom = YStart + BlockDepth;

            for (int i = 1; i < BiomeWidth; i++)
            {
                int x = GetActualX(i);
                for (int y = top; y < bottom; y++)
                {
                    Tile t = SafeTile(x, y);
                    if (t.Get<TileWallWireStateData>().HasTile)
                    {
                        if (i > 10)
                            WorldUtils.Gen(new Point(x, y), new Shapes.Rectangle(1, 1), Actions.Chain([new Actions.Smooth(true)])); // also smooth all the blocks

                        if (!SafeTile(x - 1, y).Get<TileWallWireStateData>().HasTile && !SafeTile(x + 1, y).Get<TileWallWireStateData>().HasTile && i > 5)
                            WorldUtils.Gen(new(x, y), new Shapes.Rectangle(1, 1), Actions.Chain([new Actions.ClearTile(true), new Actions.SetLiquid()]));
                    }
                }
            }
        }

        public void SandstoneLine()
        {
            int width = BiomeWidth;
            int depth = BlockDepth;

            int sandstoneSeed = WorldGen.genRand.Next();
            ushort blockTypeToReplace1 = (ushort)ModContent.TileType<TropicalSand>();
            ushort blockTypeToReplace2 = TileID.HardenedSand;
            ushort blockTypeToPlace = (ushort)ModContent.TileType<CompressedSandstone>();
            ushort wallID = (ushort)ModContent.WallType<TropicalSandstoneWall>();

            for (int i = 0; i < width; i++)
            {
                for (int y = YStart; y < YStart + depth; y++)
                {
                    int sandstoneLineOffset = (int)(FractalBrownianMotion(i * 0.00115f, y * 0.00115f, sandstoneSeed, 7) * 30) + (int)(depth * 0.45f);


                    sandstoneLineOffset -= (int)(Math.Pow(Utils.GetLerpValue(width * 0.1f, width * 0.8f, i, true), 1.72f) * 67f);

                    Point p = new(GetActualX(i), y);
                    Tile t = SafeTile(p.X, p.Y);
                    if (y >= YStart + sandstoneLineOffset && t.HasTile && (t.TileType == blockTypeToReplace1 || t.TileType == blockTypeToReplace2))
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

                    int heightOffset = -(int)Math.Round(MathHelper.Lerp(0, 10, noise));
                    for (int dy = 0; dy != heightOffset; dy += Math.Sign(heightOffset))
                    {
                        WorldUtils.Gen(new(x, y + dy), new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                        {
                            heightOffset > 0 ? new Actions.ClearTile() : new Actions.SetTile(blockTileType, true),
                            new Actions.PlaceWall(MathHelper.Distance(dy, heightOffset) >= 3f && heightOffset < 0f ? wallID : WallID.None, true),
                            //new Actions.SetLiquid(),
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
            TileID.JungleGrass,
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
            TileID.JungleVines,
            TileID.JunglePlants,
            TileID.JunglePlants2,
            TileID.OasisPlants,
            TileID.PlantDetritus,
            TileID.CorruptJungleGrass,
            TileID.CrimsonJungleGrass,
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.CorruptPlants,
            TileID.CrimsonPlants,
            TileID.MushroomPlants,
            TileID.DyePlants,
            TileID.Trees,
            TileID.Sunflower,
            TileID.FallenLog,
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
            TileID.CorruptThorns,
            TileID.CrimsonThorns,
            TileID.CorruptGrass,
            TileID.CorruptPlants,
            TileID.Stalactite,
            TileID.ImmatureHerbs,
            TileID.MatureHerbs,
            TileID.MushroomPlants,
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
            WallID.MudUnsafe,
            WallID.LivingWoodUnsafe,
            WallID.LivingWood,
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
            TileID.LivingWood,
            TileID.LeafBlock,
            TileID.Platforms,
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
            TileID.PlantDetritus,
            TileID.JunglePlants,
            TileID.JunglePlants2,
            TileID.BeeHive,
            TileID.SeaOats
        };
        #endregion Lists
    }
}