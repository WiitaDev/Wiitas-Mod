using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using System;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Renderers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.CodeAnalysis.CSharp;

namespace WiitaMod.Projectiles
{
    public class TestingProjectile : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{977}";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic slash");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 20 * Projectile.MaxUpdates;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.PrincessWeapon, new ParticleOrchestraSettings
            {
                
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity
            });
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Vector2.Zero
            });
        }


        public override void AI()
        {
            Projectile.alpha = 255;
            Projectile.velocity *= 0.98f;
            if (Main.rand.NextBool(8))
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.DungeonWater, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default(Color), 0.9f);
                dust.noGravity = true;
                dust.position = Projectile.Center;
                dust.velocity = Main.rand.NextVector2Circular(1f, 1f) + Projectile.velocity * 0.5f;
            }

            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 0.75f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.UltimaPurpleTrail).Draw(Projectile);

            return true;
        }

    }

    internal class TestingWeapon : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Muramasa}";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Testing Weapon");
            Tooltip.SetDefault("pls i just want to make cool things");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = 1;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.width = 40;
            Item.height = 40;
            Item.damage = 24;
            Item.scale = 1f;
            Item.UseSound = SoundID.Item1;
            Item.rare = 2;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.knockBack = 3f;
            Item.noMelee = false;
            Item.DamageType = DamageClass.Melee;

        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 v = Main.rand.NextVector2CircularEdge(200f, 200f);
                if (v.Y < 0f)
                {
                    v.Y *= -1f;
                }
                v.Y += 100f;
                Vector2 vector = v.SafeNormalize(Vector2.UnitY) * 9f;
                Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center - vector * 20f, vector, ModContent.ProjectileType<TestingProjectile>(), (int)((double)damage * 0.75), 0f, Main.myPlayer, 0f, target.Center.Y);
            }
        }
    }
}