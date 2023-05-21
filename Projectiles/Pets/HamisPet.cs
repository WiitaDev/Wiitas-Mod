using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.Pets
{
    public class HamisPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            //DrawOffsetX = -20;
            // DisplayName.SetDefault("Hamis Pet");
            Main.projFrames[Projectile.type] = 10;

            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MiniMinotaur);
            AIType = ProjectileID.MiniMinotaur;
            Projectile.width = 22;
            Projectile.height = 19; // the height is 1 pixel lower because otherwise it had a row of pixels visible on top of it. And this works fine...
            Projectile.scale = 1.5f;
            DrawOriginOffsetY = 4;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.miniMinotaur = false; // Relic from AIType
            return true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
            {
                player.GetModPlayer<ModGlobalPlayer>().HamisPetEquipped = false;
            }
            if (player.GetModPlayer<ModGlobalPlayer>().HamisPetEquipped)
            {
                Projectile.timeLeft = 2;
            }
        }
    }
}
