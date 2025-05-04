using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles;
using WiitaMod.Systems;

namespace WiitaMod.Items.CraftingMaterials
{
	public class AstralAlloy : ModItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults()
		{
            Item.width = 58;
            Item.height = 64;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 25, 0);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddRecipeGroup("HardmodeOreTier3", 15)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}