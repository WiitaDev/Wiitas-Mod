using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles
{
    public class HamisPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            //DrawOffsetX = -20;
            DisplayName.SetDefault("Hamis Pet"); // Automatic from .lang files
            Main.projFrames[Projectile.type] = 10;

            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MiniMinotaur);
            AIType = ProjectileID.MiniMinotaur;
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.scale = 1.5f;
            //AnimationType = ProjectileID.BabyDino;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.miniMinotaur = false; // Relic from AIType
            return true;
        }
        public override void AI()
        {
            Projectile.velocity.X *= 1.00f;
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
