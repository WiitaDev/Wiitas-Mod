using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Placeable
{
    public class DeepcoreFlareItem : ModItem
    {

        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
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
            Item.createTile = ModContent.TileType<DeepcoreFlareTile>();
            Item.width = 24;
            Item.height = 26;
            Item.value = 0;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.CadetBlue.ToVector3());
        }
    }
}