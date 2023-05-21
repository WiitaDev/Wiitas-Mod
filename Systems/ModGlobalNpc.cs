using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Weapons.Summon;
using WiitaMod.Items.CraftingMaterials;

namespace WiitaMod.Systems
{
    public class ModGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.netID == NPCID.GoblinSummoner)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowflameApparitionStaff>(), 4, 1));
            }
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {


        }
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.ArmsDealer)
            {
                shop.Add(new Item(ModContent.ItemType<IllegalRocketLauncherParts>())
                {
                    //add custom stuff here exmple "shopCustomPrice = 2"
                });
            }

        }


    }
}