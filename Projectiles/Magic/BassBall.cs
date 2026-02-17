using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.Magic
{
    public class BassBall : ModProjectile
    {
        private float rotationSpeed;

        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
        }

        public override void OnSpawn(IEntitySource source)
        {
            rotationSpeed = Main.rand.NextFloat(0.1f, 0.4f) * (Main.rand.NextBool() ? 1f : -1f);
        }

        public override void AI()
        {
            Projectile.spriteDirection = -Projectile.direction;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 20)
            {
                Projectile.velocity.Y += 0.2f;
            }

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Projectile.rotation += rotationSpeed;

            for (int i = 0; i < 3; i++)
                Dust.NewDust(Projectile.Center, 5, 5, DustID.Water_Snow, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.6f,1.0f));

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X * 0.85f;
                    if (MathF.Abs(Projectile.velocity.X) > 8f)
                        Projectile.velocity.X = 8f;
                    if (Projectile.velocity.X < -8f)
                        Projectile.velocity.X = -8f;

                }
                if (Projectile.velocity.Y != oldVelocity.Y) 
                {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
                    if (Projectile.velocity.Y > 8f)
                        Projectile.velocity.Y = 8f;
                    if (Projectile.velocity.Y < -8f)
                        Projectile.velocity.Y = -8f;
                }
            }
            return false;
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