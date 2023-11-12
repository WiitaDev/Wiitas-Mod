using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.BassArrows.CosmicBassBow;

namespace WiitaMod.Items.Weapons.Ranger.BassBows
{
	public class CosmicBassBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cosmic Bass Bow");
			/* Tooltip.SetDefault("Arrows turn into Cosmic Bass" +
							 "\nWhen you hit an enemy with Cosmic Bass, it creates a ring around the enemy" +
							 "\nThe ring shoots outs Cosmic projectiles that deal 75% damage of the weapon"); */

            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{

            Item.shootSpeed = 30f;

			Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAmmo = AmmoID.Arrow;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.reuseDelay = 10;
            Item.shootSpeed = 30f;
            Item.knockBack = 1f;
            Item.width = 36;
            Item.height = 74;
                Item.damage = 125;
            Item.UseSound = SoundID.Item100;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 20);
            Item.noMelee = true;
            Item.channel = true;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.channel = true;
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<CosmicBassBowHold>();
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BassBow>());
			recipe.AddIngredient(ItemID.FragmentVortex, 12);
			recipe.AddIngredient(ItemID.FragmentNebula, 12);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
	}
}