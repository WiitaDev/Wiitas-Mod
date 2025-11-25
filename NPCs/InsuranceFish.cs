using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WiitaMod.Items.Pets;
using WiitaMod.Items.Placeable;

namespace WiitaMod.NPCs
{
    public class InsuranceFish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NoEarlymodeLootWhenSpawnedFromStatue[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Piranha);
            NPC.width = 60;
            NPC.height = 40;
            NPC.damage = 10;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 1000f;
            NPC.aiStyle = NPCAIStyleID.Piranha; // 3 = Fighter AI(zombie, etc.), -1 = custom AI
            NPC.knockBackResist = 0.5f;
            NPC.npcSlots = 1f;

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HamisBannerItem>();
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.Lime.ToVector3() * 0.5f);
            NPC.spriteDirection = NPC.direction * -1;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneBeach)
            {
                return SpawnCondition.Ocean.Chance; // Spawn with 45% the chance of a regular zombie.
            }
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.PoopBlock,1, 67, 67)); // 1% chance to drop in normal mode and 1% in expert/master
        }
    }
}
