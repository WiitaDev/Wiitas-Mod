using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Systems;

namespace WiitaMod.Items.Weapons.Magic
{
    public class CoralStaffProj : ModProjectile
    {
        public override string Texture => $"WiitaMod/Assets/Textures/Empty";

        public ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;        //The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 480;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (Timer >= 50)
            {
                // Homing logic
                float speed = 10f;
                float turnSpeed = 75f;
                bool hasTarget = false;

                for (int i = 0; i < 200; i++)
                {
                    NPC target = Main.npc[i];
                    if (target.active && !target.friendly && target.CanBeChasedBy())
                    {
                        // Homing calculations
                        Vector2 targetPos = target.Center - Projectile.Center;
                        float length = targetPos.Length();
                        if (length < 350f)
                        {
                            targetPos.Normalize();
                            Projectile.velocity = (Projectile.velocity * 20f + targetPos * (turnSpeed - length * 0.15f)) / 21f;
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= speed;
                            hasTarget = true;
                        }
                    }
                }
                if(!hasTarget) 
                { 
                    Projectile.velocity *= 0.98f;
                    Projectile.timeLeft -= 2;
                }
            }
            else 
            {
                Projectile.velocity *= 0.98f;
            }
            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 position = Main.rand.NextVector2CircularEdge(10, 10);
                Dust.NewDust(Projectile.Center + position, 10, 10, DustID.BlueTorch);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(Effects.UltimaBlueTrail).Draw(Projectile);

            Texture2D core = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/CircleGradient", AssetRequestMode.ImmediateLoad).Value;
            Texture2D glow = ModContent.Request<Texture2D>("WiitaMod/Assets/Textures/Glow", AssetRequestMode.ImmediateLoad).Value;

            Color result = Color.RoyalBlue;
            result.A = 0;

            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(core, Projectile.Center - Main.screenPosition, core.Frame(), result, Projectile.rotation, core.Size() * 0.5f, Projectile.scale * 0.07f, 0, 0);
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), result, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 0.65f, 0, 0);
            }


            return false;
        }
    }

    public class CoralStaffHold : ModProjectile
    {
        public override string Texture => ModContent.GetModProjectile(ModContent.ProjectileType<CoralStaffProj>()).Texture;

        public ref float Time => ref Projectile.ai[0];
        public ref float UseType => ref Projectile.ai[1];
        public ref Player player => ref Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = true;
        }

        bool firstAttack = true;
        public override void AI()
        {
            bool manaIsAvailable = player.CheckMana(player.HeldItem.mana, false, false);
            if (!player.channel && !firstAttack || !manaIsAvailable && !firstAttack)
            {
                Projectile.Kill();
            }

            Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
            player.itemTime = 2; // Set item time to 2 frames while we are used
            player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
            player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.MountedCenter;

            if (UseType == 0 && Time == 40)
            {
                MainAttack();
            }
            else if (UseType == 2 && Time == 30) 
            {
                AltAttack();
            }

            float modRotB = compArmRotBack;
            float modRotF = compArmRotFront;
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, modRotB);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, modRotF);

            Projectile.spriteDirection = player.direction;
            Projectile.rotation -= player.direction * 0.2f;

            if(Time >= 40) 
            {
                Time = 0;
                firstAttack = false;
            }
            Time++;
        }

        private void MainAttack() 
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (!firstAttack)
                {
                    player.CheckMana(player.HeldItem.mana, true, false);
                } 

                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(10 * Projectile.spriteDirection, -70), new Vector2(Main.rand.NextFloat(-3, 4), Main.rand.NextFloat(3, 5) * -1), ModContent.ProjectileType<CoralStaffProj>(), Projectile.damage, 1f, player.whoAmI);
                }
                SoundEngine.PlaySound(SoundID.Item45, player.Center);

                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = new Vector2(10 * Projectile.spriteDirection - 10, -70) + Main.rand.NextVector2CircularEdge(15, 15);
                    int smashDust = Dust.NewDust(player.Center + position, 10, 10, DustID.BlueTorch, Scale: 1.5f);
                    Main.dust[smashDust].noGravity = true;
                    Main.dust[smashDust].velocity *= 0.1f;
                }
            }
        }

        private void AltAttack()
        {
            if (player.whoAmI == Main.myPlayer)
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = new Vector2(8 * Projectile.spriteDirection - 8, 20) + Main.rand.NextVector2Circular(10, 10);
                    int smashDust = Dust.NewDust(player.Center + position, 10, 10, DustID.BlueTorch, Scale: 2f);
                    Main.dust[smashDust].noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item45, player.Center);

                player.GetModPlayer<ModGlobalPlayer>().ActivateShockwave = true;
            }
        }

        private float compArmRotBack;
        private float compArmRotFront;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D staffTexture = ModContent.Request<Texture2D>("WiitaMod/Items/Weapons/Magic/CoralStaff", AssetRequestMode.ImmediateLoad).Value;
            Color whiteColor = new Color(255, 255, 255, 255);
            Vector2 posOffset = new Vector2(10 * Projectile.spriteDirection, 0);
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float animationTime;
            if(UseType == 2) 
            {
                animationTime = Time;

                if(Time >= 10 && Time <= 25) //-20 to 10
                {
                    animationTime = 30 - Time * 2f;
                }
                else if (Time >= 26 && Time <= 33) // impact frames.  THIS CODE IS SO ATROCIOUSLY BAD
                {
                    animationTime = -20;
                }
                else if (Time > 33) //-18 to 0
                {
                    animationTime = (Time - 40) * 3f;
                }           

                compArmRotFront = (-animationTime - 35)  * 0.07f * Projectile.spriteDirection; // hand stuff

                posOffset.Y = -animationTime - 45;
            }
            else if(UseType == 0) 
            {
                animationTime = Time;               
                if (!firstAttack)
                {
                    animationTime = 40;
                }

                posOffset.Y = -animationTime * 0.75f - 20;
                compArmRotFront = (-animationTime - 15) * 0.04f * Projectile.spriteDirection; // hand stuff
            }

            Main.EntitySpriteDraw(staffTexture, player.Center + posOffset - Main.screenPosition, staffTexture.Frame(), whiteColor, MathHelper.ToRadians(-45 * Projectile.spriteDirection), staffTexture.Size() * 0.5f, Projectile.scale, effects, 0);

            lightColor = Color.RoyalBlue;
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            player.channel = false;
        }

    }
}