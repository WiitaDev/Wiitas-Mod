using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using WiitaMod.Projectiles.BassArrows;

namespace WiitaMod.Systems
{
    public class GBBGlobalNPC : GlobalNPC //GBB = Galactic(Cosmic) Bass Bow
    {
        public override bool InstancePerEntity => true;

        public int GalacticBassBowDamage;
        public int GalacticSwipeTimer;
        public int GalacticDeBuffTimer = -1;
        public float Size = 0f;

        public override void ResetEffects(NPC npc)
        {
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


            float RingShake = (180 - GalacticDeBuffTimer) / 50;
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

                    Dust d2 = Dust.NewDustPerfect(npc.Center + offset, DustID.PurpleCrystalShard, npc.velocity + new Vector2(Main.rand.NextFloat(-RingShake, RingShake), Main.rand.NextFloat(-RingShake, RingShake)), 0, Color.Purple, 1.25f);
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
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if(projectile.type == ModContent.ProjectileType<GalacticBassArrow>())
            {
                GalacticDeBuffTimer = 180;
                GalacticBassBowDamage = projectile.damage;
            }
        }
    }
}