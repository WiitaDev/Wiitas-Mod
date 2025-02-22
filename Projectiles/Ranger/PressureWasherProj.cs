using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Projectiles.Ranger
{
    public class PressureWasherProj : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];
        public ref float Distance => ref Projectile.ai[2];

        public const float MaxDistance = 650f;

        public List<Vector2> points;
        public List<Projectile> pathProjectiles;

        public Vector2 endPoint;

        //public ref Player Owner => ref Main.player[Projectile.owner];
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 69420;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.extraUpdates = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            UpdatePlayer(player);
            Projectile.Center = player.MountedCenter + Projectile.velocity * 76.5f;

            if (!player.channel)
            {
                Projectile.Kill();
                return;
            }

            if (Main.myPlayer == Projectile.owner && (Time % (1 + Projectile.extraUpdates) == 0 || Time <= (1 + Projectile.extraUpdates)))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity * 35, Projectile.velocity * 35, ModContent.ProjectileType<PressureWasherPathProj>(), Projectile.damage, 0, Projectile.owner, Time % 5);

            if (Time > 0)
            {
                endPoint = Projectile.Center;
                FindEndpoint();

                points = new List<Vector2>();
                pathProjectiles = new List<Projectile>();

                //points.Add(Projectile.Center);

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<PressureWasherPathProj>())
                    {
                        pathProjectiles.Add(proj);
                    }
                }
                pathProjectiles = pathProjectiles.OrderBy(proj => proj.ai[0]).ToList();

                List<Vector2> bezierPoints = new List<Vector2>();
                foreach (Projectile proj in pathProjectiles)
                {
                    bezierPoints.Add(proj.Center);
                }
                //bezierPoints.Add(endPoint);
                points = new BezierCurve(bezierPoints).GetPoints(pathProjectiles.Count * 2);
                endPoint = points[^1];

                //points.Add(endPoint);


                for (int i = 0; i < 3; i++)
                {
                    Dust dustS = Main.dust[Dust.NewDust(Projectile.position, 20, 20, DustID.SteampunkSteam, Main.rand.Next(-2, 3) + Projectile.velocity.X * 3, Main.rand.Next(-2, 3) + Projectile.velocity.Y * 3)];
                    dustS.noGravity = true;
                    dustS.noLightEmittence = true;
                    dustS.noLight = true;
                    dustS.alpha = Main.rand.Next(100, 245);
                }

                if (Time % 40 == 0 || Time == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item13.WithPitchOffset(Main.rand.NextFloat(0.25f, 0.35f)), player.Center);
                }
            }


            Time++;
            Projectile.localAI[0] = Time;
        }

        private void UpdatePlayer(Player player)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
                if (aim.HasNaNs())
                {
                    aim = -Vector2.UnitY;
                }

                aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.15f)); // last variable is the turn speed
                aim *= 1f;

                if (aim != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = aim;
                Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                Projectile.netUpdate = true;
            }
            int dir = Projectile.direction;
            player.ChangeDir(dir); // Set player direction to where we are shooting
            player.heldProj = Projectile.whoAmI; // Update player's held projectile
            player.itemTime = 2; // Set item time to 2 frames while we are used
            player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * dir, Projectile.velocity.X * dir); // Set the item rotation to where we are shooting
        }

        private void FindEndpoint()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Player player = Main.player[Projectile.owner];

                int samplePointCount = 5;
                float[] laserLengthSamplePoints = new float[samplePointCount];
                Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.scale, MaxDistance, laserLengthSamplePoints);
                float tileDistance = laserLengthSamplePoints.Average();

                float closestDistance = tileDistance;
                float point = 10;
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC target = Main.npc[k];
                    if (target.active)
                    {
                        if (Collision.CheckAABBvLineCollision(target.position, target.Hitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Distance, 10, ref point))
                        {
                            float distance = Vector2.Distance(Projectile.Center, target.Center);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                            }
                        }
                    }
                }

                if (Distance >= closestDistance) Distance = closestDistance;
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (Distance >= closestDistance)
                        {
                            Distance = closestDistance;
                            break;
                        }
                        Distance += 1f;
                    }
                }

                if (Distance > MaxDistance) Distance = MaxDistance;
                Vector2 end = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Distance;

                endPoint = end;
            }
        }

        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle hitbox = new Rectangle((int)Projectile.Center.X - 5, (int)Projectile.Center.Y - 5, 10, 10);

            if (Time > 2)
            {
                Rectangle endpointHitbox = new Rectangle((int)endPoint.X - 5, (int)endPoint.Y - 5, 10, 10);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 center = Vector2.Lerp(points[i], points[i + 1], 0.5f);
                    hitbox.Location = (center - hitbox.Size() * 0.5f).ToPoint();

                    if (targetHitbox.Intersects(hitbox) || targetHitbox.Intersects(endpointHitbox))
                    {
                        return true;
                    }
                }
            }

            return false;
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time > 1)
            {
                Player player = Main.player[Projectile.owner];

                Texture2D texture = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Trail_1", AssetRequestMode.ImmediateLoad).Value;
                Texture2D noise = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/PerlinNoise", AssetRequestMode.ImmediateLoad).Value;
                Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;
                VertexStrip strip = new VertexStrip();

                Color color = Color.CadetBlue.MultiplyRGBA(lightColor);
                color.A = 125;
                Color endpointColor = Color.CadetBlue.MultiplyRGBA(new Color(Lighting.GetSubLight(endPoint)));
                endpointColor.A = 125;


                Color StripColor(float progress) => Color.Lerp(color, endpointColor, progress * 2) * Math.Clamp(0.5f - (Distance / MaxDistance / 2) + (0.5f - progress), 0.1f, 0.5f); // good luck trying to understand this :(

                float StripWidth(float progress) => (2f + progress * pathProjectiles[^1].ai[0]) * 6f;

                Vector2[] position = new Vector2[points.Count];
                float[] rotation = new float[points.Count];

                for (int i = 0; i < position.Length; i++)
                {
                    //if (i <= 1 || points[i].Distance(player.MountedCenter) > points[i - 1].Distance(player.MountedCenter))
                        position[i] = points[i];
                }

                for (int i = 0; i < position.Length; i++)
                    rotation[i] = Projectile.AngleTo(endPoint);

                rotation[position.Length - 1] = Projectile.AngleTo(endPoint);

                strip.PrepareStrip(position, rotation, StripColor, StripWidth, -Main.screenPosition, position.Length * 2, true);

                Effect effect = ModContent.Request<Effect>("WiitaMod/Effects/WaterStreamEffect", AssetRequestMode.ImmediateLoad).Value;
                effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["uTexture"].SetValue(texture);
                effect.Parameters["uNoise"].SetValue(noise);
                effect.Parameters["uFlowSpeed"].SetValue(new Vector2(-4, 0f));
                effect.Parameters["uDistortionStrength"].SetValue(0.0f);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects / 60f);
                effect.CurrentTechnique.Passes[0].Apply();

                strip.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                //Main.EntitySpriteDraw(glow, endPoint - Main.screenPosition, glow.Frame(), endpointColor, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 0.4f, 0, 0);

                foreach (Vector2 point in points) 
                {
                    if (Main.rand.NextBool(21))
                    {
                        Color pointColor = new Color(Lighting.GetSubLight(point));
                        MistParticle particle = new MistParticle(point + Main.rand.NextVector2Square(-10, 11), Main.rand.NextVector2Circular(4f, 4f), pointColor, pointColor, Main.rand.NextFloat(0.15f, 0.35f), 255 - Main.rand.Next(50, 125), MathHelper.ToRadians(2));
                        ParticleManager.SpawnParticle(particle);
                    }

                    if(point == points[^1]) 
                    {
                        Color pointColor = new Color(Lighting.GetSubLight(point));
                        MistParticle particle = new MistParticle(point + Main.rand.NextVector2Square(-10, 11), Main.rand.NextVector2Circular(5f, 5f), pointColor, pointColor, Main.rand.NextFloat(0.15f, 0.35f), 255 - Main.rand.Next(50, 125), MathHelper.ToRadians(2));
                        ParticleManager.SpawnParticle(particle);
                    }
                }


                SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.channel = false;
        }
    }

    public class PressureWasherPathProj : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public const float maxTimeleft = 18;

        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 18;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override bool ShouldUpdatePosition()
        {
            if(Projectile.timeLeft == maxTimeleft)
                return true;
            else
                return true;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            float multiplier = 1.5f + maxTimeleft / 10 - Projectile.timeLeft * 0.1f;
            hitbox = new Rectangle((int)(hitbox.X - (hitbox.Width / 2)), (int)(hitbox.Y - (hitbox.Height / 2)), (int)(hitbox.Width * multiplier), (int)(hitbox.Height * multiplier));
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += target.defense * 0.75f;
            if(Projectile.timeLeft >= maxTimeleft - 1) 
            {
                modifiers.FinalDamage *= 2f;
            }
            else 
            {
                modifiers.FinalDamage *= Projectile.timeLeft / maxTimeleft + 0.1f;
            }
        }

        public override void AI()
        {
            if (Projectile.timeLeft == maxTimeleft - 1)
            {

            }
            else
            {
                Projectile.tileCollide = true;
            }

            if (Main.player[Projectile.owner].channel == false)
            {
                Projectile.Kill();
            }

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.active)
                {
                    float multiplier = 1.4f + maxTimeleft / 10 - Projectile.timeLeft * 0.1f;
                    Rectangle hitbox = new Rectangle((int)(Projectile.Hitbox.X - multiplier), (int)(Projectile.Hitbox.Y - multiplier), (int)(Projectile.Hitbox.Width * multiplier), (int)(Projectile.Hitbox.Height * multiplier));
                    if (Projectile.Colliding(hitbox, target.Hitbox) && Projectile.timeLeft <= maxTimeleft)
                    {
                        if(Projectile.timeLeft > 2)
                            Projectile.timeLeft = 2;
                    }
                }
            }

            if (Projectile.timeLeft <= 2)
            {
                Projectile.damage = 0;
                Projectile.friendly = false;
                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
            }

            Time++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 2)
                Projectile.timeLeft = 2;

            Projectile.velocity = Vector2.Zero;
            Projectile.position += oldVelocity;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1)
            {
                //MistParticle particle = new MistParticle(Projectile.position + Main.rand.NextVector2Square(-10, 11), Main.rand.NextVector2Circular(4f, 4f), lightColor, lightColor, Main.rand.NextFloat(0.15f, 0.35f) + (24 - Projectile.timeLeft) * 0.05f, 255 - Main.rand.Next(25, 125 - Projectile.timeLeft), MathHelper.ToRadians(2));
                //ParticleManager.SpawnParticle(particle);
            }
            return true;
        }

    }
}