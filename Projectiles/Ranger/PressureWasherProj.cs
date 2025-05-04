using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Systems.Primitives;

namespace WiitaMod.Projectiles.Ranger
{
    public class PressureWasherProj : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];
        public ref float Distance => ref Projectile.ai[2];

        public List<Projectile> pathProjectiles;
        public List<List<Projectile>> connectedPaths;

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
            Projectile.extraUpdates = 0;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            UpdatePlayer(player);
            Projectile.Center = player.MountedCenter + new Vector2(0, -6) + Projectile.velocity * 76.5f; // the vector offset is the itemholdout y offset

            if (!player.channel)
            {
                if (Projectile.timeLeft > 19)
                {
                    Projectile.timeLeft = 19;

                    SoundStyle soundStyle = SoundID.Item13 with
                    {
                        MaxInstances = 0
                    };
                    SoundEngine.PlaySound(soundStyle.WithPitchOffset(Main.rand.NextFloat(0.25f, 0.35f)).WithVolumeScale(0.8f), player.Center);
                }

                return;
            }

            if (Main.myPlayer == Projectile.owner && player.channel)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Projectile.velocity * 35, Projectile.velocity * 35, ModContent.ProjectileType<PressureWasherPathProj>(), Projectile.damage, 0, Projectile.owner);

            if (Time > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust dustS = Main.dust[Dust.NewDust(Projectile.position, 20, 20, DustID.SteampunkSteam, Main.rand.Next(-2, 3) + Projectile.velocity.X * 3, Main.rand.Next(-2, 3) + Projectile.velocity.Y * 3)];
                    dustS.noGravity = true;
                    dustS.noLightEmittence = true;
                    dustS.noLight = true;
                    dustS.alpha = Main.rand.Next(100, 245);
                }

                if (Time % 8 == 0 || Time == 0)
                {
                    SoundStyle soundStyle = SoundID.Item13 with
                    {
                        MaxInstances = 0
                    };
                    SoundEngine.PlaySound(soundStyle.WithPitchOffset(Main.rand.NextFloat(0.25f, 0.35f)).WithVolumeScale(0.8f), player.Center);
                }
            }


            Time++;
            Projectile.localAI[0] = Time;
        }

        List<List<Projectile>> GetAllConnectedPaths(List<Projectile> projectiles)
        {
            // Sort projectiles by ai[0] for easier grouping
            List<Projectile> sorted = projectiles.OrderBy(p => p.ai[0]).ToList();
            List<List<Projectile>> connectedPaths = new List<List<Projectile>>();
            HashSet<Projectile> visited = new HashSet<Projectile>();

            foreach (Projectile proj in sorted)
            {
                if (!visited.Contains(proj))
                {
                    List<Projectile> currentPath = new List<Projectile>();
                    ExplorePath(proj, sorted, visited, currentPath);

                    if (currentPath.Count >= 2)
                    {
                        connectedPaths.Add(currentPath);
                    }
                }
            }

            return connectedPaths;
        }

        void ExplorePath(Projectile current, List<Projectile> sorted, HashSet<Projectile> visited, List<Projectile> path)
        {
            visited.Add(current);
            path.Add(current);

            // Find neighbors (previous and next in ai[0] sequence)
            int currentIndex = sorted.IndexOf(current);

            // Check previous projectile
            if (currentIndex > 0)
            {
                Projectile prev = sorted[currentIndex - 1];
                if (!visited.Contains(prev) && Math.Abs(prev.ai[0] - current.ai[0]) == 1)
                {
                    ExplorePath(prev, sorted, visited, path);
                }
            }

            // Check next projectile
            if (currentIndex < sorted.Count - 1)
            {
                Projectile next = sorted[currentIndex + 1];
                if (!visited.Contains(next) && Math.Abs(next.ai[0] - current.ai[0]) == 1)
                {
                    ExplorePath(next, sorted, visited, path);
                }
            }
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

                aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.3f)); // last variable is the turn speed
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

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time > 1)
            {
                pathProjectiles = new List<Projectile>();
                connectedPaths = new List<List<Projectile>>();

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<PressureWasherPathProj>() && proj.ai[0] != 18 * proj.extraUpdates)
                    {
                        pathProjectiles.Add(proj);
                    }
                }

                connectedPaths = GetAllConnectedPaths(pathProjectiles);

                foreach (List<Projectile> projectiles in connectedPaths)
                {
                    if (projectiles.Count < 2) break;

                    Player player = Main.player[Projectile.owner];


                    Color color = Color.CadetBlue.MultiplyRGBA(lightColor);
                    color.A = 125;
                    Color endpointColor = Color.CadetBlue.MultiplyRGBA(new Color(Lighting.GetSubLight(projectiles[^1].Center)));
                    endpointColor.A = 125;


                    Color ColorFunction(float progress) => Color.Lerp(color, endpointColor, progress) * Math.Clamp(0.5f - (projectiles[(int)(progress * projectiles.Count)].ai[0] / 16 / 2), 0.0f, 0.5f);

                    float WidthFunction(float progress) => (2f + projectiles[(int)(progress * projectiles.Count)].ai[0]) * 4.5f;

                    Vector2[] position = new Vector2[projectiles.Count];

                    for (int i = 0; i < position.Length; i++)
                    {
                        position[i] = projectiles[i].Center;
                    }

                    position = new BezierCurve(position.ToList()).GetPoints(projectiles.Count * 2).ToArray();

                    // render the water
                    GameShaders.Misc["WiitaMod:WaterStream"].SetShaderTexture(ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Trail_1", AssetRequestMode.ImmediateLoad));
                    PrimitiveRenderer.RenderTrail(position, new PrimitiveSettings(WidthFunction, ColorFunction, smoothen: true, shader: GameShaders.Misc["WiitaMod:WaterStream"]), 24);


                    Texture2D tip = ModContent.Request<Texture2D>("WiitaMod/Particles/Mist", AssetRequestMode.ImmediateLoad).Value;
                    endpointColor.A = 0;
                    Main.EntitySpriteDraw(tip, position[^1] - Main.screenPosition, tip.Frame(verticalFrames: 3, frameY: 1), endpointColor * (1 - (projectiles[^2].ai[0] / 18)) * 2, MathHelper.ToRadians(Main.rand.Next(0, 361)), new Vector2(tip.Width / 2, tip.Height / 2 / 3), projectiles[^1].ai[0] * 0.1f + Main.rand.NextFloat(0f, 0.2f), 0, 0);

                    foreach (Vector2 point in position)
                    {
                        float age = 0;
                        float closest = -1;
                        for (int i = 0; i < projectiles.Count - 1; i++)
                        {
                            if (projectiles[i].Distance(point) < closest || closest == -1)
                            {
                                closest = projectiles[i].Distance(point);
                                age = projectiles[i].ai[0];
                            }
                        }

                        if (Main.rand.NextBool(20 - (int)age))
                        {
                            Color pointColor = new Color(Lighting.GetSubLight(point));
                            MistParticle particle = new MistParticle(point + Main.rand.NextVector2Circular(10 + age, 10 + age), Main.rand.NextVector2Circular(4f + age / 10, 4f + age / 10), pointColor, pointColor, Main.rand.NextFloat(0.15f, 0.35f) + age * 0.02f, 255 - Main.rand.Next(75, 125) - age * 2, MathHelper.ToRadians(2));
                            ParticleManager.SpawnParticle(particle);
                        }
                    }


                    SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                }

            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.channel = false;
        }
    }
}