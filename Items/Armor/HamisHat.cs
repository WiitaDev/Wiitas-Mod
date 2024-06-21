using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WiitaMod.Items.Pets;
using WiitaMod.Items.Placeable;

namespace WiitaMod.Items.Armor
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
            ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
            // ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases maximum mana by 60";
            player.statManaMax2 += 60;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.AmethystRobe || body.type == ItemID.TopazRobe || body.type == ItemID.SapphireRobe || body.type == ItemID.EmeraldRobe
                || body.type == ItemID.RubyRobe || body.type == ItemID.GypsyRobe || body.type == ItemID.DiamondRobe || body.type == ItemID.AmberRobe;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 8f;
            player.GetDamage(DamageClass.Magic) += 0.04f;
        }

        public override void SetDefaults()
        {
            Item.width = 28; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(silver: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 4;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<HamisPetItem>());
            recipe.AddIngredient(ItemID.MagicHat);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}