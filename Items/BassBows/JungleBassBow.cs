using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.BassArrows;

namespace WiitaMod.Items.BassBows
{
	public class JungleBassBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jungle Bass Bow");
			Tooltip.SetDefault("Wooden arrows turn into Leaf Bass" + 
							 "\nThe Leaf Bass breaks into homing leaves" +
							 "\nThe leaves ignore 50 enemy defense");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;


		}

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 34;
			Item.height = 54;
			Item.scale = 1f;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 8f;

		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.WoodenArrowFriendly)
			{
				type = ModContent.ProjectileType<JungleBassArrow>();
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Bass, 5);
			recipe.AddIngredient(ItemID.JungleSpores, 12);
			recipe.AddIngredient(ItemID.Stinger, 6);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			Vector2 offset = new Vector2(-4, 0);
			return offset;
		}
	}
}