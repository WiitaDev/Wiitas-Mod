using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Net;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Dusts;
using WiitaMod.Systems.Primitives;

namespace WiitaMod.Projectiles.Ranger
{
    public class TestWeaponProj : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];
        public ref float Distance => ref Projectile.ai[2];

        public List<Vector2> points;

        public Vector2 midPoint;
        public Vector2 endPoint;

        //public ref Player Owner => ref Main.player[Projectile.owner];
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 200;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Center = Main.projectile[Main.player[(int)Owner].heldProj].Center;
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

                points.Add(Projectile.Center);

                points = new BezierCurve(new List<Vector2>()
                {
                    Projectile.Center,
                    midPoint,
                  
                }).GetPoints(Main.rand.Next(1, 4) + (int)(Projectile.Distance(endPoint) * 0.017f));
                points.Add(endPoint);

                for (int i = 0; i < points.Count; i++)
                {            
                    if (i > 0)
                    {
                        HomeToEnemies(i);

                    }
                }
            }

            if (Time > 1)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    float prog = Utils.GetLerpValue(1, points.Count, i, true) * Utils.GetLerpValue(points.Count - 1, 0, i, true) * 3f;
                }

                for (int i = 1; i < points.Count; i++)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 vel = (Projectile.DirectionTo(endPoint).SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f) * 0.05f) * Main.rand.NextFloat(2f);
                        Color color = Main.hslToRgb((Projectile.localAI[0] * 0.03f + i / (float)points.Count * 0.5f) % 1f, 0.5f, 0.5f, 0);
                        Dust sparkle = Dust.NewDustPerfect(points[i], 226, vel, 0, color, Main.rand.NextFloat(1.3f));
                        sparkle.noGravity = true;
                        sparkle.noLightEmittence = true;
                    }
                }
            }
            if (Time > 80)
                Projectile.Kill();

            Time++;
            Projectile.localAI[0] = Time;
            Projectile.rotation += 0.2f;
        }

        private void FindEndpoint()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 mouse = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 500;
                //mouse = Main.player[(int)Owner].MountedCenter + Projectile.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 1100;

                /*if (closestDistance < 150 && closestTarget != -1)
                {
                    endPoint = Main.rand.NextVector2FromRectangle(Main.npc[closestTarget].Hitbox);
                    return;
                }*/

                endPoint = mouse;
            }
        }


        private void HomeToEnemies(int i) 
        {
            float closestDistance = 1000;
            NPC closestTarget = null;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.active && target.Distance(points[i]) < closestDistance && target.CanBeChasedBy())
                {
                    closestTarget = target;
                    closestDistance = target.Distance(points[i]);
                }
            }
            if(closestDistance < 150 && closestTarget != null)
            points[i] = Main.rand.NextVector2FromRectangle(closestTarget.Hitbox);
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
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(target.type == NPCID.TheDestroyerBody) 
            {
                modifiers.FinalDamage *= 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time > 1)
            {
                Texture2D texture = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/GlowTrail", AssetRequestMode.ImmediateLoad).Value;
                Texture2D bloom = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/CircleGradient", AssetRequestMode.ImmediateLoad).Value;
                Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;

                //Color StripColor(float progress) => Main.hslToRgb((Projectile.localAI[0] * 0.03f + progress) % 1f, 0.5f, 0.6f) * Utils.GetLerpValue(40, 10, Time, true);
                Color StripColor(float progress)
                {
                    Color color = Color.White;
                    return color;
                }
                float StripWidth(float progress) => 50f;

                Vector2[] position = new Vector2[points.Count];

                for (int i = 0; i < position.Length; i++)
                    position[i] = points[i];


                MiscShaderData shader = GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/shitass_laser", AssetRequestMode.ImmediateLoad));
                PrimitiveRenderer.RenderTrail(position, new PrimitiveSettings(StripWidth, StripColor, smoothen: true, shader: shader), 30);
            }
            return false;
        }
    }

    public class TestWeaponHold : ModProjectile
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
            UpdatePlayer(Owner);
            Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * Owner.HeldItem.shootSpeed, 0.07f);
            Projectile.Center = Owner.MountedCenter + Projectile.velocity * (20f + 8f * Projectile.scale);

            if (Time == 0 && Owner.whoAmI == Main.myPlayer)
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(16, 16), Projectile.velocity, ModContent.ProjectileType<TestWeaponProj>(), Projectile.damage, 1f, Owner.whoAmI, ai1: Owner.whoAmI);


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
        private void UpdatePlayer(Player player)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
                if (aim.HasNaNs())
                {
                    aim = -Vector2.UnitY;
                }

                aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.6f)); // last variable is the turn speed
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


        public override bool? CanDamage()
        {
            return false;
        }

    }
}