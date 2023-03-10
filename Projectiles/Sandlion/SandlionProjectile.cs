using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Sandlion
{
	public class SandlionProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandlion Rocket");
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 18;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = 0;
			Projectile.knockBack = 2f;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.friendly = true;
		}

		public override void OnSpawn(IEntitySource source)
		{
		}

		public override void AI()
		{
            Projectile.spriteDirection = -Projectile.direction;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);
			Dust.NewDust(Projectile.Center, 1, 1, DustID.Sand, (float)Main.rand.Next(-1, 2), (float)Main.rand.Next(-1, 2), 0, default, Main.rand.NextFloat(1f, 1.21f));
			for (int i = 0; i < 3; i++)
			{
				Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, 0, 0, 0, default, Main.rand.NextFloat(1f, 1.21f));
			}
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
        }

		public override void Kill(int timeLeft)
		{
            Player Owner = Main.player[Projectile.owner];
			for (int i = 0; i < 10; i++)
			{
				Dust.NewDust(Projectile.Center, 1, 1, DustID.Sand, (float)Main.rand.Next(-1, 2), (float)Main.rand.Next(-1, 2), 0, default, Main.rand.NextFloat(1f, 1.21f));
			}
            if (Main.myPlayer == Owner.whoAmI)
            {
                for (int i = 0; i < Main.rand.Next(5, 9); i++)
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Main.rand.NextVector2CircularEdge(1, 1), Main.rand.Next(61, 64), 1f);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(5, 11) * -1), ProjectileID.SandBallFalling, Projectile.damage - Projectile.damage / 4, 5, Projectile.owner);
				}
			}
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		}
	}
}