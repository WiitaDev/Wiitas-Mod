using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
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

        public List<Vector2> points;

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


        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);

            if (Projectile.timeLeft == maxTimeLeft * Projectile.extraUpdates)
            {
                startPoint = Projectile.Center;
            }

            if (Projectile.timeLeft > (maxTimeLeft - 1f) * Projectile.extraUpdates)
            {

                points = new BezierCurve([startPoint, Projectile.Center]).GetPoints(10);
                points.Add(Projectile.Center); // Add the precise end point

            }
            else
            {
                Projectile.position += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;


                Projectile.friendly = false;
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
                Projectile.damage = (int)(Projectile.damage * 0.8f);
            }
            Hits++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Lighting.AddLight(points[i], Color.LightYellow.ToVector3() * Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));
            }

            Color ColorFunction(float progress)
            {
                Color color = Color.Lerp(Color.Yellow, Color.White, Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));
                color.A = (byte)MathF.Max(100, Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates) * byte.MaxValue);
                return color;
            }

            float WidthFunction(float progress) {
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
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(WidthFunction, ColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 20);

            return false;
        }

    }
}