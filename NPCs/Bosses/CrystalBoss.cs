using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles;
using WiitaMod.Systems;

namespace WiitaMod.NPCs.Bosses
{
    [AutoloadBossHead]
    public class CrystalBoss : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis;
        }

        public ref float AttackType => ref NPC.ai[0];


        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath56.WithPitchOffset(-0.75f);
            NPC.width = 38;
            NPC.height = 94;
            NPC.defense = 5;
            NPC.damage = 10;
            NPC.scale = 1.25f;
            NPC.knockBackResist = 0f;
            NPC.value = 50000;
            NPC.lifeMax = 4000;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Amethyst");
            }
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.White.ToVector3());


            Player player = Main.player[NPC.target];

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();



            if (Main.rand.NextBool(40)) 
            {
                Vector2 dir = player.Center - NPC.Center;
                dir.Normalize();
                dir *= 5f;

                SoundEngine.PlaySound(SoundID.Item109.WithVolumeScale(0.75f).WithPitchOffset(0.1f), NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir,ModContent.ProjectileType<CrystalShot>(), NPC.damage, 2f, ai1: NPC.whoAmI);
                    Main.projectile[proj].friendly = false;
                    Main.projectile[proj].hostile = true;
                
                }
            }

            if (player.dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > 150) // Despawn if farther than 150 block or if player dead
            {
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 20 ticks
                NPC.EncourageDespawn(20);
                return;
            }

           

            if (Main.player[NPC.target] != null && AttackType == 0) 
            {
                float hoverHeight = -210f;

                float t = Main.GameUpdateCount * 0.03f;
                float rotation = Main.GameUpdateCount * 0.002f;


                // Infinity symbol shape
                Vector2 offset = new Vector2((float)Math.Sin(t) * 70, (float)Math.Sin(t) * (float)Math.Cos(t) * 70);
                offset = offset.RotatedBy(rotation);
                offset.X *= 1.5f;

                Vector2 hoverPosition = player.Center + new Vector2(0, hoverHeight);

                // Direction toward hover position
                Vector2 toPosition = hoverPosition + offset - NPC.Center;
                if (toPosition != Vector2.Zero)
                    toPosition.Normalize();

                float speed = 7f;
                float inertia = 25f;


                Vector2 desiredVelocity = toPosition * speed;

                // Smooth movement
                NPC.velocity = (NPC.velocity * (inertia - 1) + desiredVelocity) / inertia;
                NPC.rotation = NPC.velocity.X * 0.03f;
            }

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Meth."),
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
                for (int i = 0; i < 10; i++)
                {
                    int dustHit = Dust.NewDust(NPC.Center, 1, 1, DustID.Blood, (float)Main.rand.Next(-3, 3), (float)Main.rand.Next(-3, 3), 0, default(Color), 1f);
                    Main.dust[dustHit].scale = (float)Main.rand.Next(100, 135) * 0.013f;
                }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Amethyst, 1, 1, 5));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
            return true;
        }

    }
}
