using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Placeable
{
    public class CompressedSandstoneItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            // DisplayName.SetDefault("TropicalSand");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CompressedSandstone>());
            Item.width = 14;
            Item.height = 14;
        }

    }
}