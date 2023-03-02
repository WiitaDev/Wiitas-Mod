using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;
using WiitaMod.Projectiles;

namespace WiitaMod.Items.Pets
{
    internal class HamisPetItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chilly Egg");
            Tooltip.SetDefault("Summons a Hamis");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<HamisPet>();
            Item.width = 16;
            Item.height = 30;
            Item.UseSound = SoundID.Item2;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.rare = ItemRarityID.Orange;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.buffType = ModContent.BuffType<HamisPetBuff>();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            {
                if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                {
                    player.AddBuff(Item.buffType, 3600);
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center.X, player.Center.Y, 0, 0, ModContent.ProjectileType<HamisPet>(), 0, 0, player.whoAmI, 0f);
            return false;
        }
    }
}