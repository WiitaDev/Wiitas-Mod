using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Localization;
using WiitaMod.Items.Placeable;
using WiitaMod.Buffs;

namespace WiitaMod.Tiles
{
    public class BassBucket : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Tile properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true; // Enable interaction outline

            // Define tile dimensions and style
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table | AnchorType.Platform, 2, 0);
            TileObjectData.addTile(Type);

            // Map entry settings
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), name);

            // Other settings
            DustType = DustID.Lead;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true; // Enable smart interaction (right-click)
        }

        public override void MouseOver(int i, int j)
        {
            // Show a little pylon icon on the mouse indicating we are hovering over it.
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<BassBucketItem>();
        }

        public override bool RightClick(int i, int j)
        {
            // Get the player interacting with the tile
            Player player = Main.LocalPlayer;

            Vector2 tileCenter = new Vector2(i * 16, j * 16);

            // Apply buff for 1 minute (60 seconds)
            player.AddBuff(ModContent.BuffType<BassBucketBuff>(), 60 * 60);

            // Play sound and spawn particles

            if (Main.rand.Next(1, 501) == 1)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath61, tileCenter);
            }
            else SoundEngine.PlaySound(SoundID.NPCDeath28, tileCenter);

            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(tileCenter, 10, 10, DustID.Gold, SpeedX: Main.rand.Next(-1, 1), SpeedY: -1f);
            }

            return true;
        }

    }
}