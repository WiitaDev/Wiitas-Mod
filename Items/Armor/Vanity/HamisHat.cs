using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Items.Armor.Vanity
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class HamisHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("HamHat");
            // Tooltip.SetDefault("Drippiest drip there is");

            // If your head equipment should draw hair while drawn, use one of the following:
            // ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            // ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
        }

        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 20; // Height of the item
            Item.value = Item.sellPrice(silver: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Yellow; // The rarity of the item
            Item.vanity = true;
        }
    }
}