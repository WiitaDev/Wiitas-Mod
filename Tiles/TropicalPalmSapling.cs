using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WiitaMod.Tiles
{
    public class TropicalPalmSapling : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileID.Sets.CommonSapling[Type] = true;
            TileID.Sets.TreeSapling[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<TropicalSand>() };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(113, 90, 71), Language.GetText("MapObject.Sapling"));
            DustType = DustID.PalmWood;
            AdjTiles = new int[] { TileID.Saplings };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            // A random chance to slow down growth
            if (!WorldGen.genRand.NextBool(20))
            {
                return;
            }

            Tile tile = Framing.GetTileSafely(i, j); // Safely get the tile at the given coordinates
            bool growSuccess; // A bool to see if the tree growing was successful.

            growSuccess = WorldGen.GrowPalmTree(i, j);

            // A flag to check if a player is near the sapling
            bool isPlayerNear = WorldGen.PlayerLOS(i, j);

            //If growing the tree was a success and the player is near, show growing effects
            if (growSuccess && isPlayerNear)
            {
                WorldGen.TreeGrowFXCheck(i, j);
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 1)
                effects = SpriteEffects.FlipHorizontally;
        }
    }

    public class TropicalPalmTree : ModPalmTree
    {
        public override void SetStaticDefaults()
        {
            GrowsOnTileId = new int[] {ModContent.TileType<TropicalSand>() };
        }

        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 0.153f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.8802f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTreeTops");
        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTree");

        public override Asset<Texture2D> GetOasisTopTextures() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/AcidWoodTreeOasisTops");

        public override int DropWood() => ItemID.PalmWood;
    }
}