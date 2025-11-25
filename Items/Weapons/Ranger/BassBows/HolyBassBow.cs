using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.BassArrows;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger.BassBows
{
	public class HolyBassBow : ModItem
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
			Item.damage = 55;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 36;
			Item.height = 56;
			Item.scale = 1f;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<HolyBassLaser>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 1f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            type = ModContent.ProjectileType<HolyBassLaser>();
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.HallowedBar, 12);
            recipe.AddIngredient(ItemID.Bass, 5);
            recipe.AddIngredient(ItemID.SoulofLight, 8);
            recipe.AddIngredient(ItemID.PixieDust, 10);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}


		public override Vector2? HoldoutOffset()
		{
			Vector2 offset = new Vector2(-4, 0);
			return offset;
		}
	}
}