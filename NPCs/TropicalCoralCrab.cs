using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WiitaMod.Items.Armor;
using WiitaMod.Items.Pets;
using WiitaMod.Items.Placeable;
using WiitaMod.World;

namespace WiitaMod.NPCs
{
    public class TropicalCoralCrab : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hamis");

            NPCID.Sets.NoEarlymodeLootWhenSpawnedFromStatue[Type] = true;

            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 30;
            NPC.defense = 0;
            NPC.value = 1000f;
            NPC.CloneDefaults(NPCID.Crab);
            AIType = NPCID.Crab;

            NPC.knockBackResist = 0.3f;
            NPC.lifeMax = 100;
            NPC.damage = 18;

            Banner = NPCID.Crab; // Makes this NPC get affected by the normal zombie banner.
            BannerItem = ItemID.CrabBanner; // Makes kills of this NPC go towards dropping the banner it's associated with.
            SpawnModBiomes = new int[] { ModContent.GetInstance<TropicalOceanBiome>().Type };
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Coral, minimumDropped: 3, maximumDropped: 8)); 
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<TropicalOceanBiome>()))
            {
                return 0.5f;
            }

            return 0f;
        }
    }
}
