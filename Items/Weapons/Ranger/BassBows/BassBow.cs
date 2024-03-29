using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.BassArrows;

namespace WiitaMod.Items.Weapons.Ranger.BassBows
{
	public class BassBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bass Bow");
			// Tooltip.SetDefault("Arrows turn into bass");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

		}

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 32;
			Item.height = 50;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<BassArrow>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 9f;

		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = ModContent.ProjectileType<BassArrow>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Bass, 5);
			recipe.AddIngredient(ItemID.WoodenBow, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.AddCondition(Condition.NearWater);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			Vector2 offset = new Vector2(-4, 0);
			return offset;
		}
	}
}