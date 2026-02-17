using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;
using WiitaMod.Items.Weapons.Ranger.BassBows;
using WiitaMod.NPCs;

namespace WiitaMod.Systems
{
    public class ModGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.Bass && !ModContent.GetInstance<WiitaModServerConfig>().OnlyHamis)
            {
                item.DefaultToCapturedCritter(ModContent.NPCType<BassCritter>());
            }
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if(item.type == ModContent.ItemType<BassBow>()
            || item.type == ModContent.ItemType<JungleBassBow>()
            || item.type == ModContent.ItemType<MoltenBassBow>()
            || item.type == ModContent.ItemType<CosmicBassBow>()
            || item.type == ModContent.ItemType<HolyBassBow>()) 
            {
                if (player.HasBuff(ModContent.BuffType<BassBucketBuff>()))
                {
                    damage *= 1.01f;
                }
            }
        }
    }
}