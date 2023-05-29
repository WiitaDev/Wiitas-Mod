using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Systems
{
    // Here is a class dedicated to showcasing projectile modifications
    public class ExampleProjectileModifications : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ProjectileID.Flames;
        }
        public override void AI(Projectile projectile)
        {
            if (projectile.ai[1] == 100) //There's probably a better way to do this
            {
                projectile.ai[1]++;
                projectile.scale *= 0.5f;
                projectile.localAI[0] = 10;

                if (projectile.ai[1] >= 160)
                {
                    projectile.localAI[0] = 60;
                }
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            lightColor = Color.Green;
            return true;
        }
    }
}