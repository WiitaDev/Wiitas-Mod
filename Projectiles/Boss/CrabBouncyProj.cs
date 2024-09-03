using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles;
using WiitaMod.Systems.ParticleSystems;

namespace WiitaMod.Projectiles.Magic
{
    public class CrabBouncyProj : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Palladin's shield");     //The English name of the projectile
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 5;               //The width of projectile hitbox
            Projectile.height = 5;              //The height of projectile hitbox
            Projectile.aiStyle = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 660;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.hostile = true;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            // Adding light to the projectile
            Lighting.AddLight(Projectile.position, 0.1f, 0.2f, 0.5f);

            // Adding a blue dust trail
            if (Main.rand.NextBool(2)) // 50% chance to spawn dust
            {
                SmokeParticle smokeParticle = new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Color.RoyalBlue, 90, 0.15f, 0.75f, MathHelper.ToRadians(2), true, required: true);

                ParticleManager.SpawnParticle(smokeParticle);
            }

            // Bouncing logic
            if (Projectile.velocity.X != Projectile.oldVelocity.X)
            {
                Projectile.velocity.X = -Projectile.oldVelocity.X;
            }
            if (Projectile.velocity.Y != Projectile.oldVelocity.Y)
            {
                Projectile.velocity.Y = -Projectile.oldVelocity.Y;
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // The bouncing logic on tile collision
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X; // Invert X velocity on X collision
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y; // Invert Y velocity on Y collision
            }

            // Reduce the Projectile's velocity to simulate friction
            Projectile.velocity *= 0.95f;

            return false; // Don't destroy the Projectile on collision
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            for (int i = 0; i < 5; i++)
            {
                SmokeParticle smokeParticle = new SmokeParticle(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), Color.RoyalBlue, 90, 0.15f, 0.75f, MathHelper.ToRadians(2), true);

                ParticleManager.SpawnParticle(smokeParticle);
            }
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.BlueTrail).Draw(Projectile);

            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;

            Color color = Color.RoyalBlue;
            color.A = 0;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), color, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 0.65f, 0, 0);

            return false;
        }
    }
}
