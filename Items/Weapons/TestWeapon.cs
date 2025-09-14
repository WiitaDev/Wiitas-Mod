using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Ranger;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons
{
    public class TestWeapon : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.gunProj[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 480;
            Item.crit = 20;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 92;
            Item.height = 34;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 22, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.AbigailAttack;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<TestWeaponHold>();
            Item.shootSpeed = 6f;
        }


        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<TestWeaponHold>()] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.channel = true;
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<TestWeaponHold>();
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
    }
}