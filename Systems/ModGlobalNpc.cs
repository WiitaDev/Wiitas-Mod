using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Items.Placeable;
using WiitaMod.Items.Weapons.Ranger;
using WiitaMod.Items.Weapons.Summon;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Projectiles.Pets;

namespace WiitaMod.Systems
{
    public class ModGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int shardStacks;
        public int shardLifetime;
        public Vector2[] ShardOffsets = new Vector2[4];
        public float[] ShardRotations = new float[4];

        public void SendShardUpdate(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            ModPacket packet = ModContent.GetInstance<WiitaMod>().GetPacket();

            packet.Write((byte)1);
            packet.Write((byte)npc.whoAmI);
            packet.Write((byte)shardStacks);

            packet.Send();
        }

        public void UpdateShards(NPC npc)
        {
            if(Main.netMode != NetmodeID.MultiplayerClient)
                shardLifetime = 480;

            if (shardStacks >= 4)
                return;

            Main.rand.SetSeed(npc.whoAmI * 1000);

            for (int i = 0; i < shardStacks; i++)
            {
                ShardOffsets[i] = Main.rand.NextVector2FromRectangle(new Rectangle(-npc.width / 2, -npc.height / 2, npc.width, npc.height));
                ShardRotations[i] = Main.rand.NextFloat(MathHelper.TwoPi);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ModContent.ProjectileType<HamisNuke>())
            {
                if (npc.type == NPCID.Frog || npc.type == NPCID.GoldFrog || npc.type == NPCID.BoundTownSlimeYellow)
                {
                    modifiers.FinalDamage += npc.lifeMax - 1;
                }
                else
                {
                    modifiers.FinalDamage *= 0;
                }
            }

            if (projectile.DamageType != DamageClass.Summon || ProjectileID.Sets.IsAWhip[projectile.type])
                return;

            var crystal = npc.GetGlobalNPC<ModGlobalNPC>();

            if (crystal.shardStacks <= 0)
                return;

            int[] crystalBonus = [0, 25, 50, 100, 200]; // percentages

            int stacks = crystal.shardStacks;

            if (stacks > 0)
            {
                int bonus = projectile.originalDamage * crystalBonus[stacks] / 100;
                modifiers.FlatBonusDamage += bonus;
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.DamageType != DamageClass.Summon || ProjectileID.Sets.IsAWhip[projectile.type])
                return;

            var crystal = npc.GetGlobalNPC<ModGlobalNPC>();


            if (crystal.shardStacks > 0)
            {
                SoundEngine.PlaySound(SoundID.Item122.WithPitchOffset(0.7f).WithVolumeScale(0.6f), npc.Center);
                SoundEngine.PlaySound(SoundID.NPCDeath56.WithPitchOffset(0.75f).WithVolumeScale(0.7f), npc.Center);

                float[] crystalBonus = [1f, 1.15f, 1.25f, 2f, 3f];
                float mult = crystalBonus[crystal.shardStacks];

                for (int i = 0; i < 5 * mult * 1.5f; i++)
                {
                    Particle particle = new GlowOrbParticle(npc.Center, Main.rand.NextVector2Circular(1.5f * mult, 1.5f * mult), false, (int)(40 * mult), Main.rand.NextFloat(0.6f, 0.8f) + mult * 0.15f, new Vector2(0.75f, 1f), Color.Lerp(Color.DodgerBlue, Color.Aquamarine, Main.rand.NextFloat()), true);
                    ParticleManager.SpawnParticle(particle);
                }

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = ModContent.GetInstance<WiitaMod>().GetPacket();

                    packet.Write((byte)2);
                    packet.Write((byte)npc.whoAmI);

                    packet.Send();
                }
                else
                {
                    crystal.shardStacks = 0;
                }
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var crystal = npc.GetGlobalNPC<ModGlobalNPC>();
            if (crystal.shardStacks <= 0)
                return;

            Texture2D shardTexture = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/ShatterShards").Value;

            int frameHeight = shardTexture.Height / 3;

            for (int i = 0; i < crystal.shardStacks; i++)
            {
                Main.EntitySpriteDraw(shardTexture, npc.Center - Main.screenPosition + crystal.ShardOffsets[i], new Rectangle(0, i % 3 * frameHeight, shardTexture.Width, frameHeight), Color.White, crystal.ShardRotations[i], new Vector2(shardTexture.Width / 2f, frameHeight / 2f), new Vector2(1f, 1f), SpriteEffects.None);

                if (Main.rand.NextBool(10))
                    Dust.NewDust(npc.Center + crystal.ShardOffsets[i], 5, 5, DustID.BlueTorch, Main.rand.NextFloat(0.2f, 0.5f), Main.rand.NextFloat(0.2f, 0.5f), 20, Color.White, 1f);
            }
        }

        public override void PostAI(NPC npc)
        {
            var crystal = npc.GetGlobalNPC<ModGlobalNPC>();
            if (crystal.shardStacks <= 0 || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            crystal.shardLifetime--;

            if (crystal.shardLifetime <= 0)
            {
                crystal.shardStacks = 0;

                if(Main.dedServ)
                    crystal.SendShardUpdate(npc);
            }

        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.netID == NPCID.GoblinSummoner)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowflameApparitionStaff>(), 4, 1));
            }
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.ArmsDealer)
            {
                shop.InsertAfter(ItemID.IllegalGunParts, ModContent.ItemType<IllegalRocketLauncherParts>(), Condition.TimeNight, Condition.DownedPlantera);
                /*shop.Add(new Item(ModContent.ItemType<IllegalRocketLauncherParts>())
                {
                    //add custom stuff here exmple "shopCustomPrice = 2"
                });*/
            }

            if (shop.NpcType == NPCID.Pirate)
            {
                shop.Add(new Item(ModContent.ItemType<FlameBlaster>())
                {
                    shopCustomPrice = Item.buyPrice(0, 30)
                });
            }

            if (shop.NpcType == NPCID.Demolitionist)
            {
                shop.Add(new Item(ModContent.ItemType<DeepcoreFlareItem>())
                {
                    shopCustomPrice = Item.buyPrice(0, 0, 5)
                });
            }
        }


    }
}