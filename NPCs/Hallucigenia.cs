using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WiitaMod.Items.Consumables.Critters;
using WiitaMod.Systems;

namespace WiitaMod.NPCs
{
    public class Hallucigenia : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Snail);
            NPC.aiStyle = NPCAIStyleID.Snail;
            AIType = NPCID.Snail;
            NPC.width = 50;
            NPC.height = 26;
            NPC.defense = 0;
            NPC.damage = 0;
            NPC.lifeMax = 5;
            NPC.catchItem = ModContent.ItemType<HallucigeniaItem>();
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
            if (spawnInfo.Water && spawnInfo.Player.ZoneBeach)
                return SpawnCondition.Ocean.Chance;
            if (spawnInfo.Water && spawnInfo.Player.ZoneNormalUnderground)
                return SpawnCondition.WaterCritter.Chance;

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            if (NPC.velocity != Vector2.Zero) 
            {
                NPC.frameCounter++;

                int speed = 10;
                if (NPC.frameCounter >= speed)
                {
                    NPC.frame.Y = (NPC.frame.Y + frameHeight) % (frameHeight * Main.npcFrameCount[NPC.type]);
                    NPC.frameCounter = 0;
                }
            }
            else 
            {
                NPC.frame.Y = 0;
                NPC.frameCounter = 0;
            }

        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/NPCs/Hallucigenia_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(glow, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY - 2f), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

        }
    }
}
