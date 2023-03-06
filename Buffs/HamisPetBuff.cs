using Terraria;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Pets;
using WiitaMod.Systems;

namespace WiitaMod.Buffs
{
    public class HamisPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hamis");
            Description.SetDefault("The Hamis is friendly :)");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ModGlobalPlayer>().HamisPetEquipped = true;
            player.buffTime[buffIndex] = 18000;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<HamisPet>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<HamisPet>(), 0, 0f, player.whoAmI, 0f, 0f);
            }


        }
    }

}

