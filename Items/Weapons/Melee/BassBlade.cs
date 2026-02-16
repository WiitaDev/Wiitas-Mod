using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Magic;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Melee
{
	public class BassBlade : ModItem
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

            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.knockBack = 6f;
            Item.width = 48;
            Item.height = 48;
            Item.useTurnOnAnimationStart = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 50);
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bass, 5);
            recipe.AddIngredient(ItemID.WoodenSword, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddCondition(Condition.NearWater);
            recipe.Register();
        }
    }
}