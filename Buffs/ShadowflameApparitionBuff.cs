using Terraria;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Summon;

namespace WiitaMod.Buffs
{
    public class ShadowflameApparitionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadowflame Apparition");
            // Description.SetDefault("The Shadow Apparition will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ShadowflameApparitionMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 899831289;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

}

