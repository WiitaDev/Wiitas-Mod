using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.BassArrows;

namespace WiitaMod.Items.Weapons.Ranger.BassBows
{
	public class MoltenBassBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Molten Bass Bow");
			Tooltip.SetDefault("Arrows turn into Molten Bass that explode spreading fire\nThe Molten Bass gets extinguished when touching water");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			}

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 36;
			Item.height = 46;
			Item.scale = 1.1f;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 8.5f;

		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            type = ModContent.ProjectileType<MoltenBassArrow>();
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Bass, 5);
			recipe.AddIngredient(ItemID.MoltenFury, 1);
            recipe.AddIngredient(ItemID.Obsidian, 15);
            recipe.AddTile(TileID.Hellforge);
			recipe.Register();
		}


		public override Vector2? HoldoutOffset()
		{
			Vector2 offset = new Vector2(-4, 0);
			return offset;
		}
	}
}