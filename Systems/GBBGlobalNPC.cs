using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WiitaMod.Items.Weapons;
using WiitaMod.Projectiles.Ranger.BassArrows;

namespace WiitaMod.Systems
{
    public class GBBGlobalNPC : GlobalNPC //GBB = Galactic(Cosmic) Bass Bow
    {
        public override bool InstancePerEntity => true;

        public bool GalacticHit;
        public int GalacticBassBowDamage;
        public int GalacticSwipeTimer;
        public int GalacticDeBuffTimer = -1;
        public float Size = 0f;
        private bool SpawnedRing = false;

        public override void ResetEffects(NPC npc)
        {   
        }
        public override void SetStaticDefaults()
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
            if (!GalacticHit) { return; }

            float RingShake = (180f - GalacticDeBuffTimer) * 0.02f;
            if (GalacticDeBuffTimer > 0)
            {
                Player player = Main.LocalPlayer;
                drawColor = Color.MediumPurple;
                Size += 0.1f;
                if (Size > 2f) { Size = 2f; }


                if (Main.myPlayer == player.whoAmI && Main.gamePaused == false && !SpawnedRing)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<TestingWeaponProj>(), 0, 0, Main.myPlayer, ai2: npc.whoAmI);
                    SpawnedRing = true;
                }


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
                drawColor = default;
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
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncNPC);
                npc.netUpdate = true;
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if(projectile.type == ModContent.ProjectileType<GalacticBassArrow>())
            {
                GalacticDeBuffTimer = 180;
                GalacticBassBowDamage = projectile.damage;
                GalacticHit = true;
                npc.netUpdate = true;
            }
        }
    }
}