using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Placeable
{
    public class TropicalOceanPylonItem : ModItem
    {
        public override void SetDefaults()
        {
            // Basically, this a just a shorthand method that will set all default values necessary to place
            // the passed in tile type; in this case, the Example Pylon tile.
            Item.DefaultToPlaceableTile(ModContent.TileType<TropicalOceanPylon>());

            // Another shorthand method that will set the rarity and how much the item is worth.
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 10));
        }
    }
}