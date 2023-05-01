using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Ranger.BassArrows
{
	public class RockyMoltenBassArrow : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rocky Molten Bass Arrow");
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
		}

		public override void OnSpawn(IEntitySource source)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1f);
				Main.dust[dustHit].scale = (float)Main.rand.Next(135, 160) * 0.013f;
				Main.dust[dustHit].noGravity = true;
			}
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;

			int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dust].scale = (float)Main.rand.Next(100, 135) * 0.013f;
			Main.dust[dust].noGravity = true;

		}


		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Stone, (Main.rand.Next(-5, 5)), (Main.rand.Next(-5, 5)), 0, default(Color), 1f);
				Main.dust[dustHit].scale = (float)Main.rand.Next(135, 160) * 0.013f;
				Main.dust[dustHit].noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.Item70.WithVolumeScale(0.5f).WithPitchOffset(0.1f), Projectile.Center);
		}
	}
}