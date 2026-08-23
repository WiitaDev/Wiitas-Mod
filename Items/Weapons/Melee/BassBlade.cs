using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Melee;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Melee
{
    public class BassBlade : ModItem
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

            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 26;
            Item.useTime = 32;
            Item.knockBack = 6f;
            Item.width = 48;
            Item.height = 48;
            Item.scale = 1.1f;
            Item.useTurnOnAnimationStart = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<BassProjectile>();
            Item.value = Item.sellPrice(0, 0, 20);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = new(2.5f * player.direction, -3);
            position += new Vector2(12 * player.direction, -22);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Stinky, 180);
            target.AddBuff(BuffID.Wet, 180);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            player.AddBuff(BuffID.Stinky, 300);
            player.AddBuff(BuffID.Wet, 300);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bass, 5);
            recipe.AddIngredient(ItemID.WoodenSword, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}