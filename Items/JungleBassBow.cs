using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles;
using Terraria.GameContent.Creative;

namespace WiitaMod.Items
{
	public class JungleBassBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jungle Bass Bow"); 
			Tooltip.SetDefault("Wooden arrows turn into spore bass that explode into spore clouds");
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
			Item.useStyle = 5;
			Item.knockBack = 1;
			Item.value = 10000;
			Item.rare = 3;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 7f;

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
            recipe.AddIngredient(ItemID.JungleSpores, 16);
            recipe.AddIngredient(ItemID.Stinger, 10);
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