using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Ranger;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.Magic
{
    public class InfernalAlmanacHold : ModProjectile
    {
        // The maximum charge value
        private const float MAX_CHARGE = 20f;
        // The maximum amount of projectiles
        private const float MAX_PROJECTILES = 6f;
        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 15f;

        // The actual charge value is stored in the localAI0 field
        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float ProjectileAmount
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        } 
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 69420;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }

        public override void AI()
        {
            Timer++;
            Player player = Main.player[Projectile.owner];
            Projectile.scale = 0.75f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.Center = new Vector2(player.MountedCenter.X + MOVE_DISTANCE * Projectile.direction, player.MountedCenter.Y);

            UpdatePlayer(player);

            ProjectileAmount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<InfernalAlmanacProj>() && proj.ai[2] == 0)
                {
                    ProjectileAmount++;
                }
            }

            if (!player.channel)
            {
                Projectile.Kill();
                return;
            }

            ChargeWeapon(player);
            if (IsAtMaxCharge) 
            {
                SpawnProjectile(player);
                Charge = 0;
            }
            Projectile.netUpdate = true;
        }

        private void SpawnProjectile(Player player)
        {
            if (Projectile.owner == player.whoAmI && player.heldProj == Projectile.whoAmI && ProjectileAmount != MAX_PROJECTILES && player.CheckMana(player.GetManaCost(player.HeldItem), true, false))
            {
                int SpawnedProjectiles = (int)Projectile.ai[2];
                int projID = -1;

                // Find the first available ID using a bitmask
                for (int i = 0; i < MAX_PROJECTILES; i++)
                {
                    if ((SpawnedProjectiles & (1 << i)) == 0)
                    {
                        projID = i + 1; // IDs start at 1 for consistency
                        Projectile.ai[2] = (float)(SpawnedProjectiles | (1 << i));
                        break;
                    }
                }

                if (projID != -1)
                {
                    player.heldProj = Projectile.whoAmI;
                    Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<InfernalAlmanacProj>(), Projectile.damage, player.HeldItem.knockBack, Projectile.owner, ai0: projID);
                    Projectile.netUpdate = true;
                }
            }
        }

        private void ChargeWeapon(Player player)
        {
            if (Charge < MAX_CHARGE && player.channel)
            {
                Charge++;
            }

            for (int i = 1; i < ProjectileAmount + 1; i++)
            {
                double deg = Timer * 2 + i * 60;
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double dist = 10; //Distance away from the target

                Vector2 adjustedPosition = Projectile.Center;

                adjustedPosition.X = Projectile.Center.X - (int)(Math.Cos(rad) * dist);
                adjustedPosition.Y = Projectile.Center.Y - (int)(Math.Sin(rad) * dist);

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
            player.heldProj = -1;
            player.channel = false;
        }
    }
}