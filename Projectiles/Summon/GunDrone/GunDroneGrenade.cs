using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Summon.GunDrone
{
    public class GunDroneGrenade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = 14;
            Projectile.minion = true;
        }

        public override void OnKill(int timeLeft)
        {
            ProjectileHelper.Explode(Projectile.whoAmI, 150, 150, false, true);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }

    }
}
