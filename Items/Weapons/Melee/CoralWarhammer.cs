using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Melee;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Melee
{

    public class CoralWarhammer : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public int attackType = 1; // keeps track of which attack it is
        public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 48;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Green;

            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<CoralWarhammerHold>(); // The sword as a projectile
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.velocity.Y != 0 || player.grapCount > 0 || player.ropeCount > 0)
            {
                attackType = 2; // The spin attack            
            }
            else
            {
                attackType = attackType == 0 ? 1 : 0;
            }

            // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);

            comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire


            return false;
        }

        public override void UpdateInventory(Player player)
        {
            if (comboExpireTimer++ >= 60) // after 60 ticks (== 1 seconds) in inventory, reset the attack pattern
                attackType = 1;
        }

        public override bool MeleePrefix()
        {
            return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Coral, 10)
                .AddRecipeGroup("PrehardTier3", 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}