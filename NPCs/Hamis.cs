using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WiitaMod.Items.Armor;
using WiitaMod.Items.Pets;
using WiitaMod.Items.Placeable;

namespace WiitaMod.NPCs
{
    public class Hamis : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hamis");

            Main.npcFrameCount[NPC.type] = 17;
        }
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            Idle,
            Notice,
            Jump,
            Fall,
            Run
        }

        // Our texture is 14x20 with 2 pixels of padding vertically, so 22 is the vertical spacing.
        // These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
        private enum Frame
        {
            Idle1,
            Idle2,
            Idle3,
            Idle4,
            Idle5,
            Idle6,
            Run1,
            Run2,
            Run3,
            Run4,
            Run5,
            Run6,
            Jump1,
            Jump2,
            Jump3,
            Fall1,
            Fall2
        }

        // These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        public ref float AI_State => ref NPC.ai[0];
        public ref float AI_Timer => ref NPC.ai[1];
        bool playerNoticed = false;


        public override void SetDefaults()
        {
            NPC.width = 14;
            NPC.height = 20;
            NPC.damage = 10;
            NPC.defense = 0;
            NPC.lifeMax = 25;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 10f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1; // 3 = Fighter AI(zombie, etc.), -1 = custom AI
            NPC.scale = 1.5f;
            NPC.netAlways = true;
            NPC.npcSlots = 0.5f;

            Banner = NPC.type; // Makes this NPC get affected by the normal zombie banner.
            BannerItem = ModContent.ItemType<HamisBannerItem>(); // Makes kills of this NPC go towards dropping the banner it's associated with.
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.Common(ItemID.GoldOre, 1)); // 100% chance to drop Gold Ore
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<HamisPetItem>(), 100, 50)); // 1% chance to drop in normal mode and 2% in expert/master
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<HamisHat>(), 200, 100)); // 0.5% chance to drop in normal mode and 1% in expert/master
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            SoundEngine.PlaySound(new SoundStyle("WiitaMod/Assets/SFX/HamisBite").WithPitchOffset(Main.rand.NextFloat(0.25f, 0.50f)), NPC.Center);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.75f; // Spawn with 3/4 the chance of a regular zombie.
        }
        public override int SpawnNPC(int tileX, int tileY)
        {
            return base.SpawnNPC(tileX, tileY);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Hamis."),
            });
        }

        public override void OnKill()
        {
            for (int i = 0; i < Main.rand.Next(3, 5); i++)
            {
                Vector2 GoreSpeed = new Vector2(Main.rand.NextFloat(0f, 2f), Main.rand.NextFloat(0f, 2f));
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, GoreSpeed, Main.rand.Next(135, 137), 0.75f);
            }
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 0.75f);
            }
        }

        public override void AI()
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction * -1;

            ;
            if (NPC.velocity.Y != 0)
            {
                AI_State = (float)ActionState.Fall;
            }

            int tileX = (int)((NPC.position.X + NPC.width / 2 + 15 * NPC.direction) / 16f);
            int tileY = (int)((NPC.position.Y + NPC.height - 1f) / 16f);

            SlopeCheck(tileX, tileY);

            switch (AI_State)
            {
                case (float)ActionState.Idle:
                    GoIdle();
                    break;
                case (float)ActionState.Notice:
                    Notice();
                    break;
                case (float)ActionState.Jump:
                    Jump();
                    break;
                case (float)ActionState.Fall:
                    if (NPC.velocity.Y == 0)
                    {
                        if (Main.player[NPC.target].Distance(NPC.Center) < 64f)
                        {
                            AI_Timer = Main.rand.Next(30, 90) * -1;
                        }
                        else 
                        {
                            AI_Timer = 0;
                        }

                        AI_State = (float)ActionState.Idle;

                        NPC.velocity.X = 0;
                    }

                    break;
                case (float)ActionState.Run:
                    Run();
                    break;
            }

        }

        public override void FindFrame(int frameHeight)
        {

            // For the most part, our animation matches up with our states.
            switch (AI_State)
            {
                case (float)ActionState.Idle:
                    // npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
                    NPC.frameCounter++;

                    if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Idle1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Idle2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Idle3 * frameHeight;
                    }
                    else if (NPC.frameCounter < 40)
                    {
                        NPC.frame.Y = (int)Frame.Idle4 * frameHeight;
                    }
                    else if (NPC.frameCounter < 50)
                    {
                        NPC.frame.Y = (int)Frame.Idle5 * frameHeight;
                    }
                    else if (NPC.frameCounter < 60)
                    {
                        NPC.frame.Y = (int)Frame.Idle6 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;
                case (float)ActionState.Notice:
                    // Going from Notice to Asleep makes our npc look like it's crouching to jump.
                    if (AI_Timer < 10)
                    {
                        NPC.frame.Y = (int)Frame.Idle5 * frameHeight;
                    }
                    else if (AI_Timer > 10)
                    {
                        NPC.frame.Y = (int)Frame.Idle3 * frameHeight;
                    }

                    break;

                case (float)ActionState.Jump:
                    NPC.frameCounter++;

                    if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Jump1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Jump2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Jump3 * frameHeight;
                    }
                    else if (NPC.frameCounter < 40)
                    {
                        AI_State = (float)ActionState.Fall;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                    break;

                case (float)ActionState.Fall:
                    NPC.frame.Y = (int)Frame.Fall1 * frameHeight;
                    break;

                case (float)ActionState.Run:
                    AI_Timer++;
                    NPC.frameCounter++;

                    if (NPC.frameCounter < 5)
                    {
                        NPC.frame.Y = (int)Frame.Run1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Run2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 15)
                    {
                        NPC.frame.Y = (int)Frame.Run3 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Run4 * frameHeight;
                    }
                    else if (NPC.frameCounter < 25)
                    {
                        NPC.frame.Y = (int)Frame.Run5 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Run6 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }

                    break;

            }
        }

        private void Notice()
        {
            if(AI_Timer < 0) 
            {
                AI_State = (float)ActionState.Run;
                return;
            }
            // If the targeted player is in attack range (250).
            if (Main.player[NPC.target].Distance(NPC.Center) < 300f)
            {
                // Here we use our Timer to wait .33 seconds before actually jumping. In FindFrame you'll notice AI_Timer also being used to animate the pre-jump crouch
                AI_Timer++;

                if (AI_Timer >= Main.rand.Next(20,41) && NPC.velocity.Y == 0)
                {
                    AI_State = (float)ActionState.Jump;
                    AI_Timer = 0;
                }
            }
            else
            {
                NPC.TargetClosest(true);

                if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 300f)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    AI_State = (float)ActionState.Idle;
                    AI_Timer = 0;
                }
            }
        }

        private void GoIdle()
        {
            // TargetClosest sets npc.target to the player.whoAmI of the closest player.
            // The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
            // This is also automatically flipped if npc.confused.
            NPC.TargetClosest(true);
            if(Main.player[NPC.target].Distance(NPC.Center) > 800f) { playerNoticed = false; }

            if (!Collision.CanHitLine(NPC.position, 20, 20, Main.player[NPC.target].position, 20, 20) && playerNoticed == false) { playerNoticed = false; return; } else { playerNoticed = true; }


            // Now we check the make sure the target is still valid and within our specified notice range (500)
            if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 300f && AI_Timer >= 0)
            {
                // Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
                AI_State = (float)ActionState.Notice;
                AI_Timer = 0;
            }
            if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) <= 600f)
            {
                AI_State = (float)ActionState.Run;
            }
        }
        private void Jump()
        {
            AI_Timer++;
            float playerDistance = Main.player[NPC.target].Distance(NPC.Center);

            if (AI_Timer == 1)
            {
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                float rotation = (float)Math.Atan2(vector8.Y - (Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f) - playerDistance), vector8.X - (Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)));
                NPC.velocity = new Vector2((float)(Math.Cos(rotation) * (playerDistance * 0.03f + 4f) * -1), (float)(Math.Sin(rotation) * (playerDistance * 0.03f + 4f) * -1));

                SoundEngine.PlaySound(new SoundStyle("WiitaMod/Assets/SFX/HamisJump").WithVolumeScale(0.5f).WithPitchOffset(Main.rand.NextFloat(0.80f, 1f)), NPC.Center);
            }
        }

        private void Run()
        {
            Player target = Main.player[NPC.target];
            NPC.TargetClosest(true);

            if (target.position.X < NPC.position.X && NPC.velocity.X > -4 && NPC.HasValidTarget) // AND I'm not at max "left" velocity
            {
                NPC.velocity.X -= Main.rand.NextFloat(0.36f, 0.56f); // accelerate to the left
            }
            else if (Main.player[NPC.target].Distance(NPC.Center) < 300f && AI_Timer >= 0 && Main.rand.Next(0, 60) == 0)
            {
                NPC.velocity = Vector2.Zero;
                AI_State = (float)ActionState.Notice;
                AI_Timer = 0;
            }

            if (target.position.X > NPC.position.X && NPC.velocity.X < 4 && NPC.HasValidTarget) // AND I'm not at max "right" velocity
            {
                NPC.velocity.X += Main.rand.NextFloat(0.36f, 0.56f); // accelerate to the right
            }
            else if (Main.player[NPC.target].Distance(NPC.Center) < 300f && AI_Timer >= 0 && Main.rand.Next(0, 60) == 0)
            {
                NPC.velocity = Vector2.Zero;
                AI_State = (float)ActionState.Notice;
                AI_Timer = 0;
            }
            if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 600f)
            {
                // Out targeted player seems to have left our range, so we'll go back to sleep.
                NPC.velocity = Vector2.Zero;
                AI_State = (float)ActionState.Idle;
            }
        }
        private void SlopeCheck(int tileX, int tileY)
        {
            if (NPC.velocity.Y >= 0f)
            {
                int num81 = 0;
                if (NPC.velocity.X < 0f)
                    num81 = -1;
                if (NPC.velocity.X > 0f)
                    num81 = 1;
                Vector2 position3 = NPC.position;
                position3.X += NPC.velocity.X;
                if (tileX * 16 < position3.X + NPC.width && tileX * 16 + 16 > position3.X && (Main.tile[tileX, tileY].HasUnactuatedTile && !Main.tile[tileX, tileY].TopSlope && !Main.tile[tileX, tileY - 1].TopSlope && Main.tileSolid[Main.tile[tileX, tileY].TileType] && !Main.tileSolidTop[Main.tile[tileX, tileY].TileType] || Main.tile[tileX, tileY - 1].IsHalfBlock && Main.tile[tileX, tileY - 1].HasUnactuatedTile) && (!Main.tile[tileX, tileY - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 1].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 1].TileType] || Main.tile[tileX, tileY - 1].IsHalfBlock && (!Main.tile[tileX, tileY - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 4].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 4].TileType])) && (!Main.tile[tileX, tileY - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 2].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 2].TileType]) && (!Main.tile[tileX, tileY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 3].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 3].TileType]) && (!Main.tile[tileX - num81, tileY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX - num81, tileY - 3].TileType]))
                {
                    float num84 = tileY * 16;
                    if (Main.tile[tileX, tileY].IsHalfBlock)
                        num84 += 8f;
                    if (Main.tile[tileX, tileY - 1].IsHalfBlock)
                        num84 -= 8f;
                    if (num84 < position3.Y + NPC.height)
                    {
                        float num85 = position3.Y + NPC.height - num84;
                        float num86 = 16.1f;
                        if (num85 <= num86)
                        {
                            NPC.gfxOffY += NPC.position.Y + NPC.height - num84;
                            NPC.position.Y = num84 - NPC.height;
                            if (num85 < 9f)
                            {
                                NPC.stepSpeed = 1f;
                            }
                            else
                            {
                                NPC.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
        }

    }
}
