using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Helpers;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Particles;
using WiitaMod.Systems;
using System.Collections;
using Terraria.ModLoader.IO;

namespace WiitaMod.Projectiles.Summon
{
    public class CrystalWhipProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            Projectile.DefaultToWhip();

            // use these to change from the vanilla defaults
            Projectile.WhipSettings.Segments = 12;
            //Projectile.WhipSettings.RangeMultiplier = 1.4f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI; // Apply the targeting focus on the NPC who was hit.
            Projectile.damage = (int)(Projectile.damage * 0.70f); // Multihit penalty. Decrease the damage the more enemies the whip hits.

            // Crystal shard damage buff effect
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = ModContent.GetInstance<WiitaMod>().GetPacket();

                packet.Write((byte)0);
                packet.Write((byte)target.whoAmI);

                packet.Send();
            }
            else
            {
                ModGlobalNPC crystal = target.GetGlobalNPC<ModGlobalNPC>();

                crystal.shardStacks = Math.Min(4, crystal.shardStacks + 1);
                crystal.UpdateShards(target);
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);

            float swingProgress = Timer / timeToFlyOut;
            // This code limits dust to only spawn during the the actual swing.
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));

                // After choosing a randomized dust and a whip segment to spawn from, dust is spawned.
                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, DustID.BlueTorch, 0f, 0f, 70, Color.White, 1.1f);
                dust.position = points[pointIndex];
                dust.fadeIn = 0.3f;
                Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                // This math causes these dust to spawn with a velocity perpendicular to the direction of the whip segments, giving the impression of the dust flying off like sparks.
                dust.velocity += spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;

                Particle particle = new GlowOrbParticle(points[pointIndex], Main.rand.NextVector2Circular(2,2), false, 40, Main.rand.NextFloat(0.35f, 0.45f), new Vector2(0.75f, 1f), Color.DodgerBlue, true);

                ParticleManager.SpawnParticle(particle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int totalSegments = Projectile.WhipSettings.Segments; // The number of segments this whip has.

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, 5);
            int frameHeight = frame.Height;
            frame.Height -= 2;
            Vector2 originalOrigin = frame.Size() / 2f;
            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 origin = originalOrigin;
                float scale = 1f;

                // Handle
                if (i == 0)
                {
                    origin.Y -= 4f; // This will move where the handle is drawn so it can be more in the player's hand.
                }
                // Divide the middle of the whip (after the handle and before the head) by approximately 3 and use the middle segments in each third.
                else
                {
                    // First Segment
                    // At the start of the whip after the handle, the first segment is used.
                    int segmentToDraw = 1;

                    if (i > totalSegments / 3)
                    { // At 1/3 of the way across the whip, the second segment is used.
                      // Second Segment
                        segmentToDraw = 2;
                    }

                    if (i > 2 * (totalSegments / 3))
                    { // At 2/3 of the way across the whip, the third segment is used.
                      // Third segment
                        segmentToDraw = 3;
                    }

                    frame.Y = frameHeight * segmentToDraw; // Set the frame to the correct segment.
                }

                if (i == list.Count - 2)
                {
                    // This is the head of the whip.
                    frame.Y = frameHeight * 4;
                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.25f, Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true));
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }

            return false;
        }
    }
}