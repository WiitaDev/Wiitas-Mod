using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Helpers;
using WiitaMod.Particles;
using WiitaMod.Particles.ParticleSystems;
using WiitaMod.Systems.Primitives;

namespace WiitaMod.Projectiles.Ranger.BassArrows.HolyBassBow
{
    public class HolyBassHold : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public ref float Time => ref Projectile.ai[0];

        public const float maxTimeLeft = 40f;

        public float ChargeTime = 20f;


        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)maxTimeLeft;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.Item77, Projectile.Center);
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            Time++;

            if (Time == ChargeTime)
            {
                SoundEngine.PlaySound(SoundID.Item68, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 5f, ModContent.ProjectileType<HolyBassLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            

            UpdatePlayer(player);
            Projectile.Center = player.MountedCenter + Projectile.velocity * 25f;
        }

        private void UpdatePlayer(Player player)
        {
            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (Projectile.owner == Main.myPlayer && Time <= maxTimeLeft - 20)
            {
                Vector2 aim = Vector2.Normalize(Main.MouseWorld - player.Center);
                if (aim.HasNaNs())
                {
                    aim = -Vector2.UnitY;
                }

                // Change a portion of the Prism's current velocity so that it points to the mouse. This gives smooth movement over time.
                aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.99f)); // last variable is the turn speed
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
            Projectile.rotation = player.itemRotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < ChargeTime)
            {
                Texture2D bigRing = ModContent.Request<Texture2D>("WiitaMod/Items/Weapons/Ranger/BassBows/HolyBassBow_Halo1", AssetRequestMode.ImmediateLoad).Value;
                Texture2D smallRing = ModContent.Request<Texture2D>("WiitaMod/Items/Weapons/Ranger/BassBows/HolyBassBow_Halo2", AssetRequestMode.ImmediateLoad).Value;

                Main.EntitySpriteDraw(bigRing, Projectile.Center + Projectile.velocity * 15f - Main.screenPosition, null, Color.White * (Time - 4 / ChargeTime), Projectile.rotation, bigRing.Size() / 2, MathF.Min(Time * Time / ChargeTime, 1), SpriteEffects.None);
                Main.EntitySpriteDraw(smallRing, Projectile.Center + Projectile.velocity * 27.5f - Main.screenPosition, null, Color.White * (Time / ChargeTime), Projectile.rotation, smallRing.Size() / 2, MathF.Min(Time * Time / ChargeTime, 1), SpriteEffects.None);
            }
            if (Time == ChargeTime - 1) // -1 to ensure the halos are in line with the laser
            {
                BassBowHaloParticle smallHalo = new BassBowHaloParticle(Projectile.Center + Projectile.velocity * 27.5f, Projectile.rotation, false);
                BassBowHaloParticle bigHalo = new BassBowHaloParticle(Projectile.Center + Projectile.velocity * 15f, Projectile.rotation, true);
                ParticleManager.SpawnParticle(smallHalo);
                ParticleManager.SpawnParticle(bigHalo);
            }

            return false;
        }
    }
}