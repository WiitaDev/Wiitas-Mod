using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;

namespace WiitaMod.Systems
{
    public class ModGlobalPlayer : ModPlayer
    {
        //Accessories
        public bool HealthFlowerEquipped;
        public bool PhilosophersNecklaceEquipped;

        //Pets
        public bool HamisPetEquipped;

        //Weapons
        public int flamesShot = 0;

        //Screenshake
        public int screenShakeTimerGlobal = -1000;
        public int screenShakeVelocity = 1000;

        //Shockwave
        public bool ActivateShockwave;
        private int rippleCount = 2;
        private int rippleSize = 15;
        private int rippleSpeed = 35;
        private float distortStrength = 80f;

        int shockwaveProgress = 400;

        public override void ResetEffects()
        {
            HealthFlowerEquipped = false;
            PhilosophersNecklaceEquipped = false;
            HamisPetEquipped = false;
            ActivateShockwave = false;
        }

        public override void PreUpdate()
        {
            if (ActivateShockwave)
            {
                ActivateShockwave = false;
                if (Main.netMode != NetmodeID.Server && !Filters.Scene["Shockwave"].IsActive())
                {
                    Filters.Scene.Activate("Shockwave", Player.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(Player.Center);
                }
                shockwaveProgress = 0;
            }
            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                float progress = (shockwaveProgress) / 140f;
                Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
            }
            if (shockwaveProgress >= 480)
            {
                Filters.Scene.Deactivate("Shockwave");

            }
            shockwaveProgress++;

            if(Player.channel == false) 
            {
                flamesShot = 0;
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.statLife <= Player.statLifeMax2 / 2 && HealthFlowerEquipped && Player.HasBuff(BuffID.PotionSickness) == false)
            {
                Player.QuickHeal();
            }

        }

        public override void ModifyScreenPosition()
        {
            screenShakeTimerGlobal--;
            if (screenShakeTimerGlobal < 0 && screenShakeTimerGlobal > -100)
            {
                Main.screenPosition += new Vector2(Main.rand.Next(-screenShakeVelocity / 100, screenShakeVelocity / 100), Main.rand.Next(-screenShakeVelocity / 100, screenShakeVelocity / 100));
                if (screenShakeVelocity >= 100)
                {
                    screenShakeVelocity -= 10;
                }
            }
            else
            {
                screenShakeVelocity = 1000;
            }
            base.ModifyScreenPosition();
        }


        public override void UpdateEquips()
        {
            if (PhilosophersNecklaceEquipped)
            {
                Player.potionDelayTime = (int)(Player.potionDelayTime * 0.75f);
                Player.restorationDelayTime = (int)(Player.restorationDelayTime * 0.75f);
                Player.mushroomDelayTime = (int)(Player.mushroomDelayTime * 0.75f);
                Player.longInvince = true; //longInvince so that other accessories dont stack (this doesn't even work lol)
            }
            base.UpdateEquips();
        }

        public override void PostUpdateBuffs()
        {


        }

        // Vanilla applies immunity time before this method and after PreHurt and Hurt
        // Therefore, we should apply our immunity time increment here
        public override void PostHurt(Player.HurtInfo info)
        {
            // Different cooldownCounter values mean different damage types taken and different cooldown slots
            // We should apply our immunity time to the correct cooldown slot
            // Slot -1: Most damages from all other sources not mentioned below
            // Slot 0: Contacting with tiles that deals damage, such as spikes and cactus in don't starve world
            // Slot 1: Special enemies (Vanilla for Moon Lord, Empress of Light and their minions and projectiles)
            // Slot 2: Unused in vanilla
            // Slot 3: Trying to catch lava critters with regular bug net
            // Slot 4: Damages from lava

            // Here we increase the player's immunity time by 1 second when Example Immunity Accessory is equipped
            if (!PhilosophersNecklaceEquipped)
                return;

            if (!info.PvP)
            {
                Player.AddImmuneTime(info.CooldownCounter, 30);
            }

        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (damage >= Player.statLife && HealthFlowerEquipped && !Player.HasBuff(BuffID.PotionSickness))
            {
                Player.QuickHeal();
                SoundEngine.PlaySound(SoundID.Item4.WithVolumeScale(1f).WithPitchOffset(0.1f), Player.Center);
                if (Player.HasBuff(BuffID.PotionSickness) == false)
                {
                    return true;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item4.WithVolumeScale(1f).WithPitchOffset(0.1f), Player.Center);
                    return false;
                }
            }

            return true;
        }

    }
}