using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;
using WiitaMod.Projectiles.Summon;

namespace WiitaMod.Items.Weapons.Summon
{
    public class BassSentry : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadowflame Apparition Staff");
            // Tooltip.SetDefault("Summons a shadowflame apparition to fight for you");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DD2BallistraTowerT1Popper);
            Item.width = 60;
            Item.height = 60;
            Item.damage = 15;
            Item.mana = 12;
            Item.knockBack = 2;
            Item.sentry = true;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.shoot = ModContent.ProjectileType<BassSentrySentry>();

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = Main.MouseWorld;

            Projectile.NewProjectile(source, position.X, position.Y, 0, 0, type, damage, knockback, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            Projectile dummy = new Projectile();
            dummy.SetDefaults(Item.shoot);

            Point topLeft = (Main.MouseWorld - dummy.Size / 2).ToTileCoordinates();
            Point bottomRight = (Main.MouseWorld + dummy.Size / 2).ToTileCoordinates();

            return !Collision.SolidTilesVersatile(topLeft.X, bottomRight.X, topLeft.Y, bottomRight.Y);
        }
    }
}