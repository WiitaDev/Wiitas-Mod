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
    public class ShadowflameApparitionStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame Apparition Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("Summons a shadowflame apparition to fight for you");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.width = 60;
            Item.height = 60;

            Item.damage = 35;
            Item.mana = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.buffType = ModContent.BuffType<ShadowflameApparitionBuff>();
            Item.shoot = ModContent.ProjectileType<ShadowflameApparitionMinion>();

        }



        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2, true);
            position = Main.MouseWorld;
            return true;
        }
    }
}