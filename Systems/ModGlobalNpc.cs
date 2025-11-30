using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Items.Weapons.Summon;
using WiitaMod.Items.CraftingMaterials;
using WiitaMod.Projectiles.Pets;
using WiitaMod.Items.Weapons.Ranger;
using WiitaMod.Items.Placeable;
using WiitaMod.Items.Tools;

namespace WiitaMod.Systems
{
    public class ModGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ModContent.ProjectileType<HamisNuke>())
            {
                if (npc.type == NPCID.Frog || npc.type == NPCID.GoldFrog || npc.type == NPCID.BoundTownSlimeYellow)
                {
                    modifiers.FinalDamage += npc.lifeMax - 1;
                }
                else
                {
                    modifiers.FinalDamage *= 0;
                }
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.netID == NPCID.GoblinSummoner)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowflameApparitionStaff>(), 4, 1));
            }
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.ArmsDealer)
            {
                shop.InsertAfter(ItemID.IllegalGunParts, ModContent.ItemType<IllegalRocketLauncherParts>(), Condition.TimeNight, Condition.DownedPlantera);
                /*shop.Add(new Item(ModContent.ItemType<IllegalRocketLauncherParts>())
                {
                    //add custom stuff here exmple "shopCustomPrice = 2"
                });*/
            }

            if (shop.NpcType == NPCID.Pirate)
            {
                shop.Add(new Item(ModContent.ItemType<FlameBlaster>())
                {
                    shopCustomPrice = Item.buyPrice(0,30)
                });
            }

            if (shop.NpcType == NPCID.Demolitionist)
            {
                shop.Add(new Item(ModContent.ItemType<DeepcoreFlareItem>())
                {
                    shopCustomPrice = Item.buyPrice(0,0, 5)
                });
            }
        }


    }
}