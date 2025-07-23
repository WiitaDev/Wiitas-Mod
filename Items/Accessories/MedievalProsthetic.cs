using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;
using WiitaMod.Systems;

namespace WiitaMod.Items.Accessories
{
    public class MedievalProsthetic : ModItem
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
            Item.accessory = true;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.LightRed;
        }

        bool flag;
        public override void UpdateEquip(Player player)
        {
            if (player.velocity.Y != 0 && player.wings <= 0 && !player.mount.Active)  // not on the ground
            {
                if (flag)
                {
                    player.wingTime *= 2;
                    flag = false;
                }
                player.runAcceleration *= 1.5f;
                player.maxRunSpeed *= 1.3f;
            }
            else if (!flag && player.velocity.Y == 0) //on the ground
            {
                flag = true;
            }

            if (player.velocity.Y == 0 && player.velocity.X != 0 && !player.mount.Active) // on the ground and moving
            {
                if (player.whoAmI == Main.myPlayer)
                    player.AddBuff(ModContent.BuffType<ProstheticDebuff>(), 3);
            }

        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 15);
            recipe.AddRecipeGroup("PrehardTier2", 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}