using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles;
using WiitaMod.Systems;

namespace WiitaMod.Items.Tools
{
    public class DeepcoreFlareGun : ModItem
    {
        private const int cooldown = 1 * 60;

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
            Item.damage = 0;
            Item.shoot = ModContent.ProjectileType<DeepcoreFlareProj>();
            Item.shootSpeed = 11f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item61;
            Item.useTime = 20;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FlareGun, 1);
            recipe.AddRecipeGroup("PrehardTier2", 12);
            recipe.AddIngredient(ItemID.Glowstick, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, 0);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.GetModPlayer<WiitaModPlayer>().deepcoreFlareTimer > 0)
                return false;
            return true;
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<WiitaModPlayer>().deepcoreFlareTimer = cooldown;
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Player player = Main.LocalPlayer;
            if (player.GetModPlayer<WiitaModPlayer>().deepcoreFlareTimer <= 0)
                return;

            float remaining = player.GetModPlayer<WiitaModPlayer>().deepcoreFlareTimer / 60f;
            if (remaining > 0)
            {
                // Position the timer where the stack count would be
                Vector2 textPos = position + new Vector2(-30, 20) * scale; // Adjust offset as needed
                string text = $"{remaining:0.0}s";

                // Draw the timer text
                spriteBatch.DrawString(
                    FontAssets.ItemStack.Value,
                    text,
                    textPos,
                    Color.Lerp(Color.White, Color.Red, remaining / 10f), // Color gradient
                    0f,
                    Vector2.Zero,
                    scale * 2f,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}