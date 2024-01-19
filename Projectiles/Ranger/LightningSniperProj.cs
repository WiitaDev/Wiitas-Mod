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

namespace WiitaMod.Projectiles.Ranger.BassArrows
{
    public class LightningSniperProj : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];
        public ref float Distance => ref Projectile.ai[2];

        public List<Vector2> points;
        public List<Vector2> offsets;
        public List<Vector2> velocities;

        public Vector2 midPoint;
        public Vector2 endPoint;
        public override string Texture => $"Terraria/Images/Item_{ItemID.FlamingArrow}";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.manualDirectionChange = true;
            Projectile.extraUpdates = 2;
            Owner = -1;
        }

        public override void AI()
        {
            if (Owner < 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Main.projectile[(int)Owner].Center;

            if (Time < 1)
            {
                endPoint = Projectile.Center;
                FindEndpoint();

                Vector2 midOff = Main.rand.NextVector2Circular(10, 150).RotatedBy(Projectile.AngleTo(endPoint)) * (0.1f + Utils.GetLerpValue(0, 2000, Projectile.Distance(endPoint)));
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
                    velocities.Add(Projectile.DirectionTo(endPoint).RotatedByRandom(1.5f) * Main.rand.NextFloat(1f, 3f));
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
                        Dust sparkle = Dust.NewDustPerfect(points[i], DustID.PortalBolt, vel, 0, color, Main.rand.NextFloat(2f));
                        sparkle.noGravity = true;
                        sparkle.noLightEmittence = true;
                    }
                }
            }
            if (Time > 40)
                Projectile.Kill();

            Time++;
            Projectile.localAI[0] = Main.projectile[(int)Owner].ai[0];
            Projectile.rotation += 0.2f;
        }

        private void FindEndpoint()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 mouse;
                mouse = Projectile.Center + Projectile.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 1100;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                        if (Main.npc[i].Distance(Main.MouseWorld) < 150 && Main.npc[i].active && Projectile.FindTargetWithLineOfSight(1500) > 0)
                        {
                            endPoint = Main.rand.NextVector2FromRectangle(Main.npc[i].Hitbox);
                            Projectile.netUpdate = true;
                            return;
                        }
                }

                endPoint = mouse;
                Projectile.netUpdate = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle hitbox = new Rectangle((int)Projectile.Center.X - 30, (int)Projectile.Center.Y - 30, 60, 60);

            if (Time > 2 && Time < 30)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 center = Vector2.Lerp(points[i], points[i + 1], 0.5f);
                    hitbox.Location = (center - hitbox.Size() * 0.5f).ToPoint();

                    if (targetHitbox.Intersects(hitbox))
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
                VertexStrip strip = new VertexStrip();

                //Color StripColor(float progress) => Main.hslToRgb((Projectile.localAI[0] * 0.03f + progress) % 1f, 0.5f, 0.6f) * Utils.GetLerpValue(40, 10, Time, true);
                Color StripColor(float progress) => Color.White;
                float StripWidth(float progress) => (Utils.GetLerpValue(0.1f, 0f, progress, true) * 1.6f + progress * 3f) * 30f * MathF.Sqrt(Utils.GetLerpValue(0, 20, Time, true));

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
                lightningEffect.Parameters["uGlow"].SetValue(bloom);
                lightningEffect.Parameters["uColor"].SetValue(Vector3.One);
                lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.07f % 1f);
                lightningEffect.CurrentTechnique.Passes[0].Apply();

                strip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                Color endPointColor = Main.hslToRgb((Projectile.localAI[0] * 0.03f + 0.5f) % 1f, 0.5f, 0.5f, 0) * Utils.GetLerpValue(40, 25, Time, true);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), endPointColor * 0.6f, Projectile.rotation * 0.7f, bloom.Size() * 0.5f, Projectile.scale * 1.6f, 0, 0);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), endPointColor, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * 0.8f, 0, 0);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), endPointColor, Projectile.rotation * 1.1f, bloom.Size() * 0.5f, Projectile.scale * 0.6f, 0, 0);
                Main.EntitySpriteDraw(bloom, endPoint - Main.screenPosition, bloom.Frame(), new Color(255, 255, 255, 0) * Utils.GetLerpValue(40, 30, Time, true), Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * 0.5f, 0, 0);


                Texture2D texture2 = TextureAssets.Projectile[Type].Value;
                SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = MathF.Sqrt(Projectile.scale) * (1f + MathF.Sin(Time * 0.8f) * 0.1f) * 0.6f;

                Color whiteColor = Main.hslToRgb(Time * 0.01f % 1f, 0.5f, 0.7f, 0);

                Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, bloom.Frame(), whiteColor, Projectile.rotation * 0.5f, bloom.Size() * 0.5f, scale * 1.5f, 0, 0);
                Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, bloom.Frame(), whiteColor * 0.15f, Projectile.rotation * 0.5f, bloom.Size() * 0.5f, scale * 4f, 0, 0);
            }
            return false;
        }
    } 
    public class LightningSniperHold : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.FlamingArrow}";
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
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
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || Time >= 10)
            {
                Projectile.Kill();
                return;
            }

            bool canKill = false;

            Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            SetMagicHands();
            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * Owner.HeldItem.shootSpeed, 0.07f);
            Projectile.Center = Owner.MountedCenter + Projectile.velocity * (2f + 8f * Projectile.scale);

            if ((Time - 8) % 4 == 1)
            {

                for (int i = 0; i < 7; i++)
                {
                    Color color = Main.hslToRgb((Time + i) * 0.03f % 1f, 0.5f, 0.5f, 128);
                    Dust sparkle = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.PortalBolt, Projectile.velocity * Main.rand.NextFloat(2f), 0, color, 1f + Main.rand.NextFloat());
                    sparkle.noGravity = true;
                    sparkle.noLightEmittence = true;
                }
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(16, 16), Projectile.velocity, ModContent.ProjectileType<LightningSniperProj>(), Projectile.damage, 1f, Owner.whoAmI, ai1: Projectile.whoAmI);
            }

            //if ((Time - 8) % 5 == 1)
            //{
            //    Vector2 piercerVelocity = Projectile.velocity;

            //    if (Main.myPlayer == Projectile.owner)
            //    {
            //        piercerVelocity = (Main.MouseWorld - Projectile.Center).RotatedByRandom(0.2f) * (0.06f / MathHelper.E);
            //        Projectile.netUpdate = true;
            //    }
            //    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(10, 10), piercerVelocity, ModContent.ProjectileType<CrystalPiercer>(), Owner.HeldItem.damage, 1f, Owner.whoAmI, ai1: Projectile.whoAmI);
            //}

            if (!canKill)
                Projectile.timeLeft = 10000;

            if (canKill && Projectile.timeLeft > 5)
                Projectile.timeLeft = 5;

            if (Projectile.timeLeft < 10)
            {
                for (int i = 0; i < 3; i++)
                {
                    Color glowColor = Color.Aqua;
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(6, 6) * Projectile.scale, 0, glowColor, 2f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }

                for (int i = 0; i < 4; i++)
                {
                    Color glowColor = Main.hslToRgb((Time * 0.03f + i * 0.01f) % 1f, 0.5f, 0.5f, 128);
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowRod, Main.rand.NextVector2Circular(9, 9) * Projectile.scale, 0, glowColor, 2f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }
            }

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

        private void SetMagicHands()
        {
            float wobbleBack = -MathF.Sin(Time * 0.04f + 0.7f) * 0.2f - MathF.Sin(Time * 0.3f + 0.5f) * 0.04f;
            float offBack = -0.3f * Owner.direction;
            compArmRotBack = Projectile.velocity.ToRotation() - MathHelper.PiOver2 + offBack + wobbleBack;

            float wobbleFront = MathF.Sin(Time * 0.04f) * 0.2f + MathF.Sin(Time * 0.3f) * 0.04f;
            float offFront = 0.7f * Owner.direction;
            compArmRotFront = Projectile.velocity.ToRotation() - MathHelper.PiOver2 + offFront + wobbleFront;

            if (Time > 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Color glowColor = Color.Aqua;
                    Vector2 off = new Vector2(6f + MathF.Sin(Time * 0.5f - i * 0.02f) * 6f, 0).RotatedBy((Time - i / 2f) * 0.15f * Owner.direction) * Projectile.scale;
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center + off, DustID.PortalBoltTrail, off.SafeNormalize(Vector2.Zero) * 1.5f * MathF.Pow(Projectile.scale, 1.5f) + Owner.velocity, 0, glowColor, 1.5f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }

                for (int i = 0; i < 3; i++)
                {
                    Color glowColor = Main.hslToRgb((Time * 0.03f + i * 0.1f) % 1f, 0.5f, 0.5f, 128);
                    Vector2 off = new Vector2(15 + MathF.Sin(Time * 0.1f - i * MathHelper.TwoPi / 3f) * 12f, 0).RotatedBy((Time * 0.14f + i * MathHelper.PiOver2 / 5f) * (i % 2 == 1 ? (-1f) : 1f) * Owner.direction) * Projectile.scale;
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center + off, DustID.RainbowRod, off.SafeNormalize(Vector2.Zero) * MathF.Pow(Projectile.scale, 1.5f) + Owner.velocity, 0, glowColor, 1.1f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }
            }
        }

        /*public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/CircleGradient", AssetRequestMode.ImmediateLoad).Value;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float scale = MathF.Sqrt(Projectile.scale) * (1f + MathF.Sin(Time * 0.8f) * 0.1f) * 0.6f;

            Color rainbowColor = Main.hslToRgb(Time * 0.01f % 1f, 0.5f, 0.7f, 0);
            //Color rainbowColor = Color.White;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(rainbowColor.R, rainbowColor.G, rainbowColor.B), Projectile.rotation, texture.Size() * 0.5f, scale * 1.3f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation * 1.3f, texture.Size() * 0.5f, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor * 0.6f, Projectile.rotation * 0.7f, texture.Size() * 0.5f, scale * 1.2f, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor, Projectile.rotation * 0.5f, glow.Size() * 0.5f, scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor * 0.15f, Projectile.rotation * 0.5f, glow.Size() * 0.5f, scale * 4f, 0, 0);

            return false;
        }*/
    }
}