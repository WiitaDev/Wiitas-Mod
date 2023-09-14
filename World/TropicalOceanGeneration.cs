using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using WiitaMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace WiitaMod.World
{
    class TropicalOceanGeneration : ModSystem
    {
        public int size;
        public int nextChestLoot;
        public int wantedDirection;
        public Point location;
        public Point genLocation;
        public void Reset()
        {
            nextChestLoot = 0;
            location = new Point();
            size = Main.maxTilesX / 28;
            if (size > 200)
            {
                size -= 200;
                size /= 2;
                size += 200;
            }
        }
        public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }

        public override void PostUpdateEverything()
        {
            if (JustPressed(Keys.Insert))
                TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
        }

        private bool CanOverwriteTile(Tile tile)
        {
            return Main.tileStone[tile.TileType] || tile.TileType != TileID.Dirt || tile.TileType != TileID.ClayBlock;
            //return !Main.tileDungeon[tile.TileType] && !Main.wallDungeon[tile.WallType] && tile.TileType != TileID.LihzahrdBrick;
        }
        public bool ProperPlacement(int x, int y)
        {
            return !Main.tile[x, y].HasTile && Main.tile[x, y + 1].HasTile && (Main.tileSand[Main.tile[x, y + 1].TileType] || Main.tile[x, y + 1].TileType == TileID.ShellPile);
        }

        private void TestMethod(int x, int y)
        {
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

            // Code to test placed here:
            //WorldGen.TileRunner(x - 1, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), TileID.CobaltBrick);
            //location.X = x; location.Y = y;
            Generate();
        }
        public void GenerateSandArea(int x, int y) 
        {
            if (x - size < 10)
            {
                x = size + 10;
            }
            else if (x + size > Main.maxTilesX - 10)
            {
                x = Main.maxTilesX - 10 - size;
            }
            if (y - size < 10)
            {
                y = size + 10;
            }
            else if (y + size > Main.maxTilesY - 10)
            {
                y = Main.maxTilesY - 10 - size;
            }

            List<Point> placeTiles = new();
            for (int i = 0; i < size * 2; i++)
            {
                // A bit overkill of an extra check, but whatever
                for (int j = 0; j < size * 5; j++)
                {
                    int x2 = x + i - size;
                    int y2 = y + j - size;
                    int x3 = x2 - x;
                    int y3 = y2 - y;
                    if (Math.Sqrt(x3 * x3 + y3 * y3 * 0.175f) <= size)
                    {
                        if (CanOverwriteTile(Main.tile[x2, y2]) && (!Main.remixWorld || Main.rand.NextBool(10)))
                        {
                            if (Main.tile[x2, y2].HasTile)
                            {
                                placeTiles.Add(new Point(x2, y2));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < placeTiles.Count; i++)
            {
                int x2 = placeTiles[i].X;
                int y2 = placeTiles[i].Y;
                if (y2 > (int)Main.worldSurface)
                {
                    for (int m = -2; m <= 2; m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            if (y2 < Main.worldSurface + 25)
                            {
                                if (!WorldGen.genRand.NextBool(25 + ((int)Main.worldSurface - y2) + 2))
                                {
                                    Main.tile[x2 + m, y2 + n].TileType = (ushort)TileType<TropicalSand>();
                                    continue;
                                }
                            }
                            Main.tile[x2 + m, y2 + n].TileType = TileID.Sandstone;
                        }
                    }
                }
                else
                {
                    for (int m = -2; m <= 2; m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            if (!Main.tile[x2 + m, y2 + n].HasTile && Main.tile[x2 + m, y2 + n].LiquidAmount > 0)
                            {
                                continue;
                            }
                            Main.tile[x2 + m, y2 + n].TileType = TileID.Sand;
                        }
                    }
                }
            }
        }

        private void GenerateTropicalOcean()
        {
            int x = location.X;
            int y = location.Y + 120;
            genLocation = new Point(x, y);

            GenerateSandArea(x + wantedDirection * 20, y + 40);
        }
        public void Generate()
        {
            Reset();
            //wantedDirection = Main.dungeonX / Main.dungeonX; 
            wantedDirection = Main.dungeonX * 2 < Main.maxTilesX ? 1 : -1; // makes wanted direction opposite of dungeon
            for (int i = 0; i < 5000; i++)
            {
                int checkX = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    checkX = Main.maxTilesX - checkX;
                for (int checkY = 200; checkY < Main.worldSurface; checkY++)
                {
                    if (ProperPlacement(checkX, checkY))
                    {
                        if (wantedDirection == 0 || location.X == 0)
                        {
                            location.X = checkX;
                            location.Y = checkY;
                        }
                        else if (wantedDirection == -1)
                        {
                            if (checkX * 2 < Main.maxTilesX)
                            {
                                location.X = checkX;
                                location.Y = checkY;
                            }
                        }
                        else
                        {
                            if (checkX * 2 > Main.maxTilesX)
                            {
                                location.X = checkX;
                                location.Y = checkY;
                            }
                        }
                        i += 1000;
                        break;
                    }
                }
            }
            GenerateTropicalOcean();
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ResetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Larva"));
            if (ResetIndex != -1)
            {
                tasks.Insert(ResetIndex + 1, new PassLegacy("Wiita's Mod Tropical Ocean", (progress, configuration) =>
                {
                    progress.Message = "Creating a Tropical Ocean";
                    Generate();
                }));
            }
        }
    }
}