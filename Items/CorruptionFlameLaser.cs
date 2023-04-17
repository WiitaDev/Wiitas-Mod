using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Items
{
	public class CorruptionFlameLaser : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Corruption FlameLaser");
            Tooltip.SetDefault("AA GUGU GAGA");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 84;
			Item.height = 30;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Flames;
			Item.useAmmo = ItemID.Gel;
			Item.channel = true;
			Item.shootSpeed = 5f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.channel = true;
			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DemoniteBar, 15);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(-14, 0);
            return offset;
        }
    }
}