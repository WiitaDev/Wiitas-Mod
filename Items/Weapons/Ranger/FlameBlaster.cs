using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Ranger.FlameBlaster;
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
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.crit = 4;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 50;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = new SoundStyle("WiitaMod/Assets/SFX/PegShot") { MaxInstances = 0 , PitchVariance = 0.2f};
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
                type = ModContent.ProjectileType<FlameBlast>();
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

                CombatText.NewText(player.getRect(), modPlayer.FlameBlasterAlt ? Color.OrangeRed : Color.Firebrick, modPlayer.FlameBlasterAlt ? "Flame Blast" : "Piercing Pegshot");
            }

            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-14, -10);
        }
    }
}