using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;

namespace WiitaMod.NPCs.Bosses
{
	public class CrystalShot : ModProjectile
	{
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public ref float TargetId => ref Projectile.ai[0];
        public ref float BossId => ref Projectile.ai[1];

        private const int maxTimeleft = 240;



        public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = 0;
			Projectile.knockBack = 2f;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = maxTimeleft;
			Projectile.friendly = false;
			Projectile.hostile = true;
		}

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.tileCollide = false;
            Projectile.localAI[0] = 0f; // 0 = still escaping blocks

        }

        Vector2 offset;
        public override void AI()
		{
            Lighting.AddLight(Projectile.Center, Color.CadetBlue.ToVector3() * 0.4f);

            Player player = Main.player[(int)TargetId];


            if (Projectile.timeLeft >= maxTimeleft - 1) offset = Main.rand.NextVector2CircularEdge(48f, 48f);

            if (Projectile.timeLeft >= maxTimeleft - 60) 
            {
                Projectile.Center = Main.npc[(int)BossId].Center + offset;

                if (Projectile.timeLeft == maxTimeleft - 60) 
                {
                    Vector2 dir = player.Center - Projectile.Center;
                    dir.Normalize();
                    dir *= 5f;
                    Projectile.velocity = dir;
                    SoundEngine.PlaySound(SoundID.Item109.WithVolumeScale(0.75f).WithPitchOffset(0.1f), Projectile.Center);
                }

                if (Main.rand.NextBool((int)MathHelper.Clamp((float)(Projectile.timeLeft - 180) / (float)(maxTimeleft - 180) * 8f, 1, 10)))
                {
                    Vector2 dir = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Particle particle = new GlowOrbParticle(Projectile.Center + dir * 10, -dir, false, 10, Main.rand.NextFloat(0.55f, 0.65f), new Vector2(0.75f, 1f), Color.AliceBlue, true);
                    ParticleManager.SpawnParticle(particle);
                }

                return; 
            }

            // Wait until the projectile has left any solid tiles
            if (Projectile.localAI[0] == 0f)
            {
                if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.tileCollide = true;
                    Projectile.localAI[0] = 1f;
                }
            }

            if (Main.rand.NextBool(5))
            {
                Particle particle = new GlowOrbParticle(Projectile.Center, Main.rand.NextVector2Circular(1f, 1f) + Projectile.velocity / 2, false, 30, Main.rand.NextFloat(0.5f, 1f), new Vector2(1f, 1.5f), Color.CadetBlue, true);
                ParticleManager.SpawnParticle(particle);
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);

        }

        public override void OnKill(int timeLeft)
		{
            for (int i = 0; i < 10; i++)
            {
                Particle particle = new GlowOrbParticle(Projectile.Center, new Vector2(1,0).RotatedBy(MathHelper.TwoPi / 10 * i), false, 30, Main.rand.NextFloat(0.5f, 1f), new Vector2(1f, 1.5f), Color.CadetBlue, true);
                ParticleManager.SpawnParticle(particle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft >= maxTimeleft - 60) return false;

            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;
            Color glowColor = Color.CadetBlue;
            glowColor.A = 0;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), glowColor, Projectile.rotation, glow.Size() / 2, new Vector2(1f, 0.5f), SpriteEffects.None);

			return false;
        }
    }
}