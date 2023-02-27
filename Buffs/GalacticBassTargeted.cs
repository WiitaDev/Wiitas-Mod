using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Common;
using WiitaMod.Projectiles;

namespace WiitaMod.Buffs
{
    public class GalacticBassTargeted : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Targeting");
            Description.SetDefault("Pain shall soon happen or not?");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ModGlobalNPC>().GalacticTargeting = true;

            if (npc.buffTime[buffIndex] <= 0) 
            {
                buffIndex--;
            }
            npc.netUpdate = true;

        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            
            return base.ReApply(npc, time, buffIndex);
        }
    }

}
    
