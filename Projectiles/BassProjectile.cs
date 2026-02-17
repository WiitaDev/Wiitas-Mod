using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles
{
	public class BassProjectile : ModProjectile
	{
        private float rotationSpeed = 0;

        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 22;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = 0;
			Projectile.knockBack = 2f;
			Projectile.tileCollide = true;
			Projectile.penetrate = 1;
			Projectile.friendly = true;
		}

        public override void AI()
		{
			if (rotationSpeed == 0)
                rotationSpeed = Main.rand.NextFloat(0.1f, 0.3f) * (Main.rand.NextBool() ? 1f : -1f);


            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            Projectile.rotation = Projectile.rotation + rotationSpeed;

            Projectile.spriteDirection = -Projectile.direction;
			Projectile.velocity += new Vector2(0, 0.15f);
			Vector2 vel = Projectile.velocity;
			vel.Normalize();
			int dust = Dust.NewDust(Projectile.Center - vel, 5, 5, DustID.Water_Snow, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dust].scale = (float)Main.rand.Next(100, 135) * 0.013f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.Stinky, 180);
			target.AddBuff(BuffID.Wet, 180);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Blood, (float)Main.rand.Next(-3, 3), (float)Main.rand.Next(-3, 3), 0, default(Color), 1f);
				Main.dust[dustHit].scale = (float)Main.rand.Next(100, 135) * 0.013f;
			}
			SoundEngine.PlaySound(SoundID.NPCDeath1.WithVolumeScale(0.75f).WithPitchOffset(0.1f), Projectile.Center);
		}
	}
}