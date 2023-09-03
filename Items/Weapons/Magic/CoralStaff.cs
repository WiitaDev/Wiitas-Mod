using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Prim;
using WiitaMod.Projectiles.Magic;

namespace WiitaMod.Items.Weapons.Magic
{
	public class CoralStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Coral Staff");
			// Tooltip.SetDefault("mmm probably look bad");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.HasAProjectileThatHasAUsabilityCheck[Type] = true;
            ItemID.Sets.gunProj[Type] = true;
        }

		public override void SetDefaults()
		{
			Item.damage = 1500;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Magic;
			Item.width = 34;
			Item.height = 54;
			Item.scale = 1f;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item5;

            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<CrystalGauntletBall>();
            Item.shootSpeed = 4f;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<CrystalGauntletBall>()] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.manaCost = 0f;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<CrystalGauntletBall>()] <= 0)
            {
                if (player.altFunctionUse == 0)
                    Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI);
            }

            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            gravity *= 0.8f;
            Color glowColor = Color.Aqua * Main.GlobalTimeWrappedHourly;
            Lighting.AddLight(Item.Center, glowColor.ToVector3() * 0.4f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200);
        public override void HoldItem(Player player)
        {
            Color glowColor = Color.Aqua * Main.GlobalTimeWrappedHourly;
            Lighting.AddLight(player.MountedCenter, glowColor.ToVector3() * 0.4f);
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
	}

}