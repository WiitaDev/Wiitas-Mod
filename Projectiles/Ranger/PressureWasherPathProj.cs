using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Particles;

namespace WiitaMod.Projectiles.Ranger
{
    public class PressureWasherPathProj : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public const float maxTimeleft = 18 * (1 + 3); // multiplied by 1 + extraUpdates
        public const int oneTick = (1 + 3) * 2; // 1 + extraUpdates


        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = (int)maxTimeleft;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity /= 1 + Projectile.extraUpdates;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += target.defense * 0.30f;

            modifiers.FinalDamage *= Projectile.timeLeft / maxTimeleft * 1.5f;
        }

        public override void PostAI()
        {

            if (Time < 0.5f)
            {
                Projectile.friendly = false;
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    CheckLavaCollision();
                }
                if (Time % 1 == 0)
                    Projectile.Resize(10 + (int)Time, 10 + (int)Time);
                Projectile.friendly = true;
            }

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.active && !target.friendly)
                {
                    if (Projectile.Colliding(Projectile.Hitbox, target.Hitbox) && Time > 0.5f)
                    {
                        if (Projectile.timeLeft > oneTick && Projectile.timeLeft < maxTimeleft)
                            Projectile.timeLeft = oneTick;
                    }
                }
            }

            if (Projectile.timeLeft <= oneTick + 1)
                Projectile.friendly = false;

            if (Projectile.timeLeft <= oneTick)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
            }


            Time += 0.25f;
        }

        private void CheckLavaCollision()
        {
            for (int i = (int)(Projectile.Left.X / 16f); i <= (int)(Projectile.Right.X / 16f); i++)
            {
                for (int j = (int)(Projectile.Top.Y / 16f); j <= (int)(Projectile.Bottom.Y / 16f); j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.LiquidType == LiquidID.Lava && tile.LiquidAmount > 128)
                    {
                        if (Projectile.timeLeft > oneTick)
                            Projectile.timeLeft = oneTick;
                        return;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > oneTick)
                Projectile.timeLeft = oneTick;

            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;

            Color pointColor = new Color(Lighting.GetSubLight(Projectile.Center));
            MistParticle particle = new MistParticle(Projectile.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextVector2Circular(4f, 4f), pointColor, pointColor, Main.rand.NextFloat(0.15f, 0.35f), 255, MathHelper.ToRadians(2));
            ParticleManager.SpawnParticle(particle);

            return false;
        }
    }
}