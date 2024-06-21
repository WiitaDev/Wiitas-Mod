using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Ranger.BassArrows.CosmicBassBow
{
    public class CosmicProjectile : ModProjectile
    {
        public float Timer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;        //The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = 0;
            Projectile.knockBack = 2f;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Timer = 20;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;


            float maxDetectRadius = 1400f; // The maximum radius at which a projectile can detect a target
            float speed = 20f; // The speed at which the projectile moves towards the target
            float turnSpeed = 250f;

            Timer++;
            if (Timer >= 40)
            {
                Projectile.friendly = true;
                // Trying to find NPC closest to the projectile
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.timeLeft -= 4;
                    return;
                }

                // Homing calculations
                Vector2 targetPos = closestNPC.Center - Projectile.Center;
                float length = targetPos.Length();
                targetPos.Normalize();

                Projectile.velocity = (Projectile.velocity * 20f + targetPos * (turnSpeed - length * 0.15f)) / 21f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= speed;
            }
            else
            {
                Projectile.friendly = false;
                Projectile.velocity *= 0.96f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.CosmicProjectileTrail).Draw(Projectile);

            Texture2D core = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/CircleGradient", AssetRequestMode.ImmediateLoad).Value;
            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;

            Color result = Color.Purple;
            result.A = 0;

            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(core, Projectile.Center - Main.screenPosition, core.Frame(), result, Projectile.rotation, core.Size() * 0.5f, Projectile.scale * 0.1f, 0, 0);
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), result, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, 0, 0);
            }


            return false;
        }

        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 15; i++)
            {
                Vector2 circle = Main.rand.NextVector2Circular(2f, 2f);
                int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.PurpleCrystalShard, circle.X, circle.X, 0, default(Color), 1f);
                Main.dust[dustHit].scale = (float)Main.rand.Next(135, 190) * 0.013f;
                Main.dust[dustHit].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item109.WithPitchOffset(-0.5f), Projectile.Center);
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}