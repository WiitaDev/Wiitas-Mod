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
    public class InfernalAlmanacProj : ModProjectile
    {
        public float Timer;

        public ref float ProjectileNum => ref Projectile.ai[1];

        bool flag = false;
        bool maxCharge = false;
        public bool Channeling = true;
        public Projectile HeldProj;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;        //The recording mode

            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 64;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = 0;
            Projectile.knockBack = 4f;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                HeldProj = Main.projectile[player.heldProj];
                Timer = HeldProj.ai[0];
            }

            CircleAround(player); // set position into orbit before spawning dust

            SoundEngine.PlaySound(SoundID.Item20, player.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Lava, Main.rand.NextVector2CircularEdge(Main.rand.Next(2, 5), Main.rand.Next(2, 5)), 0, default, 1.25f);
                d.fadeIn = 0.1f;
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            for (int i = 0; i < 15; i++)
            {
                Vector2 circle = Main.rand.NextVector2Circular(2f, 2f);
                int dustHit = Dust.NewDust(Projectile.Center, 1, 1, DustID.Torch, circle.X + Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 1f);
                Main.dust[dustHit].scale = (float)Main.rand.Next(135, 190) * 0.013f;
                Main.dust[dustHit].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item20.WithPitchOffset(-0.5f), Projectile.Center);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                Timer = HeldProj.ai[0];
            }

            if (!player.channel)
            {
                Channeling = false;
            }

            float maxDetectRadius = 250f; // The maximum radius at which a projectile can detect a target
            float speed = 12f; // The speed at which the projectile moves towards the target
            float turnSpeed = 250f;

            if (Channeling == true)
            {
                Projectile.rotation = 0;
                Projectile.friendly = false;
                Projectile.timeLeft = 300;
                Projectile.velocity = Vector2.Zero;
                CircleAround(player);
            }
            else
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction) - 1.57f * Projectile.direction;
                if (!flag)
                {
                    Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * speed;
                    Projectile.friendly = true;

                    int s = player.GetModPlayer<ModGlobalPlayer>().InfernalAlmanacProjectiles;
                    string newAmount = s.ToString().Replace(ProjectileNum.ToString(), string.Empty);
                    player.GetModPlayer<ModGlobalPlayer>().InfernalAlmanacProjectiles = int.Parse(newAmount);
                    if (HeldProj.localAI[1] == 6) {
                        maxCharge = true;
                        Projectile.penetrate = 2; 
                    }


                    flag = true;
                }

                // Trying to find NPC closest to the projectile
                NPC closestNPC = FindClosestNPC(maxDetectRadius);
                if (closestNPC == null)
                {
                    Projectile.timeLeft -= 2;
                }
                else if(maxCharge)
                {
                    // Homing calculations
                    Vector2 targetPos = closestNPC.Center - Projectile.Center;
                    float length = targetPos.Length();
                    targetPos.Normalize();

                    Projectile.velocity = (Projectile.velocity * 20f + targetPos * (turnSpeed - length * 0.15f)) / 21f;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= speed;
                }
            }

            Vector2 circle = Main.rand.NextVector2Circular(3f, 3f);
            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Torch, circle.X, circle.X, 0, default(Color), 1.25f);
            Main.dust[dust].noGravity = true;

            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 6;
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

            Projectile.netUpdate = true;
        }

        private void CircleAround(Player player)
        {
            double deg = Timer * 2 + ProjectileNum * 60; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
            double rad = deg * (Math.PI / 180); //Convert degrees to radians
            double dist = 100; //Distance away from the target

            Vector2 adjustedPosition = player.Center;

            Projectile.position.X = adjustedPosition.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = adjustedPosition.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.FireTrail).Draw(Projectile);

            return true;
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}