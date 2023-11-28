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
    public class CosmicSentry : ModItem
    {
        const float MAX_DISTANCE = 600f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadowflame Apparition Staff");
            // Tooltip.SetDefault("Summons a shadowflame apparition to fight for you");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.width = 60;
            Item.height = 60;
            Item.damage = 65;

            Item.mana = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 2;
            Item.sentry = true;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.shoot = ModContent.ProjectileType<CosmicSentrySentry>();

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = Main.MouseWorld;
            if (MouseTooFar(player))
                position = player.DirectionTo(position) * MAX_DISTANCE;

            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            if (MouseTooFar(player))
                return false;

            Projectile dummy = new Projectile();
            dummy.SetDefaults(Item.shoot);

            Point topLeft = (Main.MouseWorld - dummy.Size / 2).ToTileCoordinates();
            Point bottomRight = (Main.MouseWorld + dummy.Size / 2).ToTileCoordinates();

            return !Collision.SolidTilesVersatile(topLeft.X, bottomRight.X, topLeft.Y, bottomRight.Y);
        }
        private bool MouseTooFar(Player player) => player.Distance(Main.MouseWorld) >= MAX_DISTANCE;
    }
}