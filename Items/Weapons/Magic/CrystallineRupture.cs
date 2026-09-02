using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Magic;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Magic
{
    public class CrystallineRupture : ModItem
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
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.mana = 15;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.knockBack = 6f;
            Item.width = 28;
            Item.height = 30;
            Item.UseSound = SoundID.Item100;
            Item.shoot = ModContent.ProjectileType<CrystallineRuptureHold>();
            Item.shootSpeed = 15f;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
    }
}