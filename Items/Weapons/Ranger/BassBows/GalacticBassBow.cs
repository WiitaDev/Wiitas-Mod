using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Items.Weapons.Ranger.BassBows
{
    public class GalacticBassBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Bass Bow");
            /* Tooltip.SetDefault("Arrows turn into Cosmic Bass" +
							 "\nWhen you hit an enemy with Cosmic Bass, it creates a ring around the enemy" +
							 "\nThe ring shoots outs Cosmic projectiles that deal 75% damage of the weapon"); */

            Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }

        public override void SetDefaults()
        {

            Item.shootSpeed = 30f;

            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.reuseDelay = 10;
            Item.shootSpeed = 30f;
            Item.knockBack = 1f;
            Item.width = 34;
            Item.height = 34;
            Item.damage = 90;
            Item.UseSound = SoundID.Item2;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 20);

        }
    }
}