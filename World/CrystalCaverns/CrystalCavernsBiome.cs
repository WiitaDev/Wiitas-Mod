using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using WiitaMod.Tiles;

namespace WiitaMod.World.CrystalCaverns
{
    public class CrystalCavernsBiome : ModBiome
    {
        // Select all the scenery
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ExampleUndergroundBackgroundStyle>();

        // Select Music
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/CrystalCaverns");

        // Sets how the Scene Effect associated with this biome will be displayed with respect to vanilla Scene Effects. For more information see SceneEffectPriority & its values.
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        // Populate the Bestiary Filter
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        // Calculate when the biome is active.
        public override bool IsBiomeActive(Player player)
        {
            return (player.ZoneNormalCaverns || player.ZoneNormalUnderground) && ModContent.GetInstance<CrystalCavernsBiomeTileCount>().BlockCount >= 160;
        }

    }

    public class CrystalCavernsBiomeTileCount : ModSystem
    {
        public int BlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BlockCount = tileCounts[ModContent.TileType<CrystalRock>()];
        }
    }
}