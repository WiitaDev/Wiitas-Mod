using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Ammo;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Sandlion;

namespace WiitaMod.Items
{
	public class SandlionLauncher : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandlion Launcher");
			Tooltip.SetDefault("'This is an amazing idea!'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

		}

		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 92;
			Item.height = 34;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SandlionProjectile>();
			Item.useAmmo = ModContent.ItemType<SandlionAmmo>();
			Item.shootSpeed = 8f;

		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Sandgun, 1);
			recipe.AddIngredient(ItemID.RocketLauncher, 1);
			recipe.AddIngredient<IllegalRocketLauncherParts>(1);
			recipe.AddIngredient(ItemID.AntlionMandible, 15);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			Vector2 offset = new Vector2(-24, 0);
			return offset;
		}
	}
}