using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Placeable;
using WiitaMod.NPCs.Bosses;
using WiitaMod.World;

namespace WiitaMod.Items.Consumables
{
	public class CrabBossSummonItem : ModItem
	{
		public override void SetStaticDefaults()
		{
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
		}

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = true;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<TropicalSandItem>(15)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.DemonAltar)
                .Register();  

            CreateRecipe()
                .AddIngredient<TropicalSandItem>(15)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
	}
}