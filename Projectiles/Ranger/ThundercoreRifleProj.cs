using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics;
using WiitaMod.Dusts;
using System.IO;
using Microsoft.CodeAnalysis;

namespace WiitaMod.Projectiles.Ranger
{
    public class LightningSniperProj : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        //public ref float Owner => ref Projectile.ai[1];
        public ref float Distance => ref Projectile.ai[2];

        public List<Vector2> points;
        public List<Vector2> offsets;
        public List<Vector2> velocities;

        public Vector2 midPoint;
        public Vector2 endPoint;

        public ref Player Owner => ref Main.player[Projectile.owner];
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Center = Main.projectile[Owner.heldProj].Center;
        }

        public override void AI()
        {          
            if (Time < 1)
            {
                endPoint = Projectile.Center;
                FindEndpoint();

                Vector2 midOff = Main.rand.NextVector2Circular(2, 100).RotatedBy(Projectile.AngleTo(endPoint)) * (0.1f + Utils.GetLerpValue(0, 2000, Projectile.Distance(endPoint)));
                midPoint = Vector2.Lerp(Projectile.Center, endPoint, 0.7f) + midOff;
            }

            if (Time == 1)
            {
                points = new List<Vector2>();
                offsets = new List<Vector2>();
                velocities = new List<Vector2>();

                points.Add(Projectile.Center);
                Vector2 mid0 = Vector2.Lerp(Projectile.Center, endPoint, 0.3f);
                Vector2 mid1 = Vector2.Lerp(Projectile.Center, endPoint + Main.rand.NextVector2CircularEdge(20, 400).RotatedBy(Projectile.AngleTo(endPoint)), 0.5f);
                Vector2 mid2 = Vector2.Lerp(Projectile.Center, endPoint, 0.9f);
                points = new BezierCurve(new List<Vector2>()
                {
                    Projectile.Center,
                    mid0,
                    mid1,
                    midPoint,
                    mid2

                }).GetPoints(Main.rand.Next(1, 4) + (int)(Projectile.Distance(endPoint) * 0.017f));
                points.Add(endPoint);

                for (int i = 0; i < points.Count; i++)
                {
                    offsets.Add(Main.rand.NextVector2Circular(2, 5).RotatedBy(Projectile.AngleTo(endPoint)) * Utils.GetLerpValue(1, points.Count * 0.3f, i, true) * Utils.GetLerpValue(points.Count - 1, points.Count * 0.7f, i, true));
                    velocities.Add(Projectile.DirectionTo(endPoint).RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 6f));
                }
            }

            if (Time > 1)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    float prog = Utils.GetLerpValue(1, points.Count, i, true) * Utils.GetLerpValue(points.Count - 1, 0, i, true) * 3f;
                    points[i] += (Projectile.position - Projectile.oldPosition) * (1f - i / (float)points.Count);
                    offsets[i] += (velocities[i] + Main.rand.NextVector2Circular(4, 4)) * prog;
                    velocities[i] *= Main.rand.NextFloat(0.95f, 1f) - i * 0.002f;
                }

                for (int i = 1; i < points.Count; i++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vel = (Projectile.DirectionTo(endPoint).SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f) + offsets[i] * 0.05f) * Main.rand.NextFloat(2f);
                        Color color = Main.hslToRgb((Projectile.localAI[0] * 0.03f + i / (float)points.Count * 0.5f) % 1f, 0.5f, 0.5f, 0);
                        Dust sparkle = Dust.NewDustPerfect(points[i], 226, vel, 0, color, Main.rand.NextFloat(1.3f));
                        sparkle.noGravity = true;
                        sparkle.noLightEmittence = true;
                    }
                }
            }
            if (Time > 40)
                Projectile.Kill();

            Time++;
            Projectile.localAI[0] = Time;
            Projectile.rotation += 0.2f;
        }

        private void FindEndpoint()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 mouse = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 1100;
                //mouse = Main.player[(int)Owner].MountedCenter + Projectile.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 1100;

                int closestTarget = -1;
                int closestDistance = 1000;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].Distance(Main.MouseWorld) < closestDistance)
                    {
                        closestTarget = i;
                        closestDistance = (int)Main.npc[i].Distance(Main.MouseWorld);
                    }
                }

                if(closestDistance < 150 && closestTarget != -1) 
                {
                    endPoint = Main.rand.NextVector2FromRectangle(Main.npc[closestTarget].Hitbox);
                    //Projectile.netUpdate = true;
                    return;
                }


                endPoint = mouse;
                //Projectile.netUpdate = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle hitbox = new Rectangle((int)Projectile.Center.X - 30, (int)Projectile.Center.Y - 30, 60, 60);

            if (Time > 2 && Time < 30)
            {
                Rectangle endpointHitbox = new Rectangle((int)endPoint.X - 30, (int)endPoint.Y - 30, 60, 60);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 center = Vector2.Lerp(points[i], points[i + 1], 0.5f);
                    hitbox.Location = (center - hitbox.Size() * 0.5f).ToPoint();

                    if (targetHitbox.Intersects(hitbox) || targetHitbox.Intersects(endpointHitbox))
                        return true;
                }
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time > 1)
            {
                Texture2D texture = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/GlowTrail", AssetRequestMode.ImmediateLoad).Value;
                Texture2D bloom = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/CircleGradient", AssetRequestMode.ImmediateLoad).Value;
                Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;
                VertexStrip strip = new VertexStrip();

                //Color StripColor(float progress) => Main.hslToRgb((Projectile.localAI[0] * 0.03f + progress) % 1f, 0.5f, 0.6f) * Utils.GetLerpValue(40, 10, Time, true);
                Color StripColor(float progress) => new Color(125, 249, 255, 0);
                float StripWidth(float progress) => (Utils.GetLerpValue(0.1f, 0f, progress, true) * 1.6f + progress * 3f) * 30f * MathF.Sqrt(Utils.GetLerpValue(20, 5, Time, true));

                Vector2[] position = new Vector2[points.Count];
                float[] rotation = new float[points.Count];

                for (int i = 0; i < position.Length; i++)
                    position[i] = points[i] + offsets[i];

                for (int i = 0; i < position.Length; i++)
                    rotation[i] = Projectile.AngleTo(endPoint);

                rotation[position.Length - 1] = Projectile.AngleTo(endPoint);

                strip.PrepareStrip(position, rotation, StripColor, StripWidth, -Main.screenPosition, position.Length * 2, true);

                Effect lightningEffect = ModContent.Request<Effect>("WiitaMod/Effects/CrystalLightningEffect", AssetRequestMode.ImmediateLoad).Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture"].SetValue(texture);
                lightningEffect.Parameters["uGlow"].SetValue(glow);
                lightningEffect.Parameters["uColor"].SetValue(Vector3.One);
                lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.07f % 1f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();

                strip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                Color whiteColor = new Color(125, 249, 255, 0);
                Color endPointColor = Main.hslToRgb((Projectile.localAI[0] * 0.03f + 0.5f) % 1f, 0.5f, 0.5f, 0) * Utils.GetLerpValue(40, 25, Time, true);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), whiteColor * 0.6f, Projectile.rotation * 0.7f, bloom.Size() * 0.5f, Projectile.scale * .5f, 0, 0);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), whiteColor, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * 0.8f, 0, 0);
                Main.EntitySpriteDraw(glow, endPoint - Main.screenPosition, glow.Frame(), whiteColor, Projectile.rotation * 1.1f, glow.Size() * 0.5f, Projectile.scale * 0.6f, 0, 0);
                Main.EntitySpriteDraw(glow, endPoint - Main.screenPosition, glow.Frame(), new Color(255, 255, 255, 0) * Utils.GetLerpValue(40, 30, Time, true), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 0.5f, 0, 0);

                SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = MathF.Sqrt(Projectile.scale) * (1f + MathF.Sin(Time * 0.8f) * 0.1f) * 0.6f;

                //Color whiteColor = Main.hslToRgb(Time * 0.01f % 1f, 0.5f, 0.7f, 0);

                Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, bloom.Frame(), whiteColor, Projectile.rotation * 0.5f, bloom.Size() * 0.5f, scale, 0, 0);
                Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, bloom.Frame(), whiteColor * 0.15f, Projectile.rotation * 0.5f, bloom.Size() * 0.5f, scale, 0, 0);
                Main.EntitySpriteDraw(glow, endPoint - Main.screenPosition, glow.Frame(), whiteColor, Projectile.rotation * 1.1f, glow.Size() * 0.5f, Projectile.scale * 0.6f, 0, 0);
                Main.EntitySpriteDraw(glow, endPoint - Main.screenPosition, glow.Frame(), new Color(255, 255, 255, 0) * Utils.GetLerpValue(40, 30, Time, true), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 0.5f, 0, 0);
            }
            return false;
        }
    } 
    public class LightningSniperHold : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 40;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public ref float Time => ref Projectile.ai[0];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void AI()
        {

            Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * Owner.HeldItem.shootSpeed, 0.07f);
            Projectile.Center = Owner.MountedCenter + Projectile.velocity * (20f + 8f * Projectile.scale);

            if(Time == 0)
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(16, 16), Projectile.velocity, ModContent.ProjectileType<LightningSniperProj>(), Projectile.damage, 1f, Owner.whoAmI, ai1: Projectile.whoAmI);


            Time++;

            float modRotB = compArmRotBack;
            float modRotF = compArmRotFront;
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, modRotB);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, modRotF);

            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(2, 30, Time, true) * Utils.GetLerpValue(0, 25, Projectile.timeLeft, true));
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation -= Owner.direction * 0.2f;
        }

        private float compArmRotBack;
        private float compArmRotFront;

        public override bool? CanDamage()
        {
            return false;
        }

    }
}