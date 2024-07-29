using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Placeable
{
    public class HamisBannerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hamis Banner");
            // Tooltip.SetDefault("Nearby players get a bonus against: Hamis");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<HamisBanner>();
            Item.width = 10;
            Item.height = 24;
            Item.value = 0;
        }
    }
}