using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Ranger.Sandlion
{
	public class SandlionProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sandlion Rocket");
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 18;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = 0;
			Projectile.knockBack = 2f;
			Projectile.tileCollide = true;
			Projectile.penetrate = 2;
			Projectile.friendly = true;
		}
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 2)
            {
                Projectile.penetrate = -1;
                return false;
            }
            else
            {
                return true;
            }
        }
        public override void AI()
		{
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.penetrate != 2)
                {
                    Projectile.alpha = 255;
                    Projectile.Resize(200, 200);
                    Projectile.penetrate = -1;
                    Projectile.timeLeft = 3;
                }
            }
            Projectile.spriteDirection = -Projectile.direction;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);
			Dust.NewDust(Projectile.Center, 1, 1, DustID.Sand, (float)Main.rand.Next(-1, 2), (float)Main.rand.Next(-1, 2), 0, default, Main.rand.NextFloat(1f, 1.21f));
			for (int i = 0; i < 3; i++)
			{
				Dust.NewDust(Projectile.Center, 1, 1, DustID.Smoke, 0, 0, 0, default, Main.rand.NextFloat(1f, 1.21f));
			}
        }

		public override void OnKill(int timeLeft)
		{
            Player Owner = Main.player[Projectile.owner];
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.Center, 1, 1, DustID.Sand, (float)Main.rand.Next(-1, 2), (float)Main.rand.Next(-1, 2), 0, default, Main.rand.NextFloat(1f, 1.21f));
            }
            if (Main.myPlayer == Owner.whoAmI)
            {
                for (int i = 0; i < Main.rand.Next(7, 11); i++)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-1, 1), Main.rand.Next(-1, 4)), Main.rand.Next(61, 64), 1f);
                    int Projectile1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(5, 11) * -1), ProjectileID.SandBallFalling, Projectile.damage - Projectile.damage / 4, 5, Projectile.owner);
                    Main.projectile[Projectile1].friendly = true;
                    Main.projectile[Projectile1].hostile = false;
                }
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            SoundEngine.PlaySound(SoundID.NPCDeath1.WithVolumeScale(0.75f), Projectile.Center);
        }
	}
}