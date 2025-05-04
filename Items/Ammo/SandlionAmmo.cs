using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.Sandlion;
using WiitaMod.Systems;

namespace WiitaMod.Items.Ammo
{
	public class SandlionAmmo : ModItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults()
		{
            Item.damage = 50; // The damage for projectiles isn't actually 50, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<SandlionProjectile>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 16f; // The speed of the projectile.
            Item.ammo = Item.type; // The ammo class this ammo belongs to.

        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(50);
            recipe.AddIngredient(ItemID.SandBlock, 100);
            recipe.AddIngredient(ItemID.AntlionMandible, 1);
            recipe.AddIngredient(ItemID.DryRocket, 50);
            recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}