using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems.Primitives;

namespace WiitaMod.Projectiles.Ranger
{
    public class PiercingPegshot : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public Vector2 endPoint;
        public List<Vector2> points;


        private const float maxDistance = 1000f;

        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float Hits
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = 9999;
            Projectile.timeLeft = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter + new Vector2(0, -10) + Projectile.velocity * 6f; // the vector offset is the itemholdout y offset
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FindEndpoint();

            points = new BezierCurve([Projectile.Center, endPoint]).GetPoints(10);
        }

        private void FindEndpoint()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Player player = Main.player[Projectile.owner];

                for (Distance = 0; Distance <= maxDistance; Distance += 5f)
                {
                    var end = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Distance;
                    if (!Collision.CanHitLine(Projectile.Center, 1, 1, end, 1, 1))
                    {
                        Distance -= 5f;
                        break;
                    }
                }

                Vector2 mouse = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Distance;
                endPoint = mouse;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Distance;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, unit, Projectile.height, ref point)
                && Hits < 2)
            {
                Hits += 1;
                return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Color ColorFunction(float progress)
            {
                Color color = Color.Lerp(Color.Firebrick, Color.Orange, progress);
                color.A = (byte)(Projectile.timeLeft / 20f * Byte.MaxValue);

                return color;
            }

            float WidthFunction(float progress) => MathHelper.Lerp(30f, 5f, 1 - Projectile.timeLeft / 20f);

            GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/FuzzyLaser", AssetRequestMode.ImmediateLoad));
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(WidthFunction, ColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 30);

            return false;
        }
    }
}