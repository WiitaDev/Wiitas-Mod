using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Prim;

namespace WiitaMod.Projectiles.Magic
{
    public class CoralStaffProj : ModProjectile, IDrawAdditive
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.BoneArrow}";

        public TestPrimTrail trail;
        private bool initialized = false;
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Coral Staff Proj");     //The English name of the projectile
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.light = 2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (!initialized)
            {
                initialized = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    trail = new TestPrimTrail(Projectile, 500);
                    WiitaMod.primitives.CreateTrail(trail);
                }
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);
        }

        public void AdditiveCall(SpriteBatch sB, Vector2 screenPos)
        {
            Vector2 drawPosition = Projectile.Center;

            TrianglePrimitive tri = new TrianglePrimitive()
            {
                TipPosition = drawPosition - screenPos,
                Rotation = Projectile.velocity.ToRotation(),
                Height = 100,
                Color = Color.White,
                Width = 80
            };

            PrimitiveRenderer.DrawPrimitiveShape(tri, WiitaMod.basicEffect);

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item37.WithPitchOffset(-1f), Projectile.Center);
        }
    }
}