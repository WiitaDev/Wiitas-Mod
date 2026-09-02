using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Projectiles.Magic
{
    public class CrystallineRuptureHold : ModProjectile
    {
        // The maximum charge value
        private const float MAX_CHARGE = 80f;

        public Player Owner => Main.player[Projectile.owner];
        public ref float Charge => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 28;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 69420;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }

        public override void AI()
        {
            Projectile.Center = Owner.MountedCenter + Vector2.UnitX * Owner.direction * 12f;

            bool channeling = Owner.channel && !Owner.noItems && !Owner.CCed && Owner.CheckMana(Owner.HeldItem, -1, false, false);

            if (!channeling && Main.myPlayer == Owner.whoAmI)
            {
                Projectile.Kill();
                return;
            }

            // Charging sound
            if (Charge % 8 == 0) 
            {
                SoundEngine.PlaySound(SoundID.Item15.WithVolumeScale(0.55f).WithPitchOffset(-0.8f + Charge * 0.01f) with { MaxInstances = 0 }, Projectile.Center);    
            }

            // Shoot when at max charge
            if (Charge >= MAX_CHARGE)
            {
                SoundEngine.PlaySound(SoundID.Item101.WithVolumeScale(0.85f).WithPitchOffset(0.25f) with { PitchVariance = 0.1f }, Projectile.Center);
                for (int i = 0; i < 20; i++) 
                {
                    Particle particle = new GlowOrbParticle(Owner.MountedCenter + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 32f, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 6f + Main.rand.NextVector2Circular(2f, 2f), false, Main.rand.Next(30,41), Main.rand.NextFloat(0.8f, 1f), new Vector2(0.75f, 1f), Color.Lerp(Color.DodgerBlue, Color.Aquamarine, Main.rand.NextFloat()), true);
                    ParticleManager.SpawnParticle(particle);
                }

                if (Main.myPlayer == Projectile.owner && Owner.CheckMana(Owner.HeldItem, -1, true, false))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.MountedCenter + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 32f, Projectile.velocity * Owner.HeldItem.shootSpeed, ModContent.ProjectileType<CrystallineRuptureCrystal>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);         
                }
                Charge = 0f;
            }

            Charge++;

            UpdatePlayer();
        }

        private void UpdatePlayer()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - Owner.Center);
                Projectile.velocity = aim;
            }

            Projectile.spriteDirection = Projectile.direction;
            Owner.ChangeDir(Projectile.spriteDirection); // Set player direction to where we are shooting
            Owner.heldProj = Projectile.whoAmI; // Update player's held projectile
            Owner.itemTime = 2; // Set item time to 2 frames while we are used
            Owner.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            Owner.itemRotation = 0f;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = -1;
            player.channel = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float progress = MathHelper.Min(Charge / (MAX_CHARGE - 20), 1f);
            Vector2 drawPosition = Owner.MountedCenter + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 32f;

            Lighting.AddLight(Projectile.Center, Color.DodgerBlue.ToVector3() * progress);

            Vector2 offset = Main.rand.NextVector2CircularEdge(32f, 32f);
            Particle chargePart = new GlowOrbParticle(drawPosition + offset * ((1 - progress) * 0.5f), -offset * 0.05f * progress, false, 20, Main.rand.NextFloat(0.2f, 0.3f) + progress * 0.1f, new Vector2(1f, 1f), Color.Lerp(Color.DodgerBlue, Color.Aquamarine, Main.rand.NextFloat()), true);
            ParticleManager.SpawnParticle(chargePart);


            Texture2D magicCircle = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/MagicCircle", AssetRequestMode.ImmediateLoad).Value;

            float aimRotation = Projectile.velocity.SafeNormalize(Vector2.UnitX).ToRotation();

            Color color = Color.DodgerBlue;
            color.A = 0;
            color *= progress * 0.5f;

            Main.EntitySpriteDraw(magicCircle, drawPosition - Main.screenPosition, null, color, aimRotation, magicCircle.Size() / 2, progress * new Vector2(0.6f, 1f), SpriteEffects.None, 0);

            if (Charge == MAX_CHARGE)
            {
                MagicCircleFadeParticle particle = new MagicCircleFadeParticle(drawPosition, aimRotation);
                ParticleManager.SpawnParticle(particle);
            }

            return true;
        }
    }
}