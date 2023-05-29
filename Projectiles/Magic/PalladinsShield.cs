using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.Magic
{
    public class PalladinsShieldProjectile: ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.PaladinsShield}";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Palladin's shield");     //The English name of the projectile
        }

        public override void SetDefaults()
        {
            Projectile.scale = 10f;
            Projectile.width = 28;               //The width of projectile hitbox
            Projectile.height = 38;              //The height of projectile hitbox
            Projectile.aiStyle = 0;             //The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true;         //Can the projectile deal damage to enemies?
            Projectile.hostile = false;         //Can the projectile deal damage to the player?
            Projectile.penetrate = -1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.light = 2f;            //How much light emit around the projectile
            Projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false;          //Can the projectile collide with tiles?
            AIType = ProjectileID.BulletHighVelocity;           //Act exactly like default Bullet
            DrawOriginOffsetY = 175;
            DrawOffsetX = 100;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Main.player[Projectile.owner].GetModPlayer<ModGlobalPlayer>().screenShakeTimerGlobal = -90;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage += (target.lifeMax / 10) - 1;
            modifiers.DisableCrit();

            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item37.WithPitchOffset(-1f), Projectile.Center);
        }
    }

    internal class PalladinsShield : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.PaladinsShield}";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Palladin's shield");
            // Tooltip.SetDefault("Make big palladin's shield that do damage :)\nThe palladin's shield deals 10% of the targets health");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.knockBack = 1;
            Item.value = Item.buyPrice(10, 0, 0, 0);
            Item.rare = ItemRarityID.Master;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 50f;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.mana = 0;
            Item.damage = 1;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<PalladinsShieldProjectile>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.PaladinsShield, 1);
            recipe.AddIngredient(ItemID.PlatinumCoin, 100);
            recipe.AddTile(TileID.TreeNymphButterflyJar);
            recipe.AddTile(TileID.VoidMonolith);
            recipe.Register();
        }
    }
}