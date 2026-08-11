using Terraria;
using Terraria.ModLoader;
using WiitaMod.Systems;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Placeable
{
    public class CrystalRockItem : ModItem
    {

        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystalRock>());

        }
    }
}