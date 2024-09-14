using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Projectiles.Magic;
using WiitaMod.Systems.BossSystems;

namespace WiitaMod.NPCs.Bosses
{
    [AutoloadBossHead]
    public class CrabBoss : ModNPC
    {
        private enum AttackTypes
        {
            BouncyProjectiles = 0,
        }

        public ref float Timer => ref NPC.ai[0];

        private float AIState
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }


        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 200;
            NPC.height = 200;
            NPC.damage = 22;
            NPC.defense = 10;
            NPC.lifeMax = 3200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(gold: 15);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f; // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Default buff immunities should be set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
            // To dynamically adjust immunities of an active NPC, NPC.buffImmune[] can be changed in AI: NPC.buffImmune[BuffID.OnFire] = true;
            // This approach, however, will not preserve buff immunities. To preserve buff immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, as shown in the ApplySecondStageBuffImmunities method below.

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/CrabBossMusic");
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            AIState = (int)AttackTypes.BouncyProjectiles;
        }

        public override void AI()
        {
            Timer++;
            NPC.TargetClosest(true);

            if (NPC.life > 3200 * 0.8f) //more than 80% of health
            {
                NPC.aiStyle = -1;
                NPC.noGravity = false;
            }
            else
            {
                if (NPC.aiStyle == -1) // phase change
                {
                    SoundEngine.PlaySound(new SoundStyle("WiitaMod/Assets/SFX/NerdDogSound"), NPC.Center);
                    Timer = 0;
                    NPC.aiStyle = -2;
                    AIState = (int)AttackTypes.BouncyProjectiles;
                }
            }

            switch (AIState)
            {
                case (float)AttackTypes.BouncyProjectiles:

                    int attackInterval = NPC.aiStyle == -1 ? 60 : 30; // attack slower in first phase
                    float projSpeed = NPC.aiStyle == -1 ? 5 : 7.5f;

                    if (Timer >= attackInterval)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Player target = Main.player[NPC.target];

                            // Calculate direction and speed of the projectile
                            Vector2 direction = target.Center - NPC.Center;
                            direction.Normalize();
                            direction *= 5f; // Set the speed of the projectile
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction, ModContent.ProjectileType<CrabBouncyProj>(), NPC.damage / 3, 1f);
                        }
                        Timer = 0;
                    }
                    break;
            }

        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedTropicalCrabBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemID.CultistBossBag));
            npcLoot.Add(ItemDropRule.Common(ItemID.PoopBlock, 1, 5, 25));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
            return true;
        }
    }
}
