using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Items.Accessories
{
	public class PhilosophersNecklace : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Philosopher's Necklace");
			Tooltip.SetDefault("Reduces the cooldown of healing potions by 25%\nIncreases length of invincibility after taking damage");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.accessory = true;
			Item.value = Item.sellPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.LightRed;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<ModGlobalPlayer>().PhilosophersNecklaceEquipped = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CrossNecklace, 1);
			recipe.AddIngredient(ItemID.PhilosophersStone, 1);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}