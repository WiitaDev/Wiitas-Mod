using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Systems
{
    public class RecipeModSystem : ModSystem
    {
        public override void AddRecipeGroups()/* tModPorter Note: Removed. Use ModSystem.AddRecipeGroups */
        {
            RecipeGroup PreHard1 = new RecipeGroup(() => Lang.misc[37] + " Copper Bar", new int[]
            {
                ItemID.CopperBar,
                ItemID.TinBar
            });
            RecipeGroup.RegisterGroup("PrehardTier1", PreHard1);

            RecipeGroup PreHard2 = new RecipeGroup(() => Lang.misc[37] + " Iron Bar", new int[]
            {
                ItemID.IronBar,
                ItemID.LeadBar
            });
            RecipeGroup.RegisterGroup("PrehardTier2", PreHard2);

            RecipeGroup PreHard3 = new RecipeGroup(() => Lang.misc[37] + " Gold Bar", new int[]
            {
                ItemID.GoldBar,
                ItemID.PlatinumBar
            });
            RecipeGroup.RegisterGroup("PrehardTier3", PreHard3);

            RecipeGroup Hard1 = new RecipeGroup(() => Lang.misc[37] + " Cobalt Bar", new int[]
            {
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            });
            RecipeGroup.RegisterGroup("HardmodeTier1", Hard1);

            RecipeGroup Hard2 = new RecipeGroup(() => Lang.misc[37] + " Mythril Bar", new int[]
            {
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            });
            RecipeGroup.RegisterGroup("HardmodeTier2", Hard2);

            RecipeGroup Hard3 = new RecipeGroup(() => Lang.misc[37] + " Titanium Bar", new int[]
            {
                ItemID.TitaniumBar,
                ItemID.AdamantiteBar
            });
            RecipeGroup.RegisterGroup("HardmodeTier3", Hard3);
            

            //Ores vvvv            
            
            RecipeGroup PreHardOre1 = new RecipeGroup(() => Lang.misc[37] + " Copper Ore", new int[]
            {
                ItemID.CopperOre,
                ItemID.TinOre
            });
            RecipeGroup.RegisterGroup("PrehardOreTier1", PreHard1);

            RecipeGroup PreHardOre2 = new RecipeGroup(() => Lang.misc[37] + " Iron Ore", new int[]
            {
                ItemID.IronOre,
                ItemID.LeadOre
            });
            RecipeGroup.RegisterGroup("PrehardOreTier2", PreHard2);

            RecipeGroup PreHardOre3 = new RecipeGroup(() => Lang.misc[37] + " Gold Ore", new int[]
            {
                ItemID.GoldOre,
                ItemID.PlatinumOre
            });
            RecipeGroup.RegisterGroup("PrehardOreTier3", PreHard3);

            RecipeGroup HardOre1 = new RecipeGroup(() => Lang.misc[37] + " Cobalt Ore", new int[]
            {
                ItemID.CobaltOre,
                ItemID.PalladiumOre
            });
            RecipeGroup.RegisterGroup("HardmodeOreTier1", Hard1);

            RecipeGroup HardOre2 = new RecipeGroup(() => Lang.misc[37] + " Mythril Ore", new int[]
            {
                ItemID.MythrilOre,
                ItemID.OrichalcumOre
            });
            RecipeGroup.RegisterGroup("HardmodeOreTier2", Hard2);

            RecipeGroup HardOre3 = new RecipeGroup(() => Lang.misc[37] + " Titanium Ore", new int[]
            {
                ItemID.TitaniumOre,
                ItemID.AdamantiteOre
            });
            RecipeGroup.RegisterGroup("HardmodeOreTier3", Hard3);
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.DivingHelmet);
            recipe.AddRecipeGroup("PrehardTier1", 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}