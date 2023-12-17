using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.BassArrows;

namespace WiitaMod.Projectiles.Summon
{
    public class BassSentrySentry : ModProjectile
    {
        float ringRadius = 20 * 16; //20 tiles, cuz 1 tile is 16pixels

        public float ShootTimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadowflame Apparition");
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 48;
            Projectile.height = 38;
            Projectile.damage = 10;
            Projectile.knockBack = 3f;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.sentry = true;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 15)
                Projectile.velocity.Y += 0.2f;

            for (int i = 0; i < 20; i++)
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(ringRadius, ringRadius), DustID.GreenMoss, Vector2.Zero, 0, default, 1.25f);
                d2.fadeIn = 0.1f;
                d2.noGravity = true;
            }

            for (int n = 0; n < Main.maxNPCs; n++) 
            {
                NPC target = Main.npc[n];
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < ringRadius * ringRadius)
                    {
                        if (ShootTimer == 0)
                        {
                            Vector2 projVel = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10f;

                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projVel, ModContent.ProjectileType<BassArrow>(), Projectile.damage, Projectile.knockBack);
                            ShootTimer = 40;
                        }
                    }
                }
            }
            if (ShootTimer > 0)
            {
                ShootTimer--;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool? CanDamage() => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}
