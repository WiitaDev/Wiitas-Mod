using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Ranger;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Ranger
{
    public class FlameBlaster : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.crit = 4;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item112;
            Item.shoot = ModContent.ProjectileType<PiercingPegshot>();
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 1f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return player.altFunctionUse == 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            WiitaModPlayer modPlayer = player.GetModPlayer<WiitaModPlayer>();

            if (modPlayer.FlameBlasterAlt == true)
            {
                type = ProjectileID.BeeArrow;
            }
            else
            {
                type = ModContent.ProjectileType<PiercingPegshot>();
            }
        }

        public override bool? UseItem(Player player)
        {
            WiitaModPlayer modPlayer = player.GetModPlayer<WiitaModPlayer>();

            if (player.altFunctionUse == 2)
            {
                modPlayer.FlameBlasterAlt = modPlayer.FlameBlasterAlt != true;

                CombatText.NewText(player.getRect(), modPlayer.FlameBlasterAlt ? Color.OrangeRed : Color.Firebrick, modPlayer.FlameBlasterAlt ? "Piercing Pegshot" : "Flame Blast");
                return true;
            }

            return true;
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
            return new Vector2(-14, -10);
        }
    }
}