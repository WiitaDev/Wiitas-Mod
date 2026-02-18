using Terraria;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Summon;
using WiitaMod.Systems;

namespace WiitaMod.Buffs
{
    public class BassSummonBuff : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BassSummon>()] > 0)
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

