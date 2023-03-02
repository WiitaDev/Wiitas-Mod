using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Graphics.Effects;

namespace WiitaMod.Systems
{
    public class ModGlobalPlayer : ModPlayer
    {
        public bool HealthFlowerEquipped;
        public bool PhilosophersNecklaceEquipped;

        public bool HamisPetEquipped;

        //Screenshake
        public int screenShakeTimerGlobal = -1000;
        public int screenShakeVelocity = 1000;



        public override void ResetEffects()
        {
            HealthFlowerEquipped = false;
            PhilosophersNecklaceEquipped = false;
            HamisPetEquipped = false;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
        }
        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (Player.statLife <= Player.statLifeMax2 / 2 && HealthFlowerEquipped && Player.HasBuff(BuffID.PotionSickness) != true)
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

        public override void PreUpdate()
        {
        }

        public override void UpdateEquips()
        {
            if (PhilosophersNecklaceEquipped)
            {
                Player.potionDelayTime = (int)((float)Player.potionDelayTime * 0.75f);
                Player.restorationDelayTime = (int)((float)Player.restorationDelayTime * 0.75f);
                Player.mushroomDelayTime = (int)((float)Player.mushroomDelayTime * 0.75f);
            }
            base.UpdateEquips();
        }

        public override void PostUpdateBuffs()
        {


        }

        // Vanilla applies immunity time before this method and after PreHurt and Hurt
        // Therefore, we should apply our immunity time increment here
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
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

            if (!pvp)
            {
                Player.longInvince = true; //longInvince so that other accessories dont stack
                Player.AddImmuneTime(cooldownCounter, 30);
            }

        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (damage >= Player.statLife && HealthFlowerEquipped && Player.HasBuff(BuffID.PotionSickness) != true)
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