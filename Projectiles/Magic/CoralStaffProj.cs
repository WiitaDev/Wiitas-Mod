using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Sets.SwordsMisc.BladeOfTheDragon;
using WiitaMod.Prim;

namespace WiitaMod.Projectiles.Magic
{
    public class CoralStaffProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.BoneArrow}";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Coral Staff Proj");     //The English name of the projectile
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

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                WiitaMod.primitives.CreateTrail(new CoralStaffPrimTrail(Projectile));
            }
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item37.WithPitchOffset(-1f), Projectile.Center);
        }
    }
}