using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;
using WiitaMod.NPCs;
using static Terraria.ModLoader.ModContent;

namespace WiitaMod.Tiles
{
    public class HamisStatue : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IsAMechanism[Type] = true; // Ensures that this tile and connected pressure plate won't be removed during the "Remove Broken Traps" worldgen step
            DustType = DustID.Silver;

            AddMapEntry(new Color(144, 148, 144), Language.GetText("MapObject.Statue"));
        }


        public override void HitWire(int i, int j)
        {
            // Find the coordinates of top left tile square through math
            int y = j - Main.tile[i, j].TileFrameY / 18;
            int x = i - Main.tile[i, j].TileFrameX / 18;

            const int TileWidth = 2;
            const int TileHeight = 3;

            // Here we call SkipWire on all tile coordinates covered by this tile. This ensures a wire signal won't run multiple times.
            for (int yy = y; yy < y + TileHeight; yy++)
            {
                for (int xx = x; xx < x + TileWidth; xx++)
                {
                    Wiring.SkipWire(xx, yy);
                }
            }

            float spawnX = (x + TileWidth * 0.5f) * 16;
            float spawnY = (y + TileHeight * 0.65f) * 16;

            var entitySource = new EntitySource_TileUpdate(x, y, context: "HamisStatue");

            // 30 is the time before it can be used again. NPC.MechSpawn checks nearby for other spawns to prevent too many spawns. 3 in immediate vicinity, 6 nearby, 10 in world.
            int spawnedNpcId = ModContent.NPCType<Hamis>();

            if (Wiring.CheckMech(x, y, 30) && NPC.MechSpawn(spawnX, spawnY, spawnedNpcId))
            {
                NPC.NewNPC(entitySource, (int)spawnX, (int)spawnY - 12, spawnedNpcId, ai2: -5); //ai2 is -5 because i cant get the NPC.SpawnedFromStatue shit to work
            }
        }
    }

    public class HamisStatueModWorld : ModSystem
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
                    if (GenVars.statueList.Any(point => point.X == TileType<HamisStatue>()))
                    {
                        return;
                    }

                    // Make space in the statueList array, and then add a Point16 of (TileID, PlaceStyle)
                    Array.Resize(ref GenVars.statueList, GenVars.statueList.Length + 1);

                    for (int i = GenVars.statueList.Length - 1; i < GenVars.statueList.Length; i++)
                    {
                        GenVars.statueList[i] = new Point16(TileType<HamisStatue>(), 0);

                        // Do this if you want the statue to spawn with wire and pressure plate
                        // WorldGen.StatuesWithTraps.Add(i);
                    }
                }));
            }
        }
    }
}