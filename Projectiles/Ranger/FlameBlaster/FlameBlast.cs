using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Projectiles.Ranger.FlameBlaster
{
    public class FlameBlast : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }


        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter + new Vector2(0, -10) + Projectile.velocity * 9.5f; // the vector offset is the itemholdout y offset
        }


        public override void AI()
        {
            if (Projectile.timeLeft > 10) 
            {
                Projectile.Resize(Projectile.width + 1, Projectile.height + 1);

                if (Projectile.timeLeft == 30) 
                {
                    for (int i = 0; i < 60; i++)
                    {
                        MistParticle mistParticle = new MistParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(40)) * Main.rand.NextFloat(0.2f, 4f), Color.OrangeRed, Color.WhiteSmoke, 0.33f, 255, MathHelper.ToRadians(Main.rand.NextFloat(1f, 3f)));
                        ParticleManager.SpawnParticle(mistParticle);
                        SmokeParticle smokeParticle = new SmokeParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(40)) * Main.rand.NextFloat(0.2f, 4f), Color.Yellow, 90, 0.2f, 1f, MathHelper.ToRadians(Main.rand.NextFloat(1f, 3f)), true);
                        ParticleManager.SpawnParticle(smokeParticle);
                    }
                }
            }
            else 
            {
                Projectile.velocity = Vector2.Zero;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 150);
        }

    }
}