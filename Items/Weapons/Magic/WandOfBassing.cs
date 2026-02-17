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
	public class WandOfBassing : ModItem
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

            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.mana = 2;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.knockBack = 6f;
            Item.width = 28;
            Item.height = 30;
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<BassBall>();
            Item.shootSpeed = 5.5f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 20);
            Item.noMelee = true;
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bass, 5);
            recipe.AddIngredient(ItemID.WandofSparking, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
		}
	}
}