using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Placeable
{
    public class TropicalSandItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            // DisplayName.SetDefault("TropicalSand");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TropicalSand>());
            Item.width = 12;
            Item.height = 12;
        }
        /*
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient<ExampleItem>()
                .AddTile<Tiles.Furniture.ExampleWorkbench>()
                .Register();

            CreateRecipe() // Add multiple recipes set to one Item. 
                .AddIngredient<ExampleWall>(4)
                .AddTile<Tiles.Furniture.ExampleWorkbench>()
                .Register();

            CreateRecipe()
                .AddIngredient<ExamplePlatform>(2)
                .AddTile<Tiles.Furniture.ExampleWorkbench>()
                .Register();
        }*/


    }
}