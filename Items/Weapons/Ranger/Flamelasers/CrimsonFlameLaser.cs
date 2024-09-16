using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger.Flamelasers;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger.Flamelasers
{
    public class CrimsonFlameLaser : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bloody Flamelaser");
            /* Tooltip.SetDefault("Uses gel for ammo\n"
                             + "'Is it a Flamethrower or a Laser?'\n"
                             + "'Weaker but more Agile!'"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {

            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.useAmmo = ItemID.Gel;
            Item.channel = true;
            Item.shootSpeed = 8f;

            //All the code above is obsolete due to this
            Item.CloneDefaults(ItemID.Flamethrower);
            Item.width = 84;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.useTime = 6;
            Item.useAnimation = 30;
            Item.damage = 35;
            Item.shoot = ProjectileID.Flames;
            Item.UseSound = SoundID.Item34;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.channel = true;
            if (player.GetModPlayer<ModGlobalPlayer>().flamesShot <= 20)
            {
                Projectile.NewProjectile(Item.GetSource_FromThis(), position.X, position.Y, velocity.X, velocity.Y, ProjectileID.Flames, damage, knockback, Main.myPlayer); //Creates a new projectile with our new vector for spread.
                player.GetModPlayer<ModGlobalPlayer>().flamesShot++;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<ModGlobalPlayer>().flamesShot >= 20)
            {
                type = ModContent.ProjectileType<CrimsonFlameLaserlaser>();
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("HardmodeTier2", 6);
            recipe.AddIngredient(ItemID.CrimtaneBar, 6);
            recipe.AddIngredient(ItemID.Ichor, 10);
            recipe.AddIngredient(ItemID.Vertebrae, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(-14, 0);
            return offset;
        }
    }
}