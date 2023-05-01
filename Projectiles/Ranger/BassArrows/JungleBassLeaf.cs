using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Ranger.BassArrows
{
	public class JungleBassLeaf : ModProjectile
	{
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Leaf}";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bass Leaf");
			Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 14;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.aiStyle = 0;
			Projectile.knockBack = 2f;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
            Projectile.ArmorPenetration = 50;
            Projectile.CritChance = 0;
            Projectile.timeLeft = 240;
			Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
		}

		public override void AI()
		{
			Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);

            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.ai[0]++;
			if (Projectile.ai[0] >= 90) 
			{
                float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
                float projSpeed = 12f; // The speed at which the projectile moves towards the target
                Projectile.friendly = true;

                // Trying to find NPC closest to the projectile
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null) 
                {
                    Projectile.timeLeft -= 10;
                    return;
                }
                // If found, change the velocity of the projectile and turn it in the direction of the target
                // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.velocity *= 0.95f;
                Projectile.scale += 0.05f;
                if(Projectile.scale >= 1f) 
                {
                    Projectile.scale = 1f;
                }
                Projectile.friendly = false;
            }
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
		}
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0f;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = false;
        }

        public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
			{
				int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Grass, (Main.rand.Next(-2, 3)), (Main.rand.Next(-2, 3)), 0, default(Color), 1f);
				Main.dust[dustHit].scale = (float)Main.rand.Next(135, 160) * 0.013f;
				Main.dust[dustHit].noGravity = true;
			}
			SoundEngine.PlaySound(SoundID.Grass.WithVolumeScale(0.75f).WithPitchOffset(-0.75f), Projectile.Center);
		}
        public override bool PreDraw(ref Color lightColor)
        {
            float maxDetectRadius = 400f;
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (Projectile.ai[0] >= 90 && closestNPC != null)
            {
                default(Effects.JungleBassLeafTrail).Draw(Projectile);
            }
            return true;
        }

    }
}