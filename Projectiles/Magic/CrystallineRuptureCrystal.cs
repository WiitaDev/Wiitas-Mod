using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Particles;
using WiitaMod.Systems;
using WiitaMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;

namespace WiitaMod.Projectiles.Magic
{
    public class CrystallineRuptureCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600 * 2;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;
            DrawOriginOffsetY = -4;
            DrawOffsetX = -42;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero) 
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                Particle particle = new GlowOrbParticle(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), Main.rand.NextVector2Circular(1f, 1f), false, 30, Main.rand.NextFloat(0.5f, 0.7f), new Vector2(0.75f, 1f), Color.Lerp(Color.DodgerBlue, Color.Aquamarine, Main.rand.NextFloat()), true);
                ParticleManager.SpawnParticle(particle);
            }
            else 
            {
                Projectile.timeLeft -= 5;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += oldVelocity;
            Projectile.velocity = Vector2.Zero;
            Projectile.friendly = false;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.damage *= 2;

            ProjectileHelper.Explode(Projectile.whoAmI, 256, 256, false, false);

            for (int i = 0; i < 45; i++)
            {
                Particle particle = new GlowOrbParticle(Projectile.Center + Main.rand.NextVector2Circular(128f, 128f), Main.rand.NextVector2Circular(2f, 2f), false, 40, Main.rand.NextFloat(0.8f, 1f), new Vector2(0.75f, 1f), Color.Lerp(Color.DodgerBlue, Color.Aquamarine, Main.rand.NextFloat()), true);
                ParticleManager.SpawnParticle(particle);

                Particle particle2 = new GlowOrbParticle(Projectile.Center + Main.rand.NextVector2Circular(48f, 48f), Main.rand.NextVector2Circular(9f, 9f), false, 60, Main.rand.NextFloat(4f, 4.5f), new Vector2(0.75f, 1f), Color.Lerp(Color.DodgerBlue, Color.Aquamarine, Main.rand.NextFloat()), true);
                ParticleManager.SpawnParticle(particle2);
            }
            for (int i = 0; i < 35; i++)
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(80f, 80f);
                Particle particle = new SmokeParticle(Projectile.Center + offset, offset * 0.1f, Color.DodgerBlue, 30, Main.rand.NextFloat(0.4f, 0.5f), 0.7f, Main.rand.NextFloat(-0.1f, 0.1f), true);
                ParticleManager.SpawnParticle(particle);
            }

            SoundEngine.PlaySound(SoundID.Item122.WithVolumeScale(0.75f).WithPitchOffset(0.1f), Projectile.Center);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void PostDraw(Color lightColor) 
        {
            if (Projectile.velocity != Vector2.Zero) return;

            float progress = 1 - (Projectile.timeLeft / 1200f);

            // Normal pulse.
            float pulseSpeed = MathHelper.Lerp(2f, 8f, progress);
            float pulse = (float)(Math.Sin(Projectile.timeLeft * pulseSpeed * 0.01f) * 0.5f + 0.5f);

            // Gets stronger as the explosion approaches.
            float intensity = MathHelper.Lerp(0.7f, 2.4f, progress);
            float scale = MathHelper.Lerp(1f, 1.45f, pulse) * intensity;
            Lighting.AddLight(Projectile.Center, Color.DodgerBlue.ToVector3() * scale * 0.25f);


            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;
            Color glowColor = Color.DodgerBlue;
            glowColor.A = 0;
            glowColor *= intensity;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), glowColor, Projectile.rotation, glow.Size() / 2, scale, SpriteEffects.None);

        }

    }
}