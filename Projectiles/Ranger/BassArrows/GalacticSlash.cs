using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Ranger.BassArrows
{
    public class GalacticSlash : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{977}";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic slash");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 20 * Projectile.MaxUpdates;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.PrincessWeapon, new ParticleOrchestraSettings
            {

                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity
            });
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Vector2.Zero
            });
        }
        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.DungeonWater, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default(Color), 0.9f);
                dust.noGravity = true;
                dust.position = Projectile.Center;
                dust.velocity = Main.rand.NextVector2Circular(1f, 1f) + Projectile.velocity * 0.5f;
            }

            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 0.75f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.UltimaPurpleTrail).Draw(Projectile);

            return true;
        }

    }
}