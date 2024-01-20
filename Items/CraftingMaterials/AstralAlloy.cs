using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles;

namespace WiitaMod.Items.CraftingMaterials
{
	public class AstralAlloy : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Illegal Rocket Launcher Parts");
			// Tooltip.SetDefault("'Banned everywhere'");

			Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults()
		{
            Item.width = 58;
            Item.height = 64;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 25, 0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddRecipeGroup("HardmodeTier3", 15)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}