using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using WiitaMod.Tiles;
using WiitaMod.World.CrystalCaverns;

namespace WiitaMod.World
{
    class WiitasWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ResetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Reset"));
            if (ResetIndex != -1)
            {
                tasks.Insert(ResetIndex + 1, new PassLegacy("Wiita's Mod Statue Setup", (progress, configuration) =>
                {
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
                        //WorldGen.StatuesWithTraps.Add(i);
                    }
                }));
            }

            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (index != -1)
            {
                tasks.Insert(index + 1, new CrystalCavernsGenPass("Crystal Caverns", 1.0));
            }
        }
    }
}