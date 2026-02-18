using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Tiles;
using WiitaMod.Systems;
using WiitaMod.Items.Consumables.Critters;

namespace WiitaMod.Items.Consumables.Food
{
    public class HallucigeniaNoodles : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

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
            Item.DefaultToFood(72, 60, BuffID.WellFed3, 28800, true); // 28800 is 8 minutes: 8 * 60 * 60
            Item.width = 30;
            Item.height = 28;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<HallucigeniaItem>())
                .AddTile(TileID.CookingPots)
                .Register();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
        }
    }
}