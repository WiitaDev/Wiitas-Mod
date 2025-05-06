using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Magic;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Magic
{
    public class SandPouch : ModItem
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
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 8;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.knockBack = 0f;
            Item.width = 26;
            Item.height = 30;
            Item.shootSpeed = 0f;
            Item.UseSound = SoundID.Item25;
            Item.shoot = ModContent.ProjectileType<SandPouchProj>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1);
            Item.noMelee = true;
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            ModGlobalPlayer modPlayer = player.GetModPlayer<ModGlobalPlayer>();

            // Cycle through combo stages
            switch (modPlayer.SandAttackCounter)
            {
                case 0:
                    type = ModContent.ProjectileType<SandPouchProj>();
                    damage = 22;
                    break;
                case 1:
                    type = ModContent.ProjectileType<SandPouchProj>();
                    damage = 25;
                    break;
                case 2:
                    type = ModContent.ProjectileType<SandPouchProj>();
                    damage = 40;
                    Item.shootSpeed = 8f;
                    break;
            }


            // Update combo tracking
            if (modPlayer.SandAttackCounter < 2)
                modPlayer.SandAttackTimer = 30;
            else
                modPlayer.SandAttackTimer = 0;

            modPlayer.SandAttackCounter = (modPlayer.SandAttackCounter + 1) % 3;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Silk, 5);
            recipe.AddIngredient(ItemID.FallenStar, 3);
            recipe.AddIngredient(ItemID.SandBlock, 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}