using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Consumables.Food
{
    internal class FriedSeagull : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(214, 141, 45),
                new Color(77, 44, 4),
                new Color(241, 167, 70)
            };

            ItemID.Sets.IsFood[Type] = true; //This allows it to be placed on a plate and held correctly
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(72, 60, BuffID.WellFed2, 57600); // 57600 is 16 minutes: 16 * 60 * 60
            Item.width = 36;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Seagull)
                .AddTile(TileID.CookingPots)
                .Register();
        }
    }
}