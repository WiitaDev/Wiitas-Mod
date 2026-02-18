using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;

namespace WiitaMod.Projectiles.Summon.GunDrone
{
    public class GunDrone : ModProjectile
    {
        private NPC target;
        private Vector2 hoverPosition;
        public ref float shootCooldown => ref Projectile.ai[0];

        private Vector2 PrimaryWeaponOffset = new Vector2(0, 20);
        private Vector2 SecondaryWeaponOffset = new Vector2(0, 20);
        private bool IsSecondary => Projectile.knockBack == 5;

        private float gunRotation = 0f;
        private Vector2 gunPosition;

        private int reloadFrame;
        private int reloadTimer;
        private bool isReloading;
        private const int AnimationSpeed = 4; // Ticks per frame
        private const int TotalFrames = 5; // Total animation frames

        private const float MaxSpeed = 10f;
        private const float Acceleration = 0.85f;

        private const float EngagementDistance = 1200f;
        private const float StabilityThreshold = 50f;

        private const float TeleportDistance = 2000f;
        private const float IdleVerticalOffset = -150f;
        private const float BaseOrbitRadius = 50f;
        private const float RadiusPerDrone = 10f;


        private int DroneIndex => GetDroneIndex();
        private int GetDroneIndex()
        {
            int index = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (other.active && other.type == Projectile.type && other.owner == Projectile.owner)
                {
                    if (other.whoAmI < Projectile.whoAmI) index++;
                }
            }
            return index;
        }

        private int TotalDrones => CountActiveDrones();
        private int CountActiveDrones()
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == Projectile.owner && proj.type == Projectile.type)
                    count++;
            }
            return count;
        }

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 118;
            Projectile.height = 54;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.knockBack = 3f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 2f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            shootCooldown = IsSecondary ? 70f : 120;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Main.myPlayer == player.whoAmI && Projectile.Distance(player.Center) > TeleportDistance)
            {
                Projectile.position = player.Center + new Vector2(-player.direction * 100, IdleVerticalOffset);
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            UpdatePlayerStatus(player);
            TargetEnemyWithLOS(player);

            if (target != null && target.active)
            {
                EngageTarget(player);
            }

            IdleBehavior(player);
            SmoothMovement();
            UpdateRotation();
            UpdateAnimation();

            gunPosition = Projectile.Center + (IsSecondary ? SecondaryWeaponOffset : PrimaryWeaponOffset).RotatedBy(Projectile.rotation);

            if (target != null && target.active)
            {
                // Calculate weapon rotation
                Vector2 direction = target.Center - gunPosition;
                gunRotation = direction.ToRotation();
            }
            else
            {
                gunRotation = Projectile.rotation / 2;
            }

            shootCooldown = MathHelper.Max(shootCooldown - 1f, 0);
        }


        private void TargetEnemyWithLOS(Player player)
        {
            // Check for player's priority target first
            if (player.HasMinionAttackTargetNPC)
            {
                NPC priorityTarget = Main.npc[player.MinionAttackTargetNPC];
                if (priorityTarget.CanBeChasedBy() && (priorityTarget.Distance(Projectile.Center) < EngagementDistance * 1.5f))
                {
                    target = priorityTarget;
                    return;
                }
            }

            // Original targeting logic
            target = Main.npc
                .Where(n => n.active && !n.friendly && n.CanBeChasedBy() &&
                      n.Distance(Projectile.Center) < EngagementDistance &&
                      (Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1) ||
                       Collision.CanHitLine(player.Center, 1, 1, n.Center, 1, 1)))
                .OrderBy(n => n.Distance(Projectile.Center))
                .FirstOrDefault();
        }

        private void EngageTarget(Player player)
        {
            if (target == null) return;

            if (shootCooldown <= 0)
            {
                ShootAtTarget();
                shootCooldown = IsSecondary ? 70f : 120f;
            }
        }

        private void IdleBehavior(Player player)
        {
            float radius = BaseOrbitRadius + (RadiusPerDrone * TotalDrones);
            radius = Math.Min(radius, 100f); // Cap maximum radius

            float baseTime = Main.GameUpdateCount * 0.016666667f; // Convert ticks to seconds
            float angle = (baseTime * 0.75f) + (MathHelper.TwoPi * DroneIndex / TotalDrones);

            Vector2 orbitOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
            hoverPosition = player.Center + orbitOffset + new Vector2(0, IdleVerticalOffset);

            MoveTowards(hoverPosition, 0.5f);
        }

        private void MoveTowards(Vector2 destination, float accelerationMultiplier = 1f)
        {
            Vector2 toDestination = destination - Projectile.Center;

            if (toDestination.Length() > 10f)
            {
                toDestination.Normalize();
                Projectile.velocity += toDestination * Acceleration * accelerationMultiplier;
            }
        }

        private void ShootAtTarget()
        {
            if (target == null || !target.active) return;

            // Get shoot position from weapon tip
            Texture2D gunTexture = IsSecondary
            ? ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDroneGrenadeLauncher", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value
            : ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDroneSniper", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Vector2 muzzleOffset = new Vector2(gunTexture.Width / 3 * 2, 0).RotatedBy(gunRotation);
            Vector2 shootPosition = Projectile.Center + (IsSecondary ? SecondaryWeaponOffset : PrimaryWeaponOffset).RotatedBy(Projectile.rotation) + muzzleOffset;

            Vector2 shootDirection = (target.Center - shootPosition).SafeNormalize(Vector2.UnitY);

            int projectileType = IsSecondary
            ? ModContent.ProjectileType<GunDroneGrenade>()
            : ProjectileID.BulletHighVelocity;

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile proj = Main.projectile[Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    shootPosition,
                    shootDirection * (IsSecondary ? 17.5f : 15f),
                    projectileType,
                    IsSecondary ? Projectile.damage : Projectile.damage * 2,
                    Projectile.knockBack,
                    Projectile.owner
                )];

                proj.CritChance = 0;
                proj.penetrate = IsSecondary ? proj.penetrate : 1;
                proj.minion = true;
                proj.DamageType = DamageClass.Summon;
            }

            Projectile.velocity = -shootDirection * (IsSecondary ? 5f : 10f); //Recoil effect

            SoundEngine.PlaySound(IsSecondary ? SoundID.Item61.WithVolumeScale(0.65f) : SoundID.Item38.WithVolumeScale(0.65f), Projectile.position);

            for (int i = 0; i < 15; i++)
            {
                short dustID = Main.rand.NextBool() ? DustID.Smoke : DustID.Torch;
                Dust.NewDust(shootPosition, 2, 2, dustID, shootDirection.RotatedByRandom(0.1f).X * Main.rand.NextFloat(0.5f, 1.5f), shootDirection.RotatedByRandom(0.1f).Y * Main.rand.NextFloat(0.5f, 1.5f));
            }

            if (IsSecondary)
            {
                // Start reload animation
                isReloading = true;
                reloadFrame = 1;
                reloadTimer = 0;
            }
        }

        public override void PostDraw(Color lightColor)
        {
            // Glowsprite
            Texture2D droneGlow = ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDrone_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(droneGlow, Projectile.Center - Main.screenPosition, null, Color.White,Projectile.rotation, new Vector2(droneGlow.Width / 2, droneGlow.Height / 2), 1f, SpriteEffects.None, 0);


            Texture2D gunTexture = IsSecondary
            ? ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDroneGrenadeLauncher", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value
            : ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDroneSniper", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int frameHeigth = gunTexture.Height / TotalFrames;
            int currentFrame = isReloading ? reloadFrame : 0;

            Rectangle sourceRect = new Rectangle(0, frameHeigth * currentFrame, gunTexture.Width, frameHeigth);

            Vector2 origin = new Vector2(gunTexture.Width / 3, IsSecondary ? frameHeigth / 2 : gunTexture.Height / 2);

            SpriteEffects effects = SpriteEffects.None;
            if (gunRotation < -MathHelper.PiOver2 || gunRotation > MathHelper.PiOver2)
            {
                effects = SpriteEffects.FlipVertically;
            }

            Main.EntitySpriteDraw( // draw weapon
                gunTexture,
                gunPosition - Main.screenPosition,
                IsSecondary ? sourceRect : null,
                lightColor,
                gunRotation,
                origin,
                1f,
                effects,
                0
            );

            // draw gun glowsprites
            Texture2D gunGlow = IsSecondary 
            ? ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDroneGrenadeLauncher_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value
            : ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDroneSniper_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Main.EntitySpriteDraw(gunGlow, gunPosition - Main.screenPosition, null, Color.White, gunRotation, origin, 1f, effects, 0);


            Texture2D ringTexture = ModContent.Request<Texture2D>("WiitaMod/Projectiles/Summon/GunDrone/GunDroneRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw( //draw the white effect
                ringTexture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor * 0.3f,
                Projectile.rotation,
                new Vector2(ringTexture.Width / 2, ringTexture.Height / 2),
                1f,
                SpriteEffects.None,
                0
            );
        }

        private void SmoothMovement()
        {
            // Smooth deceleration as approaching position
            float distance = Vector2.Distance(Projectile.Center, hoverPosition);
            float speedFactor = MathHelper.Clamp(distance / StabilityThreshold, 0.1f, 1f);

            Projectile.velocity *= 0.96f * speedFactor;

            // Hard speed limit with smooth clamping
            if (Projectile.velocity.Length() > MaxSpeed)
            {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) *
                    MathHelper.Lerp(Projectile.velocity.Length(), MaxSpeed, 0.1f);
            }
        }

        private void UpdateRotation()
        {
            // Smooth rotation with momentum damping
            float targetRotation = Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.Length() > 0.5f)
            {
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.1f);
            }
            else
            {
                Projectile.rotation *= 0.9f;
            }
        }

        private void UpdatePlayerStatus(Player player)
        {
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<GunDroneBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<GunDroneBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }

        private void UpdateAnimation()
        {
            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // For the grenade launcher animation
            if (isReloading)
            {
                if (++reloadTimer >= AnimationSpeed)
                {
                    reloadTimer = 0;
                    if (++reloadFrame >= TotalFrames)
                    {
                        reloadFrame = 0; // Return to default frame
                        isReloading = false;
                    }
                }
            }
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;
    }
}