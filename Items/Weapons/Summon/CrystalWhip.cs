using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Summon;

namespace WiitaMod.Items.Weapons.Summon
{
    public class CrystalWhip : ModItem
    {
        //public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExampleWhipDebuff.TagDamage);

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<CrystalWhipProjectile>(), 25, 2, 5.5f);
            Item.rare = ItemRarityID.Orange;
        }

        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}