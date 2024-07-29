using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles;
using WiitaMod.Systems;

namespace WiitaMod.Items.CraftingMaterials
{
	public class IllegalRocketLauncherParts : ModItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Illegal Rocket Launcher Parts");
			// Tooltip.SetDefault("'Banned everywhere'");

			Item.ResearchUnlockCount = 1;

		}

		public override void SetDefaults()
		{
            Item.width = 48;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }

	}
}