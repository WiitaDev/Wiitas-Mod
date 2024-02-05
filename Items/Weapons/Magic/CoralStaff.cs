using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Items.Weapons.Magic
{
	public class CoralStaff : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Homing Staff");
            //Tooltip.SetDefault("Summons homing projectiles");
            Item.ResearchUnlockCount = 1;

            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.reuseDelay = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CoralStaffHold>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.channel = true;
            player.statMana += Item.mana;
            return true;
        }

        public override void AddRecipes()
        {
            
        }
    }
}