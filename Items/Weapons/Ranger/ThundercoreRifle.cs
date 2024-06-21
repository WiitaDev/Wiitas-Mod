using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
			Item.damage = 580;
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
			Item.shoot = ModContent.ProjectileType<ThundercoreRifleHold>();
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 6f;
		}


        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ThundercoreRifleHold>()] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ThundercoreRifleHold>()] <= 0)
            {
				if (player.altFunctionUse == 0)
				{
					Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<ThundercoreRifleHold>(), damage, 0, player.whoAmI);
					Main.player[player.whoAmI].GetModPlayer<ModGlobalPlayer>().screenShakeTimerGlobal = -80;
					Main.player[player.whoAmI].GetModPlayer<ModGlobalPlayer>().screenShakeVelocity = 400;
				}
            }

            return false;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("WiitaMod/Items/Weapons/Ranger/ThundercoreRifle_Glow", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
                    Item.position.Y - Main.screenPosition.Y + Item.height - texture.Height * 0.5f + 2f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
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