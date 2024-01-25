using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Ammo;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Ranger;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger
{
	public class ThundercoreRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
            ItemID.Sets.gunProj[Type] = true;
        }

		public override void SetDefaults()
		{
			Item.damage = 650;
			Item.crit = 20;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 92;
			Item.height = 34;
			Item.useTime = 90;
			Item.useAnimation = 90;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Thunder;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<ThunderCoreRifleHold>();
			Item.shootSpeed = 6f;
		}


        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ThunderCoreRifleHold>()] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ThunderCoreRifleHold>()] <= 0)
            {
				if (player.altFunctionUse == 0)
				{
					Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI);
					Main.player[player.whoAmI].GetModPlayer<ModGlobalPlayer>().screenShakeTimerGlobal = -80;
					Main.player[player.whoAmI].GetModPlayer<ModGlobalPlayer>().screenShakeVelocity = 400;
				}
            }

            return false;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AstralAlloy>(), 8);
            recipe.AddRecipeGroup("HardmodeTier3", 3);
			recipe.AddIngredient(ItemID.SoulofSight, 10);
			recipe.AddIngredient(ItemID.SoulofFright, 5);
			recipe.AddIngredient(ItemID.Ectoplasm, 3);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			Vector2 offset = new Vector2(-50, -6);
			return offset;
		}
	}
}