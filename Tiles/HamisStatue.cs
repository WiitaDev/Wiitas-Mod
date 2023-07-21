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
using WiitaMod.Items.Placeable;
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
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(144, 148, 144), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = 11;
        }


        public override void HitWire(int i, int j)
        {
            // Find the coordinates of top left tile square through math
            int y = j - Main.tile[i, j].TileFrameY / 18;
            int x = i - Main.tile[i, j].TileFrameX / 18;

            Wiring.SkipWire(x, y);
            Wiring.SkipWire(x, y + 1);
            Wiring.SkipWire(x, y + 2);
            Wiring.SkipWire(x + 1, y);
            Wiring.SkipWire(x + 1, y + 1);
            Wiring.SkipWire(x + 1, y + 2);

            // We add 16 to x to spawn right between the 2 tiles. We also want to right on the ground in the y direction.
            int spawnX = x * 16 + 16;
            int spawnY = (y + 3) * 16;

            // If you want to make an NPC spawning statue, see below.
            int npcIndex = -1;
            // 30 is the time before it can be used again. NPC.MechSpawn checks nearby for other spawns to prevent too many spawns. 3 in immediate vicinity, 6 nearby, 10 in world.
            if (Wiring.CheckMech(x, y, 30) && NPC.MechSpawn((float)spawnX, (float)spawnY, NPCType<Hamis>()))
            {
                npcIndex = NPC.NewNPC(NPC.GetSource_None(),spawnX, spawnY - 12, NPCType<Hamis>());
            }
            if (npcIndex >= 0)
            {
                Main.npc[npcIndex].value = 0f;
                Main.npc[npcIndex].npcSlots = 0f;
                // Prevents Loot if NPCID.Sets.NoEarlymodeLootWhenSpawnedFromStatue and !Main.HardMode or NPCID.Sets.StatueSpawnedDropRarity != -1 and NextFloat() >= NPCID.Sets.StatueSpawnedDropRarity or killed by traps.
                // Prevents CatchNPC
                Main.npc[npcIndex].SpawnedFromStatue = true;
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
                tasks.Insert(ResetIndex + 1, new PassLegacy("Wiita's Mod Statue Setup", (progress, configuration) => {
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