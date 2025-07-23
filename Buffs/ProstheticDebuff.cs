using Terraria;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Buffs
{
    public class ProstheticDebuff : ModBuff
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<WiitaModPlayer>().ProstheticDebuff = true;
        }
    }

}

