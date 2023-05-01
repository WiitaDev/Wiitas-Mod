using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.BassArrows;

namespace WiitaMod.Items.BassBows
{
	public class GalacticBassBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Bass Bow");
			Tooltip.SetDefault("Arrows turn into Cosmic Bass" +
							 "\nWhen you hit an enemy with Cosmic Bass, it creates a ring around the enemy" +
							 "\nThe ring shoots outs Cosmic projectiles that deal 75% damage of the weapon");

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
			Item.value = Item.sellPrice(0, 20, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 30f;

		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = ModContent.ProjectileType<GalacticBassArrow>();
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float numberProjectiles = 2; // 3 shots
			float rotation = MathHelper.ToRadians(3);//Shoots them in a 3 degree radius.
			int ProjAi = 0; // This flips the Arrows sprite direction in its code
			position += Vector2.Normalize(velocity * 3f); //3 should equal whatever number you had on the previous line
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / numberProjectiles)); // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(Item.GetSource_FromThis(), position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, Main.myPlayer, ai1: ProjAi); //Creates a new projectile with our new vector for spread.
				ProjAi = 100;
			}
			return false;
		}

		public override void HoldItem(Player player)
		{

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Bass, 20);
			recipe.AddIngredient(ItemID.FragmentVortex, 12);
			recipe.AddIngredient(ItemID.FragmentNebula, 12);
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