using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Projectiles.Melee
{
    public class ParacrystalliteShardProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of the projectile's hitbox.
            Projectile.height = 12; // The height of the projectile's hitbox.
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Particle particle = new GlowOrbParticle(Projectile.Center, Main.rand.NextVector2Circular(2, 2), false, 40, Main.rand.NextFloat(0.25f, 0.3f), new Vector2(0.75f, 1f), Color.Purple, true);
                ParticleManager.SpawnParticle(particle);
            }
        }


        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;

            Projectile.ai[0]++;

            Projectile.localAI[1] = Projectile.velocity.LengthSquared() * 0.5f;
            Projectile.rotation += Projectile.localAI[1];

            if (Projectile.ai[0] >= 30)
            {
                float maxDetectRadius = 300f;
                float maxSpeed = 8f;
                Projectile.friendly = true;

                // Trying to find NPC closest to the projectile
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                {
                    Projectile.velocity *= 0.98f;
                    return;
                }

                Projectile.timeLeft += 1;

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * maxSpeed, 0.1f);

            }
            else
            {
                Projectile.friendly = false;
                Projectile.velocity *= 0.98f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;
            Color glowColor = Color.DodgerBlue;
            glowColor.A = 0;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), glowColor, Projectile.rotation, glow.Size() / 2, new Vector2(0.45f, 0.45f), SpriteEffects.None);

            return true;
        }

        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
            }
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.85f;
            }
            return false;
        }
    }
}