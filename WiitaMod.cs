using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Systems;
using WiitaMod.Systems.Primitives;

namespace WiitaMod
{
    public class WiitaMod : Mod
    {
        public override void Load()
        {
            if (!Main.dedServ)
            {
                /*Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("WiitaMod/Effects/ShockwaveEffect", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.High);
                Filters.Scene["Shockwave"].Load();*/

                ParticleManager.RegisterParticles();
                PrimitiveRenderer.Initialize();
            }
        }

        public override void Unload()
        {
            ParticleManager.Unload();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte messageType = reader.ReadByte();

            switch (messageType)
            {
                case 0:
                    {
                        // Request to add one shard to an NPC.

                        if (Main.netMode != NetmodeID.Server)
                            return;

                        NPC npc = Main.npc[reader.ReadByte()];

                        if (!npc.active)
                            return;

                        ModGlobalNPC crystal = npc.GetGlobalNPC<ModGlobalNPC>();

                        bool maxShards = crystal.shardStacks == 4;

                        crystal.shardStacks = Math.Min(4, crystal.shardStacks + 1);

                        crystal.UpdateShards(npc);

                        if(!maxShards)
                            crystal.SendShardUpdate(npc);

                        break;
                    }

                case 1:
                    {
                        // Synchronize shard count.

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            return;

                        NPC npc = Main.npc[reader.ReadByte()];
                        byte shardAmount = reader.ReadByte();


                        if (!npc.active)
                            return;

                        ModGlobalNPC crystal = npc.GetGlobalNPC<ModGlobalNPC>();

                        crystal.shardStacks = shardAmount;
                        crystal.UpdateShards(npc);

                        break;
                    }

                case 2:
                    {
                        // Request to consume the shards.

                        if (Main.netMode != NetmodeID.Server)
                            return;

                        int npcWhoAmI = reader.ReadByte();

                        NPC npc = Main.npc[npcWhoAmI];

                        if (!npc.active)
                            return;

                        ModGlobalNPC crystal = npc.GetGlobalNPC<ModGlobalNPC>();

                        if (crystal.shardStacks <= 0)
                            return;

                        SoundEngine.PlaySound(SoundID.Item122.WithPitchOffset(0.7f).WithVolumeScale(0.6f), npc.Center);
                        SoundEngine.PlaySound(SoundID.NPCDeath56.WithPitchOffset(0.75f).WithVolumeScale(0.7f), npc.Center);

                        float[] crystalBonus = [1f, 1.15f, 1.25f, 2f, 3f];
                        float mult = crystalBonus[crystal.shardStacks];

                        for (int i = 0; i < 5 * mult * 1.5f; i++)
                        {
                            Particle particle = new GlowOrbParticle(npc.Center, Main.rand.NextVector2Circular(1.5f * mult, 1.5f * mult), false, (int)(40 * mult), Main.rand.NextFloat(0.6f, 0.8f) + mult * 0.15f, new Vector2(0.75f, 1f), Color.Lerp(Color.DodgerBlue, Color.Aquamarine, Main.rand.NextFloat()), true);
                            ParticleManager.SpawnParticle(particle);
                        }

                        crystal.shardStacks = 0;
                        crystal.shardLifetime = 0;

                        crystal.SendShardUpdate(npc);

                        break;
                    }
            }
        }
    }
}