using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.BassArrows
{
    public class GalacticBassArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Bass Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;        //The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = 1;
            Projectile.knockBack = 2f;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 100)
            {
                Projectile.spriteDirection = -1;
            }
            else
            {
                Projectile.spriteDirection = 1;
            }
            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.GalacticBassTrail).Draw(Projectile);

            return true;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 circle = Main.rand.NextVector2Circular(2f, 2f);
                int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.PurpleCrystalShard, circle.X, circle.X, 0, default(Color), 1f);
                Main.dust[dustHit].scale = (float)Main.rand.Next(135, 190) * 0.013f;
                Main.dust[dustHit].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item109.WithPitchOffset(-0.5f), Projectile.Center);
        }
    }
}