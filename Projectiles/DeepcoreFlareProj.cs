using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Placeable;
using WiitaMod.Tiles;

namespace WiitaMod.Projectiles
{
    public class DeepcoreFlareProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
            ProjectileID.Sets.ForcePlateDetection[Type] = true;
            ProjectileID.Sets.FallingBlockTileItem[Type] = new(ModContent.TileType<DeepcoreFlareTile>(), ModContent.ItemType<DeepcoreFlareItem>());
        }

        public override void SetDefaults()
        {
            // The sandgun projectile when compared to the falling projectile has a ranged damage type, isn't hostile, and has extraupdates = 1.
            // Note that EbonsandBallGun has infinite penetration, unlike SandBallGun
            Projectile.CloneDefaults(ProjectileID.EbonsandBallGun);
            Projectile.damage = 0;
            Projectile.friendly = false;
            //AIType = ProjectileID.EbonsandBallGun; // This is needed for some logic in the ProjAIStyleID.FallingTile code.
        }

        public override void AI()
        {
            base.AI();

            Lighting.AddLight(Projectile.Center, Color.CadetBlue.ToVector3());
            Dust.NewDust(Projectile.Center, 5, 5, DustID.UltraBrightTorch, Main.rand.NextFloat(-1.00f , 1.00f), Main.rand.NextFloat(-1.00f, 1.00f));

            if (Projectile.ai[1] <= 60) 
            {
                Projectile.velocity = Projectile.oldVelocity;
            }

            Projectile.rotation += MathHelper.ToRadians(5);
            Projectile.ai[1]++;
        }
    }
}