using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.Ranger.BassArrows.CosmicBassBow
{
    public class CosmicBassBowHold : ModProjectile
    {

        // The maximum charge value
        private const float MAX_CHARGE = 180f;
        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 70f;

        // The actual charge value is stored in the localAI0 field
        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public float BurstTimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        private float turnSpeed = 0.2f;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hide = true;
            Projectile.timeLeft = 420;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center + Projectile.velocity * MOVE_DISTANCE;

            UpdatePlayer(player);
            ChargeBow(player);

            if (Charge <= 20 && !player.channel)
            {
                ShootSingle(player);
                Projectile.Kill();
            }
            if (Charge > 20 && !IsAtMaxCharge && !player.channel)
            {
                int chargeFact = (int)(Charge / 60f);
                if (chargeFact > 2) chargeFact = 2;
                for (int i = 0; i <= chargeFact; i++)
                {
                    if (BurstTimer == 10 && i == 0 || BurstTimer == 20 && i == 1 || BurstTimer == 30 && i == 2)
                    {
                        ShootBurst(player);
                        if (i == chargeFact)
                        {
                            Projectile.Kill();
                        }
                    }
                }
                BurstTimer++;
            }
            if (IsAtMaxCharge && !player.channel || Projectile.timeLeft == 1)
            {
                ShootBig(player);
                Projectile.Kill();
            }

            if (IsAtMaxCharge)
            {
                turnSpeed = 0.1f;
            }
        }

        private void ShootSingle(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.position, Projectile.velocity * 30, ModContent.ProjectileType<CosmicBassArrow>(), Projectile.damage / 2, 0, Main.myPlayer);
        }
        private void ShootBurst(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item74.WithPitchOffset(-0.5f), player.Center);
            if (Main.myPlayer == player.whoAmI)
            {
                float numberProjectiles = 2; // 3 shots
                float rotation = MathHelper.ToRadians(3);//Shoots them in a 3 degree radius.
                int ProjAi = 0; // This flips the Arrows sprite direction in its code
                Projectile.position += Vector2.Normalize(Projectile.velocity * 3f); //3 should equal whatever number you had on the previous line

                for (int k = 0; k < numberProjectiles; k++)
                {
                    Vector2 perturbedSpeed = Projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, k / numberProjectiles)); // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                    Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.position, perturbedSpeed * 30f, ModContent.ProjectileType<CosmicBassArrow>(), Projectile.damage, 0.25f, Main.myPlayer, ai1: ProjAi); //Creates a new projectile with our new vector for spread.
                    ProjAi = 100;
                }
                Main.player[Projectile.owner].GetModPlayer<ModGlobalPlayer>().screenShakeTimerGlobal = -80;
                Main.player[Projectile.owner].GetModPlayer<ModGlobalPlayer>().screenShakeVelocity = 100 + (int)Charge;
            }
        }

        private void ShootBig(Player player)
        {
            Main.player[Projectile.owner].GetModPlayer<ModGlobalPlayer>().screenShakeTimerGlobal = -75;
            SoundEngine.PlaySound(SoundID.Item74.WithPitchOffset(-0.33f), player.Center);

            Vector2 offset = Projectile.velocity;
            offset *= MOVE_DISTANCE - 20;
            Vector2 pos = player.Center + offset - new Vector2(10, 10);

            for (int i = 0; i < 30; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(pos, 8, 8, DustID.PurpleCrystalShard, 0, 0, 100, new Color(), 1.25f)];
                dust.noGravity = true;
                dust.velocity *= Main.rand.Next(10, 21) * 0.1f;
            }
            float Damage = Projectile.damage * 2.5f;
            Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.position, Projectile.velocity * 30, ModContent.ProjectileType<BigCosmicBassArrow>(), (int)Damage, 0, Main.myPlayer);
        }

        private void ChargeBow(Player player)
        {
            Vector2 offset = Projectile.velocity;
            offset *= MOVE_DISTANCE - 20;
            Vector2 pos = player.Center + offset - new Vector2(10, 10);
            if (Charge < MAX_CHARGE && player.channel)
            {
                Charge++;
            }
            int chargeFact = (int)(Charge / 10f);
            Vector2 dustVelocity = Vector2.UnitX * 18f;
            dustVelocity = dustVelocity.RotatedBy(Projectile.rotation - 1.57f);
            Vector2 spawnPos = Projectile.Center + dustVelocity;
            for (int k = 0; k < chargeFact + 1; k++)
            {
                Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (30f - chargeFact);
                Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, DustID.PurpleCrystalShard, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f)];
                dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (30f - chargeFact) / 10f;
                dust.scale = Main.rand.Next(10, 20) * 0.06f;
                dust.noGravity = true;
            }

            SoundStyle ChargeSound = SoundID.Item15 with
            {
                Volume = 0.7f,
                Pitch = Charge * 0.002f - 0.3f,
                MaxInstances = 10,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            };

            if (!IsAtMaxCharge && (int)Charge % 10 == 0) // Charge sound and screeen shake
            {
                SoundEngine.PlaySound(ChargeSound, player.Center);
                Main.player[Projectile.owner].GetModPlayer<ModGlobalPlayer>().screenShakeTimerGlobal = -80;
                Main.player[Projectile.owner].GetModPlayer<ModGlobalPlayer>().screenShakeVelocity = 100 + (int)Charge;
            }

            float chargesoundFact = Charge / 60f; //Charge upgrade sound
            if (Charge == 20 || chargesoundFact == 1 || chargesoundFact == 2 || Charge == MAX_CHARGE - 1)
            {
                SoundEngine.PlaySound(SoundID.Item105.WithPitchOffset(chargesoundFact * 0.1f - 0.1f).WithVolumeScale(0.8f), player.Center);
            }
            if(Charge == MAX_CHARGE - 1) 
            {
                for (int i = 0; i < 100; i++)
                {
                    Dust d2 = Dust.NewDustPerfect(player.Center, DustID.PurpleCrystalShard, Main.rand.NextVector2CircularEdge(Main.rand.Next(7,11), Main.rand.Next(7, 11)), 0, Color.Purple, 2.25f);
                    d2.fadeIn = 0.1f;
                    d2.noGravity = true;
                }
            }

        }

        private void UpdatePlayer(Player player)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
                if (aim.HasNaNs())
                {
                    aim = -Vector2.UnitY;
                }

                aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, turnSpeed)); // last variable is the turn speed
                aim *= 1f;

                if (aim != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = aim;
                Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                Projectile.netUpdate = true;
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