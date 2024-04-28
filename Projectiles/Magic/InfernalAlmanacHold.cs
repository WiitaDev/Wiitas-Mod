using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.Magic
{
    public class InfernalAlmanacHold : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        // The maximum charge value
        private const float MAX_CHARGE = 20f;
        // The maximum amount of projectiles
        private const float MAX_PROJECTILES = 6f;
        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 25f;

        // The actual charge value is stored in the localAI0 field
        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float ProjectileAmount
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        } 
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;
            Timer++;
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center + Projectile.velocity * MOVE_DISTANCE;

            if (!player.channel)
            {             
                Projectile.Kill();
            }

            UpdatePlayer(player);
            ChargeWeapon(player);
            if (IsAtMaxCharge) 
            {
                SpawnProjectile(player);
                Charge = 0;
            }
        }

        private void SpawnProjectile(Player player)
        {
            if (Main.myPlayer == player.whoAmI && ProjectileAmount != MAX_PROJECTILES && player.CheckMana(player.GetManaCost(player.HeldItem), true, false))
            {                
                int SpawnedProjectiles = player.GetModPlayer<ModGlobalPlayer>().InfernalAlmanacProjectiles;
                int projID = 0;
                for (int i = 1; i < MAX_PROJECTILES + 1; i++) {
                    if (!SpawnedProjectiles.ToString().Contains(i.ToString()))
                    {
                        string newNumbers = SpawnedProjectiles.ToString().Insert(i - 1, i.ToString());
                        player.GetModPlayer<ModGlobalPlayer>().InfernalAlmanacProjectiles = int.Parse(newNumbers);
                        projID = i;
                        break;
                    }
                }
                
                Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<InfernalAlmanacProj>(), Projectile.damage, player.HeldItem.knockBack, Main.myPlayer, ai1: projID);
                ProjectileAmount++;
            }
        }

        private void ChargeWeapon(Player player)
        {
            Vector2 offset = Projectile.velocity;
            offset *= MOVE_DISTANCE - 20;
            Vector2 pos = player.Center + offset - new Vector2(10, 10);

            if (Charge < MAX_CHARGE && player.channel)
            {
                Charge++;
            }

            for (int i = 1; i < ProjectileAmount + 1; i++)
            {
                double deg = Timer * 2 + i * 60; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double dist = 10; //Distance away from the target

                Vector2 adjustedPosition = Projectile.Center;

                adjustedPosition.X = Projectile.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
                adjustedPosition.Y = Projectile.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                Dust dust = Dust.NewDustPerfect(adjustedPosition, DustID.Torch);
                dust.scale = Main.rand.Next(10, 20) * 0.04f;
                dust.noGravity = true;
            }

        }

        private void UpdatePlayer(Player player)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
                Projectile.velocity = aim;
            }

            int dir = Projectile.direction;
            player.ChangeDir(dir); // Set player direction to where we are shooting
            player.heldProj = Projectile.whoAmI; // Update player's held projectile
            player.itemTime = 2; // Set item time to 2 frames while we are used
            player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * dir, Projectile.velocity.X * dir); // Set the item rotation to where we are shooting
        }

        public override bool ShouldUpdatePosition() => false;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.channel = false;
        }
    }
}