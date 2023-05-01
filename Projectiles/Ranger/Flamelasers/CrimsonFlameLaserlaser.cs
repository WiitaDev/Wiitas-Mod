using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Projectiles.Ranger.Flamelasers
{
    // The following laser shows a channeled ability, after charging up the laser will be fired
    // Using custom drawing, dust effects, and custom collision checks for tiles
    public class CrimsonFlameLaserlaser : ModProjectile
    {
        // Use a different style for constant so it is very clear in code when a constant is used

        // The maximum charge value
        private const float MAX_CHARGE = 60f;
        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 70f;

        // The actual distance is stored in the ai0 field
        // By making a property to handle this it makes our life easier, and the accessibility more readable
        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float LaserSize = 0;
        private int CounterThing = 0;

        // The actual charge value is stored in the localAI0 field
        public float Charge
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property
        public bool IsAtMaxCharge => Charge == MAX_CHARGE;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // We start drawing the laser if we have charged up
            if (IsAtMaxCharge)
            {
                DrawLaser(Main.spriteBatch, (Texture2D)TextureAssets.Projectile[Projectile.type], Main.player[Projectile.owner].Center,
                    Projectile.velocity, 10, Projectile.damage, -1.57f, LaserSize, 1000f, Color.White, (int)MOVE_DISTANCE);
            }
            return false;
        }


        // The core function of drawing a laser
        public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            // Draws the laser 'body'
            for (float i = transDist; i <= Distance; i += step)
            {
                Color c = Color.White;
                var origin = start + i * unit;
                spriteBatch.Draw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 26, 28, 30), i < transDist ? Color.Transparent : c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
            }

            // Draws the laser 'tail'
            spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            // Draws the laser 'head'
            spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
        }

        // Change the way of collision check of the projectile
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // We can only collide if we are at max charge, which is when the laser is actually fired
            if (!IsAtMaxCharge) return false;

            Player player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
                player.Center + unit * Distance, 22, ref point);
        }

        // Set custom immunity time on hitting an NPC
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5;

            target.AddBuff(BuffID.Ichor, 180);

            Player player = Main.player[Projectile.owner];
            if (Main.myPlayer == player.whoAmI)
            {
                if (Main.rand.NextBool(4))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(2, 6) * -1), ProjectileID.GoldenShowerFriendly, Projectile.damage, 0, Main.myPlayer);
                }
            }
        }

        // The AI of the projectile
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center + Projectile.velocity * MOVE_DISTANCE;
            Projectile.timeLeft = 2;

            // By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
            // First we update player variables that are needed to channel the laser
            // Then we run our charging laser logic
            // If we are fully charged, we proceed to update the laser's position
            // Finally we spawn some effects like dusts and light

            UpdatePlayer(player);
            ChargeLaser(player);
            CustomStuff(player);

            // If laser is not charged yet, stop the AI here.
            if (Charge < MAX_CHARGE) return;

            SetLaserPosition(player);
            SpawnDusts(player);
            CastLights();
        }
        private void CustomStuff(Player player)
        {

            if (Charge < MAX_CHARGE) return;

            if (LaserSize < 0.75f)
            {
                LaserSize += 0.05f;
            }

            if (CounterThing == 0)
            {
                SoundEngine.PlaySound(SoundID.Item122.WithVolumeScale(0.4f), player.Center);

                for (int i = 0; i < Main.rand.Next(8, 16); i++)
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.position, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)) + Projectile.velocity * 3, ProjectileID.MolotovFire, Projectile.damage, 0, Main.myPlayer);
                        SoundEngine.PlaySound(SoundID.Item20, player.Center);
                    }
                }
                CounterThing++;
            }
        }

        private void SpawnDusts(Player player)
        {
            Vector2 unit = Projectile.velocity * -1;
            Vector2 dustPos = player.Center + Projectile.velocity * Distance;
            for (int i = 0; i < 5 + 1; i++)
            {
                Vector2 offset = Projectile.velocity.RotatedBy(1.57f) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                Dust dust = Main.dust[Dust.NewDust(dustPos + offset - Vector2.One * 4f, 8, 8, DustID.OrangeTorch, 0.0f, 0.0f, 100, new Color(), 2f)];
                dust.velocity *= 10f;
                dust.noGravity = true;
                unit = dustPos - Main.player[Projectile.owner].Center;
                unit.Normalize();
                if (Main.rand.NextBool(4))
                {
                    dust = Main.dust[Dust.NewDust(Main.player[Projectile.owner].Center + 55 * unit, 8, 8, DustID.OrangeTorch, 0.0f, 0.0f, 100, new Color(), 2f)];
                    dust.velocity += Projectile.velocity;
                    dust.velocity *= 5f;
                    dust.noGravity = true;
                }
            }

        }

        /*
		 * Sets the end of the laser position based on where it collides with something
		 */
        private void SetLaserPosition(Player player)
        {
            for (Distance = MOVE_DISTANCE; Distance <= 1750f; Distance += 5f)
            {
                var start = player.Center + Projectile.velocity * Distance;
                if (!Collision.CanHitLine(player.Center, 1, 1, start, 1, 1))
                {
                    Distance -= 5f;
                    break;
                }
            }
        }

        private void ChargeLaser(Player player)
        {
            // Kill the projectile if the player stops channeling
            if (!player.channel)
            {
                Projectile.Kill();
            }
            else
            {
                //Do we still have enough mana? If not, we kill the projectile because we cannot use it anymore
                /*if (Main.time % 10 < 1 && player.HasAmmo(player.inventory[player.selectedItem], true) && !player.noItems && !player.CCed)
                {
                     Projectile.Kill();
                }*/
                Vector2 offset = Projectile.velocity;
                offset *= MOVE_DISTANCE - 20;
                Vector2 pos = player.Center + offset - new Vector2(10, 10);
                if (Charge < MAX_CHARGE)
                {
                    Charge++;
                }
                int chargeFact = (int)(Charge / 20f);
                Vector2 dustVelocity = Vector2.UnitX * 18f;
                dustVelocity = dustVelocity.RotatedBy(Projectile.rotation - 1.57f);
                Vector2 spawnPos = Projectile.Center + dustVelocity;
                for (int k = 0; k < chargeFact + 1; k++)
                {
                    Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - chargeFact * 2);
                    Dust dust = Main.dust[Dust.NewDust(pos, 20, 20, DustID.OrangeTorch, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f)];
                    dust.velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
                    dust.scale = Main.rand.Next(10, 20) * 0.06f;
                    dust.noGravity = true;
                }
            }
        }

        private void UpdatePlayer(Player player)
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
                if (aim.HasNaNs())
                {
                    aim = -Vector2.UnitY;
                }

                // Change a portion of the Prism's current velocity so that it points to the mouse. This gives smooth movement over time.
                aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.12f)); // last variable is the turn speed
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

        private void CastLights()
        {
            // Cast a light along the line of the laser
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

        /*
		 * Update CutTiles so the laser will cut tiles (like grass)
		 */
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Distance, (Projectile.width + 10) * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}