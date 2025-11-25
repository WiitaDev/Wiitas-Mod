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
using WiitaMod.Helpers;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Systems.Primitives;

namespace WiitaMod.Projectiles.Ranger.BassArrows
{
    public class HolyBassLaser : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public const float maxTimeLeft = 20f;

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

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter + Projectile.velocity * 6.5f; // the vector offset is the itemholdout offset
            startPoint = Projectile.Center;
        }


        public override void AI()
        {
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

            float WidthFunction(float progress) => MathHelper.Lerp(25f, 1f, 1 - Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));


            GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/FuzzyLaser", AssetRequestMode.ImmediateLoad));
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(WidthFunction, ColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 30);


            return false;
        }

    }
}