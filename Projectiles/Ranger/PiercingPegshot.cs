using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using WiitaMod.Systems.Primitives;

namespace WiitaMod.Projectiles.Ranger
{
    public class PiercingPegshot : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public Vector2 startPoint;

        public List<Vector2> points;
        public List<Vector2> velocities;

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = 9999;
            Projectile.extraUpdates = 150;
            Projectile.timeLeft = 30 * Projectile.extraUpdates;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter + new Vector2(0, -10) + Projectile.velocity * 9.5f; // the vector offset is the itemholdout y offset
            startPoint = Projectile.Center;

            velocities = new List<Vector2>();
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 29 * Projectile.extraUpdates)
            {
                if (Projectile.penetrate <= Projectile.maxPenetrate - 2)
                {
                    Projectile.position += Projectile.velocity;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.friendly = false;
                }
                points = new BezierCurve([startPoint, Projectile.Center]).GetPoints(10);
                points.Add(Projectile.Center);
            }
            else
            {
                Projectile.position += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;


                Projectile.friendly = false;

                if (Projectile.timeLeft % Projectile.extraUpdates == 0)
                {
                    if (velocities.Count == 0)
                    {
                        for (int i = 0; i < points.Count; i++)
                        {
                            velocities.Add(startPoint.DirectionTo(Projectile.Center).RotatedByRandom(1.5f) * Main.rand.NextFloat(0.0005f, 0.001f) * Vector2.Distance(startPoint, Projectile.Center));
                        }
                    }
                    else
                    {
                        for (int i = 1; i < points.Count; i++)
                        {
                            velocities[i] *= Main.rand.NextFloat(0.98f, 1f);
                            points[i] += velocities[i];
                        }
                    }
                }


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

            Color ColorFunction(float progress)
            {
                Color color = Color.Lerp(Color.Orange, Color.Firebrick, progress);
                color.A = 150;
                return color;
            }

            float WidthFunction(float progress) => 30f;

            GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/FuzzyLaser", AssetRequestMode.ImmediateLoad));
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(WidthFunction, ColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 30);

            return false;
        }
    }
}