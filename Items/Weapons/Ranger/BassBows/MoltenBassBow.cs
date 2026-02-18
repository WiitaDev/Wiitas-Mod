using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.BassArrows;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger.BassBows
{
	public class MoltenBassBow : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Molten Bass Bow");
			// Tooltip.SetDefault("Arrows turn into Molten Bass that explode spreading fire\nThe Molten Bass gets extinguished when touching water");

			Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 36;
			Item.height = 46;
			Item.scale = 1.1f;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 10f;

		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity.Normalize();
			velocity *= Item.shootSpeed;
			knockback = Item.knockBack;
            type = ModContent.ProjectileType<MoltenBassArrow>();
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Bass, 5);
            recipe.AddIngredient(ItemID.HellstoneBar, 12);
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