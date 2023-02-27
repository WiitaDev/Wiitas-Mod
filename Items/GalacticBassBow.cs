using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles;

namespace WiitaMod.Items
{
	public class GalacticBassBow : ModItem
	{
		public int comboCounter = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Galactic Bass Bow");
			Tooltip.SetDefault("Oh boy imma write this later...");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

		}

		public override void SetDefaults()
		{
			Item.damage = 90;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 36;
			Item.height = 74;
			Item.scale = 1f;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.value = Item.buyPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 30f;

		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (type == ProjectileID.WoodenArrowFriendly)
			{
				type = ModContent.ProjectileType<GalacticBassArrow>();
			}
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int Proj = 1;
            Vector2 Offset = velocity.RotatedBy(MathHelper.ToRadians(3));
            for (int i = 0; i < Proj; i++)
            {
                if (Main.myPlayer == player.whoAmI)
				{
					Projectile.NewProjectile(source, position, Offset, type, damage, knockback, player.whoAmI, ai1: 100);
				}
            }
            return true;
        }

		public override void HoldItem(Player player)
		{
			
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Bass, 69);
			recipe.AddIngredient(ItemID.FallenStar, 420);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			Vector2 offset = new Vector2(-4, -8);
			return offset;
		}
	}
}