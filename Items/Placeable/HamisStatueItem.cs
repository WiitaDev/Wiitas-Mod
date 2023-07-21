using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Pets;
using WiitaMod.Tiles;

namespace WiitaMod.Items.Placeable
{
    internal class HamisStatueItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hamis Statue");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            Item.createTile = ModContent.TileType<HamisStatue>();
            Item.width = 24;
            Item.height = 26;
            Item.value = 100;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 50);
            recipe.AddIngredient(ModContent.ItemType<HamisPetItem>());
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.AddCondition(Condition.InGraveyard);
            recipe.Register();
        }
    }
}