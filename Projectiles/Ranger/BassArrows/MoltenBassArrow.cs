using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Ranger.BassArrows
{
    public class MoltenBassArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Molten Bass Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;        //The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = 1;
            Projectile.knockBack = 2f;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1f);
                Main.dust[dustHit].scale = (float)Main.rand.Next(135, 160) * 0.013f;
                Main.dust[dustHit].noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;

            if (Projectile.wet)
            {
                SoundEngine.PlaySound(SoundID.LiquidsWaterLava.WithVolumeScale(0.9f).WithPitchOffset(0f), Projectile.Center);
                Projectile.active = false;
                for (int i = 0; i < 10; i++)
                {
                    int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1f);
                    Main.dust[dustHit].scale = (float)Main.rand.Next(135, 160) * 0.013f;
                    Main.dust[dustHit].noGravity = true;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<RockyMoltenBassArrow>(), 20, 0, Main.myPlayer);

            }
            else
            {
                Projectile.active = true;
            }

            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.78f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.FireTrail).Draw(Projectile);

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override void OnKill(int timeLeft)
        {
            Player Owner = Main.player[Projectile.owner];
            if (Main.myPlayer == Owner.whoAmI)
            {
                for (int i = 0; i < Main.rand.Next(3, 7); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(3, 5) * -1) + Projectile.velocity * 0.3f, Main.rand.Next(400, 403), Projectile.damage / 2, 0, Main.myPlayer);
                }
            }

            for (int i = 0; i < 30; i++)
            {
                int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, (Main.rand.Next(-5, 5)), (Main.rand.Next(-5, 5)), 0, default(Color), 1f);
                Main.dust[dustHit].scale = (float)Main.rand.Next(135, 180) * 0.013f;
                Main.dust[dustHit].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item14.WithVolumeScale(0.75f).WithPitchOffset(0.1f), Projectile.Center);
        }
    }
}