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

namespace WiitaMod.Projectiles.Ranger.FlameBlaster
{
    public class PiercingPegshot : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public const float maxTimeLeft = 20f;

        public Vector2 startPoint;

        public List<Vector2> points;
        public List<Vector2> velocities;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(startPoint);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            startPoint = reader.ReadVector2();
        }


        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = 9999;
            Projectile.extraUpdates = 200;
            Projectile.timeLeft = (int)maxTimeLeft * Projectile.extraUpdates;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter + new Vector2(0, -10) + Projectile.velocity * 9.5f; // the vector offset is the itemholdout y offset
            startPoint = Projectile.Center;

            for (int i = 0; i < 8; i++)
            {
                MistParticle mistParticle = new MistParticle(Projectile.Center, Projectile.velocity * 0.5f + Main.rand.NextVector2Circular(4f, 4f), Color.Orange, Color.WhiteSmoke, 0.33f, 255, MathHelper.ToRadians(2f));
                ParticleManager.SpawnParticle(mistParticle);
                SmokeParticle smokeParticle = new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 90, 0.2f, 0.70f, MathHelper.ToRadians(2), true);
                ParticleManager.SpawnParticle(smokeParticle);
            }
        }


        public override void AI()
        {
            if (Projectile.timeLeft == maxTimeLeft * Projectile.extraUpdates) 
            {
                velocities = new List<Vector2>();
            }

            if (Projectile.timeLeft > (maxTimeLeft - 1f) * Projectile.extraUpdates)
            {

                points = new BezierCurve([startPoint, Projectile.Center]).GetPoints(10);

                if (Projectile.penetrate <= Projectile.maxPenetrate - 2)
                {
                    if (Projectile.velocity != Vector2.Zero)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            MistParticle mistParticle = new MistParticle(points[^1], Projectile.velocity * 0.5f + Main.rand.NextVector2Circular(4f, 4f), Color.OrangeRed, Color.WhiteSmoke, 0.25f, 255, MathHelper.ToRadians(2f));
                            ParticleManager.SpawnParticle(mistParticle);
                            SmokeParticle smokeParticle = new SmokeParticle(points[^1], Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 90, 0.2f, 0.70f, MathHelper.ToRadians(2), true);
                            ParticleManager.SpawnParticle(smokeParticle);
                        }
                    }

                    Projectile.position += Projectile.velocity;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.friendly = false;
                }
                points.Add(Projectile.Center); // Add the precise end point

            }
            else
            {
                if (Projectile.velocity != Vector2.Zero)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        MistParticle mistParticle = new MistParticle(points[^1], Projectile.velocity * 0.5f + Main.rand.NextVector2Circular(4f, 4f), Color.OrangeRed, Color.WhiteSmoke, 0.25f, 255, MathHelper.ToRadians(2f));
                        ParticleManager.SpawnParticle(mistParticle);
                        SmokeParticle smokeParticle = new SmokeParticle(points[^1], Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 90, 0.2f, 0.70f, MathHelper.ToRadians(2), true);
                        ParticleManager.SpawnParticle(smokeParticle);
                    }
                }


                Projectile.position += Projectile.velocity;
                Projectile.velocity = Vector2.Zero;


                Projectile.friendly = false;

                if (Projectile.timeLeft % Projectile.extraUpdates == 0)
                {
                    if (velocities.Count == 0)
                    {
                        for (int i = 0; i < points.Count; i++)
                        {
                            velocities.Add(startPoint.DirectionTo(Projectile.Center).RotatedByRandom(1.5f) * Main.rand.NextFloat(0.0002f, 0.0005f) * Vector2.Distance(startPoint, Projectile.Center));
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

            for (int i = 0; i < 7; i++)
            {
                MistParticle mistParticle = new MistParticle(points[^1], Projectile.velocity * 0.5f + Main.rand.NextVector2Circular(4f, 4f), Color.OrangeRed, Color.WhiteSmoke, 0.25f, 255, MathHelper.ToRadians(2f));
                ParticleManager.SpawnParticle(mistParticle);
                SmokeParticle smokeParticle = new SmokeParticle(points[^1], Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 90, 0.2f, 0.70f, MathHelper.ToRadians(2), true);
                ParticleManager.SpawnParticle(smokeParticle);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Lighting.AddLight(points[i], Color.Orange.ToVector3() * Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));
            }


            Color OuterColorFunction(float progress)
            {
                //Color color = Color.Lerp(Color.Orange, Color.Firebrick, progress - 0.5f);
                Color color = Color.OrangeRed;
                color.A = (byte)MathF.Max(100, Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates) * byte.MaxValue);
                return color;
            }

            Color InnerColorFunction(float progress)
            {
                //Color color = Color.Lerp(Color.Orange, Color.Firebrick, progress - 0.5f);
                Color color = Color.Yellow;
                color.A = (byte)MathF.Max(100, Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates) * byte.MaxValue);
                return color;
            }

            float OuterWidthFunction(float progress) => MathHelper.Lerp(22f, 1f, 1 - Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));

            float InnerWidthFunction(float progress) => MathHelper.Lerp(17f, 1f, 1 - Projectile.timeLeft / (maxTimeLeft * Projectile.extraUpdates));


            // Stack two trails because I don't know how to make a cool laser
            GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/FireLaser", AssetRequestMode.ImmediateLoad));
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(OuterWidthFunction, OuterColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 30);

            GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/FuzzyLaser", AssetRequestMode.ImmediateLoad));
            PrimitiveRenderer.RenderTrail(points, new PrimitiveSettings(InnerWidthFunction, InnerColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 30);


            return false;
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++)
            {
                SmokeParticle smokeParticle = new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 90, 0.2f, 0.70f, MathHelper.ToRadians(2), true);
                ParticleManager.SpawnParticle(smokeParticle);
            }
            target.AddBuff(BuffID.OnFire3, 150);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            for (int i = 0; i < 5; i++)
            {
                SmokeParticle smokeParticle = new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), Color.Orange, 90, 0.2f, 0.70f, MathHelper.ToRadians(2), true);
                ParticleManager.SpawnParticle(smokeParticle);
            }
        }
    }
}