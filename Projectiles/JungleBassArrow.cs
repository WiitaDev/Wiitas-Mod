using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles
{
	public class JungleBassArrow : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spore Bass Arrow");
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = 1;
			Projectile.knockBack = 2f;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.friendly = true;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;

			int dust = Dust.NewDust(Projectile.Center, 1, 1, 40, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dust].scale = (float)Main.rand.Next(100, 135) * 0.013f;
			Main.dust[dust].noGravity = true;

		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Poisoned, 240);
		}

		public override void Kill(int timeLeft)
		{
			Player Owner = Main.player[Projectile.owner];
			if (Main.myPlayer == Owner.whoAmI)
			{
				for (int i = 0; i < Main.rand.Next(1, 4); i++)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2)), ProjectileID.SporeCloud, Projectile.damage / 2, 0, Main.myPlayer);
				}
			}

			for (int i = 0; i < 10; i++)
			{
				int dustHit = Dust.NewDust(Projectile.Center, 1, 1, 2, (Main.rand.Next(-2, 3)), (Main.rand.Next(-2, 3)), 0, default(Color), 1f);
				Main.dust[dustHit].scale = (float)Main.rand.Next(135, 160) * 0.013f;
				Main.dust[dustHit].noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.Grass.WithVolumeScale(0.75f).WithPitchOffset(-0.75f), Projectile.Center);
		}
	}
}