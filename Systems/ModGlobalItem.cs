using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Buffs;
using WiitaMod.Items.Weapons.Ranger.BassBows;

namespace WiitaMod.Systems
{
    public class ModGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            // Apply to weapons
            return lateInstantiation && entity.damage > 0;
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