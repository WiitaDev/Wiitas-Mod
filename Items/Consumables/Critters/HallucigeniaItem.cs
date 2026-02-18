using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using WiitaMod.NPCs;
using WiitaMod.Systems;

namespace WiitaMod.Items.Consumables.Critters
{

    public class HallucigeniaItem : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToCapturedCritter(ModContent.NPCType<Hallucigenia>());

            Item.value = Item.sellPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Blue;
            Item.bait = 40;
        }
    }
}