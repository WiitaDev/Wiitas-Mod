using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;
using WiitaMod.Tiles;
using WiitaMod.World.TropicalOcean.Background;

namespace WiitaMod.World.TropicalOcean
{
    // Shows setting up two basic biomes. For a more complicated example, please request.
    public class TropicalOceanBiome : ModBiome
    {
        // Select all the scenery
        //public override ModWaterStyle WaterStyle => ModContent.GetInstance<ExampleWaterStyle>(); // Sets a water style for when inside this biome
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<TropicalOceanBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<TropicalCavernsBackgroundStyle>();

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        // Select Music
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/heisenburger");

        public override int BiomeTorchItemType => ItemID.CoralTorch;
        public override int BiomeCampfireItemType => ItemID.CoralCampfire;

        // Populate the Bestiary Filter
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => "WiitaMod/Assets/Textures/Backgrounds/TropicalOceanMapBG";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<TropicalOceanTileCount>().tropicalSandCount >= 120;

            // Limit this biome to the x area of the biome (english is not englishing)
            bool b2;
            if (Main.dungeonX > Main.maxTilesX / 2)
            {
                b2 = player.position.ToTileCoordinates().X < TropicalOceanGeneration.GetActualX(TropicalOceanGeneration.BiomeWidth);
            }
            else
            {
                b2 = player.position.ToTileCoordinates().X > TropicalOceanGeneration.GetActualX(TropicalOceanGeneration.BiomeWidth);
            }

            bool b3 = player.ZoneSkyHeight || player.ZoneOverworldHeight || (player.position.ToTileCoordinates().Y < TropicalOceanGeneration.CaveStart);

            return b1 || (b2 && b3);
        }

        // Declare biome priority. The default is BiomeLow so this is only necessary if it needs a higher priority.
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    }

    public class TropicalOceanTileCount : ModSystem
    {
        public int tropicalSandCount;
        public int tropicalSandstoneCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            tropicalSandCount = tileCounts[ModContent.TileType<TropicalSand>()];
            tropicalSandstoneCount = tileCounts[ModContent.TileType<CompressedSandstone>()];
        }
    }
}