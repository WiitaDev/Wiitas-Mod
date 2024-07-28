using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Tiles;
using WiitaMod.Assets.Textures.Backgrounds;
using Terraria.WorldBuilding;

namespace WiitaMod.World
{
    // Shows setting up two basic biomes. For a more complicated example, please request.
    public class TropicalCavernsBiome : ModBiome
    {
        // Select all the scenery
        //public override ModWaterStyle WaterStyle => ModContent.GetInstance<ExampleWaterStyle>(); // Sets a water style for when inside this biome
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<TropicalCavernsBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        // Select Music
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/TropicalCavernsMusic");

        public override int BiomeTorchItemType => ItemID.CoralTorch;
        public override int BiomeCampfireItemType => ItemID.CoralCampfire;

        // Populate the Bestiary Filter
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => "WiitaMod/Assets/Textures/Backgrounds/TropicalCavernsMapBG";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<TropicalOceanTileCount>().tropicalSandstoneCount >= 240;

            // Limit this biome to the x area of the biome (english is not englishing)
            bool b2;
            if (Main.dungeonX > Main.maxTilesX / 2) //true if dungeon is on the right side
            {
                b2 = player.position.ToTileCoordinates().X < TropicalOceanGeneration.GetActualX(TropicalOceanGeneration.BiomeWidth);
            }
            else
            {
                b2 = player.position.ToTileCoordinates().X > TropicalOceanGeneration.GetActualX(TropicalOceanGeneration.BiomeWidth);
            }


            bool b3 = player.position.ToTileCoordinates().Y < (TropicalOceanGeneration.CaveStart + 500) && player.position.ToTileCoordinates().Y > (TropicalOceanGeneration.CaveStart + 20);

            return b1 && b2 && b3;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    }
}