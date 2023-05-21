using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Items.Accessories
{
	public class HealthFlower : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Health Flower");
			/* Tooltip.SetDefault("Automatically use healing potions when taking lethal damage to prevent death" +
							 "\nOr when your health is under 50% & you take damage from an enemy"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<ModGlobalPlayer>().HealthFlowerEquipped = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.JungleRose, 1);
			recipe.AddIngredient(ItemID.HealingPotion, 1);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}