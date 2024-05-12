using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using WiitaMod.Tiles;
using ReLogic.Utilities;
using System.Collections.Generic;
using System.Linq;
using System;
using Terraria.DataStructures;

namespace WiitaMod.World
{
    class WiitasWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ResetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
            if (ResetIndex != -1)
            {
                tasks.Insert(ResetIndex + 1, new PassLegacy("Wiita's Mod Setup", (progress, configuration) => {
                    progress.Message = "Adding Hamis Statues";

                    // Not necessary, just a precaution.
                    if (GenVars.statueList.Any(point => point.X == ModContent.TileType<HamisStatue>()))
                    {
                        return;
                    }

                    // Make space in the statueList array, and then add a Point16 of (TileID, PlaceStyle)
                    Array.Resize(ref GenVars.statueList, GenVars.statueList.Length + 1);

                    for (int i = GenVars.statueList.Length - 1; i < GenVars.statueList.Length; i++)
                    {
                        GenVars.statueList[i] = new Point16(ModContent.TileType<HamisStatue>(), 0);

                        // Do this if you want the statue to spawn with wire and pressure plate
                        // WorldGen.StatuesWithTraps.Add(i);
                    }
                }));
            }

            int Index = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (Index != -1)
            {
                tasks.Insert(Index + 1, new PassLegacy("Tropical Ocean", (progress, configuration) =>
                {
                    progress.Message = "Creating Tropical Ocean";
                    var tropicalOcean = ModContent.GetInstance<TropicalOceanGeneration>();
                    tropicalOcean.Generate();
                }));
            }

            int Index2 = tasks.FindIndex(genpass => genpass.Name.Equals("Shimmer"));
            if (Index2 != -1)
            {
                tasks.RemoveAt(Index2); //remove the old shimmer that would have spawned !!!!!!!!!!!!!!!!!
                tasks.Insert(Index2, new PassLegacy("Shimmer", (progress, configuration) =>
                {
                    progress.Message = "Relocate Shimmer";
                    int num702 = 250; // changed this from 50 to 250 in order to get the shimmer to spawn lower, so that it doesn't collide with the tropical ocean !!!!!!!!!!!!!!!!!
                    int num703 = (int)(Main.worldSurface + Main.rockLayer) / 2 + num702;
                    int num704 = (int)((double)((Main.maxTilesY - 250) * 2) + Main.rockLayer) / 3;
                    if (num704 > Main.maxTilesY - 330 - 100 - 30)
                    {
                        num704 = Main.maxTilesY - 330 - 100 - 30;
                    }
                    if (num704 <= num703)
                    {
                        num704 = num703 + 50;
                    }
                    int num705 = WorldGen.genRand.Next(num703, num704);
                    int num706 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.11)));
                    int num707 = (int)Main.worldSurface + 150;
                    int num708 = (int)(Main.rockLayer + Main.worldSurface + 200.0) / 2;
                    if (num708 <= num707)
                    {
                        num708 = num707 + 50;
                    }
                    if (WorldGen.tenthAnniversaryWorldGen)
                    {
                        num705 = WorldGen.genRand.Next(num707, num708);
                    }
                    int num709 = 0;
                    while (!WorldGen.ShimmerMakeBiome(num706, num705))
                    {
                        num709++;
                        if (WorldGen.tenthAnniversaryWorldGen && num709 < 10000)
                        {
                            num705 = WorldGen.genRand.Next(num707, num708);
                            num706 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.11)));
                        }
                        else if (num709 > 20000)
                        {
                            num705 = WorldGen.genRand.Next((int)Main.worldSurface + 100 + 20, num704);
                            num706 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.8), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.2)));
                        }
                        else
                        {
                            num705 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 20, num704);
                            num706 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.11)));
                        }
                    }
                    GenVars.shimmerPosition = new Vector2D((double)num706, (double)num705);
                    int num710 = 200;
                    GenVars.structures.AddProtectedStructure(new Rectangle(num706 - num710 / 2, num705 - num710 / 2, num710, num710));
                }));
            }
        }
    }
}