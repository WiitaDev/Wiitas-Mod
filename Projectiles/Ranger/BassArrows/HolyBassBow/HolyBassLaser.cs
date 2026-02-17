using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Helpers;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Systems.Primitives;

namespace WiitaMod.Projectiles.Ranger.BassArrows.HolyBassBow
{
    public class HolyBassLaser : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public const float maxTimeLeft = 15f;
        public ref float Hits => ref Projectile.ai[0];

        public Vector2 startPoint;

        public float distance;

        public List<Vector2> points;

        private Vector2 direction;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000; // extra padding for drawing so that laser doesn't disappear if slightly off-screen
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 200;
            Projectile.timeLeft = (int)maxTimeLeft * Projectile.extraUpdates;
            Projectile.ignoreWater = true;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int paddingX = 6, paddingY = 6;
            hitbox.Width = Projectile.width + paddingX;
            hitbox.Height = Projectile.height + paddingY;
            hitbox.Offset(-paddingX / 2, -paddingY / 2);
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);

            if (Projectile.timeLeft == maxTimeLeft * Projectile.extraUpdates)
            {
                startPoint = Projectile.Center;
                direction = Projectile.velocity;
            }

            if (Projectile.timeLeft > (maxTimeLeft - 1f) * Projectile.extraUpdates)
            {

                points = new BezierCurve([startPoint, Projectile.Center]).GetPoints(10);
                points.Add(Projectile.Center); // Add the precise end point

                distance = Vector2.Distance(Projectile.Center, startPoint);
                // A bandaid fix to offset the last point a bit further because for some reason the laser doesn't render properly
                points[^1] += direction.SafeNormalize(Vector2.Zero) * distance * 0.04f;
            }
            else
            {
                Projectile.position += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                Projectile.friendly = false;
            }


            for (int i = 0; i < points.Count; i++)
            {
                Lighting.AddLight(points[i], Color.LightYellow.ToVector3() * Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Projectile.friendly = false;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Hits >= 3)
            {
                Projectile.damage = (int)(Projectile.damage * 0.85f);
            }
            Hits++;

            for (int i = 0; i < 8; i++)
            {
                Particle particle = new GlowOrbParticle(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity / 2f, false, 30, Main.rand.NextFloat(0.5f, 1f), new Vector2(1f, 2f), Color.Yellow, true);
                ParticleManager.SpawnParticle(particle);
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (points == null || points.Count < 2)
                return false;

            Color ColorFunction(float progress)
            {
                Color color = Color.Lerp(Color.Yellow, Color.White, Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));
                color.A = (byte)MathF.Max(100, Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates) * byte.MaxValue);
                return color;
            }

            float WidthFunction(float progress)
            {
                float scale = MathHelper.Lerp(35f, 1f, 1 - Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));

                float length = Vector2.Distance(startPoint, Projectile.Center);
                float taperDistance = 32f;
                float taperProgress = MathHelper.Clamp(taperDistance / length, 0f, 1f);

                if (progress < taperProgress)
                {
                    float t = progress / taperProgress;
                    return scale * t;
                }

                return scale;
            }


            GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/FuzzyLaser", AssetRequestMode.ImmediateLoad));
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(WidthFunction, ColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 40);

            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;
            Color glowColor = Color.Yellow;
            glowColor.A = 0;

            float glowSize = Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates) * 1.5f;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), glowColor, 0f, glow.Size() / 2, glowSize, SpriteEffects.None);

            return false;
        }

    }
}