using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Projectiles.Pets
{
    public class HamisPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            //DrawOffsetX = -20;
            // DisplayName.SetDefault("Hamis Pet");
            Main.projFrames[Projectile.type] = 10;

            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MiniMinotaur);
            AIType = ProjectileID.MiniMinotaur;
            Projectile.width = 22;
            Projectile.height = 19; // the height is 1 pixel lower because otherwise it had a row of pixels visible on top of it. And this works fine...
            Projectile.scale = 1.5f;
            DrawOriginOffsetY = 4;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.miniMinotaur = false; // Relic from AIType
            return true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            NukeTheFrogs();
            if (player.dead)
            {
                player.GetModPlayer<ModGlobalPlayer>().HamisPetEquipped = false;
            }
            if (player.GetModPlayer<ModGlobalPlayer>().HamisPetEquipped)
            {
                Projectile.timeLeft = 2;
            }
        }

        public void NukeTheFrogs() 
        {
            Player player = Main.player[Projectile.owner];
            float distanceFromTarget = 700f;
            Vector2 targetCenter = Projectile.position;
            bool foundTarget = false;

            if (NPC.CountNPCS(NPCID.Frog) > 0 || NPC.CountNPCS(NPCID.GoldFrog) > 0 || NPC.CountNPCS(687) > 0)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && (npc.type == NPCID.Frog || npc.type == NPCID.GoldFrog || npc.type == 687))
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright

                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            if (foundTarget)
            {
                Projectile.ai[2]++;

                if (Projectile.ai[2] >= 60)
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Vector2 direction = targetCenter - Projectile.Center;
                        direction.Normalize();
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 10, ModContent.ProjectileType<HamisNuke>(), 1, 0f, Projectile.owner);
                        Projectile.ai[2] = 0;
                    }
                }
            }
        }
    }

    public class HamisNuke : ModProjectile 
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.MiniNukeRocketI}";
        public override void SetStaticDefaults()
        {

            //DrawOffsetX = -20;
            // DisplayName.SetDefault("Hamis Nuke");
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.damage = 0;
            Projectile.penetrate = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Projectile.penetrate == 2) 
            {
                Projectile.penetrate = -1;
                return false;
            }
            else 
            {
                return true;
            }
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y * (float)Projectile.direction, Projectile.velocity.X * (float)Projectile.direction) + MathHelper.ToRadians(90f) * Projectile.direction;
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.penetrate != 2)
                {
                    Projectile.alpha = 255;
                    Projectile.Resize(250, 250);
                    Projectile.penetrate = -1;
                    Projectile.timeLeft = 3;
                }
            }

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1f);
            dust.scale = 0.1f + Main.rand.Next(5) * 0.1f;
            dust.fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
            dust.noGravity = true;
            dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;

            dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
            dust.scale = 1f + Main.rand.Next(5) * 0.1f;
            dust.noGravity = true;
            dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            // Fire Dust spawn
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }
            for (int g = 0; g < 2; g++)
            {
                var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y -= 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y -= 1.5f;
            }
        }
    }
}
