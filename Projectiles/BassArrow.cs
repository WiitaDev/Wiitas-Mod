using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles
{
	public class BassArrow : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bass Arrow");
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.damage = 8;
			Projectile.aiStyle = 1;
			Projectile.knockBack = 2f;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = 2;
			Projectile.friendly = true;
		}

		public override void OnSpawn(IEntitySource source)
		{
		}

		public override void AI()
		{
			if (Projectile.velocity.X >= 0) Projectile.spriteDirection = 1;
			else Projectile.spriteDirection = -1;
			int dust = Dust.NewDust(Projectile.Center, 1, 1, 101, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dust].scale = (float)Main.rand.Next(100, 135) * 0.013f;
			int dust2 = Dust.NewDust(Projectile.Center, 1, 1, 34, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dust2].scale = (float)Main.rand.Next(100, 135) * 0.013f;

		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			for (int i = 0; i < 5; i++)
			{
				int dustHit = Dust.NewDust(Projectile.Center, 1, 1, 5, (float)Main.rand.Next(-5, 5), (float)Main.rand.Next(5, 10), 0, default(Color), 1f);
				Main.dust[dustHit].scale = (float)Main.rand.Next(135, 160) * 0.013f;
				Main.dust[dustHit].noGravity = true;
			}
			target.AddBuff(BuffID.Stinky, 180);
			target.AddBuff(BuffID.Wet, 180);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustHit = Dust.NewDust(Projectile.Center, 1, 1, 5, (float)Main.rand.Next(-3, 3), 0, 0, default(Color), 1f);
				Main.dust[dustHit].scale = (float)Main.rand.Next(100, 135) * 0.013f;
			}
			SoundEngine.PlaySound(SoundID.NPCDeath1.WithVolumeScale(0.75f).WithPitchOffset(0.1f), Projectile.Center);
		}
	}
}