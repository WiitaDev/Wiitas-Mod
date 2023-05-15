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

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            // This example does not use the AppliesToEntity hook, as such, we can handle multiple npcs here by using if statements.
            if (type == NPCID.ArmsDealer && NPC.downedPlantBoss && Main.dayTime == false)
            {
                // Adding an item to a vanilla NPC is easy:
                // This item sells for the normal price.
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<IllegalRocketLauncherParts>());
                nextSlot++; // Don't forget this line, it is essential.
            }

        }

    }
}