using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WiitaMod.Items.Pets;
using WiitaMod.Items.Placeable;
using WiitaMod.Systems;

namespace WiitaMod.NPCs
{
    public class BassCritter : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Goldfish);
            NPC.aiStyle = NPCAIStyleID.Piranha;
            AIType = NPCID.Goldfish;
            NPC.width = 30;
            NPC.height = 22;
            NPC.defense = 0;
            NPC.damage = 0;
            NPC.lifeMax = 5;
            NPC.catchItem = ItemID.Bass;
        }

        public override void OnKill()
        {
            for (int i = 0; i < 10; i++)
            {
                int dustHit = Dust.NewDust(NPC.Center, 1, 1, DustID.Blood, (float)Main.rand.Next(-3, 3), (float)Main.rand.Next(-3, 3), 0, default(Color), 1f);
                Main.dust[dustHit].scale = (float)Main.rand.Next(100, 135) * 0.013f;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Water && (spawnInfo.Player.ZoneOverworldHeight || spawnInfo.Player.ZoneNormalUnderground) && !spawnInfo.Player.ZoneBeach)
            {
                return SpawnCondition.WaterCritter.Chance;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;

            int speed = 6;

            if (NPC.wet)
            {
                if (NPC.frameCounter >= speed)
                {
                    NPC.frame.Y = (NPC.frame.Y + frameHeight) % (frameHeight * 2);
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                if (NPC.frameCounter >= speed)
                {
                    NPC.frame.Y = (NPC.frame.Y + frameHeight) % (frameHeight * 2) + frameHeight * 2;
                    NPC.frameCounter = 0;
                }

            }
        }
    }
}
