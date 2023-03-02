using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items;
using WiitaMod.Projectiles;

namespace WiitaMod.Systems
{
    public class ModGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool GalacticTargeting;
        public int GalacticBassBowDamage;
        public int GalacticSwipeTimer;
        public int GalacticDeBuffTimer = -1;
        public float Size = 0f;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.netID == NPCID.GoblinSummoner)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowflameApparitionStaff>(), 4, 1));
            }
        }

        public override void ResetEffects(NPC npc)
        {
            GalacticTargeting = false;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (Main.gamePaused == false)
            {
                GalacticDeBuffTimer--;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {

            if (GalacticDeBuffTimer > 0)
            {
                NetMessage.SendData(MessageID.SyncNPC);
                Size += 0.1f;
                if (Size > 2f) { Size = 2f; }
                for (int i = 0; i < 5 * (npc.height / 10); i++)
                {//Circle Appear
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * (npc.height * Size));
                    offset.Y += (float)(Math.Cos(angle) * (npc.height * Size));

                    Dust d2 = Dust.NewDustPerfect(npc.Center + offset, DustID.PurpleCrystalShard, npc.velocity, 0, Color.Purple, 1.25f);
                    d2.fadeIn = 0.1f;
                    d2.noGravity = true;
                }
                if (Size == 2f)
                {//Spawn projectiles to the marked enemy
                    Player player = Main.LocalPlayer;
                    GalacticSwipeTimer++;
                    if (GalacticSwipeTimer == 6)
                    {
                        GalacticSwipeTimer = 0;
                        if (Main.myPlayer == player.whoAmI && Main.gamePaused == false)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * (npc.height * Size));
                            offset.Y += (float)(Math.Cos(angle) * (npc.height * Size));

                            Vector2 v = Main.rand.NextVector2CircularEdge(offset.X, offset.Y);
                            Vector2 vector = v.SafeNormalize(Vector2.UnitY) * (npc.height * Size * 0.1f);
                            Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center - vector * 20f, vector, ModContent.ProjectileType<GalacticSlash>(), GalacticBassBowDamage - GalacticBassBowDamage / 4, 0f, Main.myPlayer, 0f, npc.Center.Y);
                        }
                    }
                }
            }
            else if (GalacticDeBuffTimer <= 0 && Size != 0.0f)
            {
                Size -= 0.1f;
                if (Size < 0f) { Size = 0f; }
                for (int i = 0; i < 5 * (npc.height / 10); i++)
                {//Circle Disappear
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * (npc.height * Size));
                    offset.Y += (float)(Math.Cos(angle) * (npc.height * Size));

                    Dust d2 = Dust.NewDustPerfect(npc.Center + offset, DustID.PurpleCrystalShard, npc.velocity, 0, Color.Purple, 1.25f);
                    d2.fadeIn = 0.1f;
                    d2.noGravity = true;
                }
            }

            npc.netUpdate = true;
        }
    }
}