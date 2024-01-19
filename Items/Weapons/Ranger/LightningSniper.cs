using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Ammo;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Ranger.BassArrows;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger
{
	public class LightningSniper : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
            ItemID.Sets.gunProj[Type] = true;
        }

		public override void SetDefaults()
		{
			Item.damage = 550;
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
			Item.shoot = ModContent.ProjectileType<LightningSniperHold>();
			Item.shootSpeed = 6f;
			Item.noUseGraphic = true;
		}


        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<LightningSniperHold>()] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LightningSniperHold>()] <= 0)
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
			recipe.AddIngredient(ItemID.SoulofSight, 5);
			recipe.AddIngredient(ItemID.EyeoftheGolem);
            recipe.AddRecipeGroup("HardmodeTier3", 8);
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