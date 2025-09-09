using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger
{
	public class PressureWasher : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
		{
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
		}

		public override void SetDefaults()
		{
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.reuseDelay = 10;
            Item.shootSpeed = 10f;
            Item.knockBack = 4f;
            Item.width = 78;
            Item.height = 42;
                Item.damage = 15;
            //Item.UseSound = SoundID.Item100;
            Item.shoot = ProjectileID.WaterGun;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 5);
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
            type = ModContent.ProjectileType<PressureWasherProj>();
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.WaterGun);
			recipe.AddRecipeGroup("PrehardTier2", 12);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4,-6);
        }
    }
}