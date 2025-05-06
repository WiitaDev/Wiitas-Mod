using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Projectiles.Magic
{
    public class SandPouchProj : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        private Vector2 OriginPoint;
        private Vector2 TipPosition;
        private float RotationAngle;
        private float CurrentRadius;
        private const float RadiusSpeed = 1.8f;
        private const float MaxRadius = 120f;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 30) // Initial setup
            {
                OriginPoint = Projectile.Center;
                TipPosition = OriginPoint;
                CurrentRadius = 20f;
            }

            CurrentRadius = MathHelper.Min(CurrentRadius + RadiusSpeed, MaxRadius);
            UpdatePosition();
            CreateDust();
        }

        private void UpdatePosition()
        {
            // Calculate tip position relative to stationary projectile
            TipPosition = OriginPoint + new Vector2(
                (float)Math.Cos(RotationAngle) * CurrentRadius,
                (float)Math.Sin(RotationAngle) * CurrentRadius
            );

            // Update velocity for dust direction
            Projectile.velocity = TipPosition - OriginPoint;
        }

        // Rotated collision check
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), targetHitbox.Size(),
                OriginPoint, Projectile.Center,
                Projectile.width / 2, ref collisionPoint
            );
        }

        private void CreateDust()
        {
            // Create dust between stationary origin and moving tip
            int segments = (int)(CurrentRadius / 4);
            for (int i = 0; i < segments; i++)
            {
                float lerpValue = i / (float)segments;
                Vector2 dustPos = Vector2.Lerp(OriginPoint, TipPosition, lerpValue);

                Dust d = Dust.NewDustPerfect(
                    dustPos + Main.rand.NextVector2Circular(3, 3),
                    DustID.Sandstorm,
                    Projectile.velocity * 0.1f,
                    150, default, 1.1f
                );
                d.noGravity = true;
                d.fadeIn = 0.5f;
            }

        }
    }
}