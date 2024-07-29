using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Magic;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Magic
{
	public class InfernalAlmanac : ModItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
		{
            Item.ResearchUnlockCount = 1;
        }

		public override void SetDefaults()
		{

            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.mana = 8;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.reuseDelay = 10;
            Item.knockBack = 6f;
            Item.width = 56;
            Item.height = 94;
            Item.UseSound = SoundID.Item80;
            Item.shoot = ModContent.ProjectileType<InfernalAlmanacHold>();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.channel = true;
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<InfernalAlmanacHold>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<InfernalAlmanacHold>()] <= 0;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.HellstoneBar, 8);
			recipe.AddIngredient(ItemID.Fireblossom, 3);
			recipe.AddTile(TileID.Bookcases);
			recipe.Register();
		}
	}
}