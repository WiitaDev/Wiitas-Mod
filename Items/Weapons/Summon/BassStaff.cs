using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;
using WiitaMod.Projectiles.Summon;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Summon
{
    public class BassStaff : ModItem
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
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.width = 60;
            Item.height = 60;

            Item.damage = 10;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.buffType = ModContent.BuffType<BassSummonBuff>();
            Item.shoot = ModContent.ProjectileType<BassSummon>();

        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            player.AddBuff(Item.buffType, 2, true);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bass, 5);
            recipe.AddIngredient(ItemID.BabyBirdStaff, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

    }
}