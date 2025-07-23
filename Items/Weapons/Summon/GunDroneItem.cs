using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;
using WiitaMod.Projectiles.Summon.GunDrone;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Summon
{
    public class GunDroneItem : ModItem
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

            Item.damage = 75;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.buffType = ModContent.BuffType<GunDroneBuff>();
            Item.shoot = ModContent.ProjectileType<GunDrone>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.maxMinions < 2)
                return false;

            player.AddBuff(Item.buffType, 2, true);

            return true;
        }

        public override bool? UseItem(Player player)
        {
            WiitaModPlayer modPlayer = player.GetModPlayer<WiitaModPlayer>();

            if (player.altFunctionUse == 2)
            {
                modPlayer.GunDroneAlt = modPlayer.GunDroneAlt != true;      
                
                // Visual feedback
                CombatText.NewText(player.getRect(), modPlayer.GunDroneAlt ? Color.OrangeRed : Color.ForestGreen, modPlayer.GunDroneAlt ? "Grenade Launcher Armed" : "Sniper Armed");
            }


            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Reset position to player center
            position = player.Center;

            if (player.GetModPlayer<WiitaModPlayer>().GunDroneAlt)
            {
                knockback = 5;
            }
            else
            {
                knockback = 4;
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HallowedBar, 12);
            recipe.AddIngredient(ItemID.SoulofSight, 10);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.Wire, 30);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}