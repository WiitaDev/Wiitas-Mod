using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;

namespace WiitaMod.Projectiles.Summon
{
    public class CosmicSentrySentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadowflame Apparition");
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 10;
            Projectile.knockBack = 3f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.sentry = true;
        }
    }
}
