using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.Tiles
{
    public class DeepcoreFlareTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            Main.tileNoAttach[Type] = true;
            Main.tileMergeDirt[Type] = false;

            AddMapEntry(new Color(205, 220, 255), CreateMapEntryName());

            DustType = DustID.Stone;
            HitSound = SoundID.Dig;
        }

        // Emit light when "on"
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY < 16) // "On" state (second frame)
            {
                Color c = Color.CadetBlue * 0.03f;
                r = c.R;
                g = c.G;
                b = c.B;
            }
        }

        // Toggle state when activated by wire
        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            tile.TileFrameY = (short)(tile.TileFrameY >= 16 ? 0 : 16); // Swap frames

            // Sync tile change in multiplayer
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, i, j, 1);
        }
    }
}