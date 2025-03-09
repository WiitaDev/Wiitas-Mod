using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Ranger;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger
{
	public class ThundercorePressureWasher : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;

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
                Item.damage = 100;
            //Item.UseSound = SoundID.Item100;
            Item.shoot = ProjectileID.WaterGun;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 18);
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
            type = ModContent.ProjectileType<ThundercorePressureWasherProj>();
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient<PressureWasher>();
			recipe.AddIngredient<AstralAlloy>(5);
			recipe.AddIngredient(ItemID.Ectoplasm, 3);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-35,-9);
        }
    }
}