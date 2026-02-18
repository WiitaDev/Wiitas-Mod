using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;

namespace WiitaMod.Projectiles.Summon
{
    public class BassSummon : ModProjectile
    {
        private const float EngagementDistance = 700f;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.knockBack = 3f;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;

            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;

        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            UpdatePlayerStatus(player);

            bool onGround = (Projectile.velocity.Y == 0 || Projectile.oldVelocity.Y == 0) && Projectile.height == 18; // Projectile height thing to prevent fish from jumping immidiately when summoned


            Projectile.Resize(32, 18);

            if (Projectile.wet || Projectile.ai[0] == 1)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 8)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = (Projectile.frame + 1) % 2;
                }
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 8)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 2 + (Projectile.frame + 1) % 2;
                }
            }



            // Target Search
            NPC target = null;
            float maxDist = 700f;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy() && Vector2.Distance(Projectile.Center, npc.Center) < maxDist) target = npc;
            }

            if (target == null)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float dist = Vector2.Distance(Projectile.Center, npc.Center);
                        if (dist < maxDist && (Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1) || Collision.CanHitLine(player.Center, 1, 1, npc.Center, 1, 1)))
                        {
                            maxDist = dist;
                            target = npc;
                        }
                    }
                }
            }

            float distToPlayer = Vector2.Distance(Projectile.Center, player.Center);

            bool shouldFly = distToPlayer > 700f && target == null;

            if (distToPlayer < 150f) Projectile.ai[0] = 0;
            if (shouldFly) Projectile.ai[0] = 1;

            Projectile.spriteDirection = -Projectile.direction;

            if (Projectile.ai[0] == 1)
            {
                Projectile.tileCollide = false;
                Vector2 flyDir = player.Center - Projectile.Center;
                flyDir.Normalize();
                float flySpeed = 12f;

                Projectile.velocity = (Projectile.velocity * 15f + flyDir * flySpeed) / 16f;

                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.direction == -1) Projectile.rotation += MathHelper.Pi;
                Projectile.netUpdate = true;
                return;
            }

            Projectile.tileCollide = true;
            Projectile.velocity.Y += 0.4f;
            if (Projectile.velocity.Y > 10f) Projectile.velocity.Y = 10f;

            if (distToPlayer > 1400f)
            {
                Projectile.Center = player.Center;
            }
            else if (distToPlayer > 400f && target == null)
            {
                if (onGround)
                {
                    float jumpHeight = -12f;
                    float gravity = 0.4f;

                    float timeInAir = Math.Abs(jumpHeight / gravity) * 2f;
                    float neededX = (player.Center.X - Projectile.Center.X) / timeInAir;

                    Projectile.velocity.Y = jumpHeight;
                    Projectile.velocity.X = MathHelper.Clamp(neededX, -14f, 14f);
                    Projectile.netUpdate = true;
                }
            }
            else if (target != null && target.active)
            {
                Vector2 direction = target.Center - Projectile.Center;

                if (onGround)
                {
                    if (Math.Abs(direction.X) < 100f && Math.Abs(direction.Y) < 50f)
                    {
                        Projectile.velocity.Y = -5.5f;
                        Projectile.velocity.X = (direction.X > 0 ? 1 : -1) * Main.rand.NextFloat(1f,3f);
                    }
                    else
                    {
                        float jumpPower = -7f;
                        float gravity = 0.4f;

                        if (direction.Y < -50f) jumpPower = -10f;
                        if (direction.Y < -150f) jumpPower = -14f;

                        if (Math.Abs(direction.X) > 300f && jumpPower > -10f) jumpPower = -11f;

                        float timeInAir = Math.Abs(jumpPower / gravity) * 2f;
                        float neededX = direction.X / timeInAir;

                        Projectile.velocity.Y = jumpPower;
                        Projectile.velocity.X = MathHelper.Clamp(neededX, -12f, 12f);
                    }
                    Projectile.netUpdate = true;
                }
                else
                {
                    // If we are in the air, slightly nudge the fish toward the enemy 
                    float airTraction = 0.15f;
                    if (direction.X > 0) Projectile.velocity.X += airTraction;
                    else Projectile.velocity.X -= airTraction;

                    if (Math.Abs(Projectile.velocity.X) > 10f) Projectile.velocity.X *= 0.95f;
                    Projectile.netUpdate = true;
                }
            }
            else if (onGround)
            {
                // Idle Flop near player

                Projectile.velocity.X *= 0.9f;

                if (Vector2.Distance(Projectile.Center, player.Center) > 160f && Main.rand.NextBool(5))
                {
                    Projectile.velocity.Y = -5f;
                    Projectile.velocity.X = (player.Center.X > Projectile.Center.X ? 1 : -1) * 3f;
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDust(Projectile.Center + new Vector2(0, 6), 10, 10, DustID.Water_Snow, (float)Main.rand.Next(-5, 5), (float)Main.rand.Next(5, 10), 0, default(Color), Main.rand.NextFloat(0.65f, 1f));
                    }
                }
                else if (Main.rand.NextBool(40))
                {
                    Projectile.velocity.Y = -Main.rand.NextFloat(3f, 5f);
                    Projectile.velocity.X = Main.rand.NextFloat(-2f, 2f);
                }
                Projectile.netUpdate = true;
            }

            // Flop Rotation
            if (!onGround)
            {
                Projectile.rotation += Projectile.velocity.X * 0.1f;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Stinky, 180);
            target.AddBuff(BuffID.Wet, 180);
        }


        private void UpdatePlayerStatus(Player player)
        {
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<BassSummonBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<BassSummonBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }
    }
}