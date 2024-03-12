using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using WiitaMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace WiitaMod.World
{
    class TropicalOceanGeneration : ModSystem //CODE YOINKED FROM AEQUUS MOD!
    {
        public int size;
        public int wantedDirection;
        public Point location;
        public Point genLocation;
        public void Reset()
        {
            location = new Point();
            size = Main.maxTilesX / 28;
            if (size > 200) // small: 150, medium: ~215, large: 250
            {
                size -= 200;
                size /= 2;
                size += 200;
            }
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

            //WorldUtils.Gen(genLocation, new Shapes.Circle(x, y / 2), Actions.Chain(new GenAction[]{ new Actions.SetTile(TileID.Sandstone), new Actions.PlaceWall(WallID.Sandstone),}));

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
                            placeTiles.Add(new Point(x2, y2));
                            /*if (Main.tile[x2, y2].HasTile)
                            {
                                placeTiles.Add(new Point(x2, y2));
                            }*/
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
                                    Main.tile[x2 + m, y2 + n].TileType = TileID.HardenedSand;
                                    continue;
                                }
                            }
                            //Main.tile[x2 + m, y2 + n].TileType = TileID.Sandstone;
                            WorldGen.PlaceTile(x2 + m, y2 + n, TileID.Sandstone);
                        }
                    }
                }
                else
                {
                    for (int m = -2; m <= 2; m++)
                    {
                        for (int n = -2; n <= 2; n++)
                        {
                            /*if (!Main.tile[x2 + m, y2 + n].HasTile && Main.tile[x2 + m, y2 + n].LiquidAmount > 0)
                            {
                                continue;
                            }*/
                            Main.tile[x2 + m, y2 + n].TileType = (ushort)TileType<TropicalSand>();
                            WorldGen.PlaceTile(x2 + m, y2 + n, (ushort)TileType<TropicalSand>());
                        }
                    }
                }
            }
        }
        private void GenerateTropicalOcean()
        {
            int x = location.X;
            int y = location.Y; //+ 120
            genLocation = new Point(x, y);

            GenerateSandArea(x + wantedDirection * 20, y); //+ 40
        }
        public void Generate()
        {
            Reset();
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
            int ResetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Create Ocean Caves"));
            if (ResetIndex != -1)
            {
                tasks.Insert(ResetIndex + 1, new PassLegacy("Wiita's: Tropical Ocean", (progress, configuration) =>
                {
                    progress.Message = "Creating Tropical Ocean";
                    Generate();
                }));
            }
            int ResetIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
            if (ResetIndex2 != -1)
            {
                tasks.Insert(ResetIndex2 + 1, new PassLegacy("Wiita's: Beach extension", (progress, configuration) =>
                {
                    progress.Message = "Extending an ocean";
                    Reset();
                    GenVars.beachSandDungeonExtraWidth *= 2000;
                }));
            }

        }
    }
}