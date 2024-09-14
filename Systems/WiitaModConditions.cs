using Terraria;
using WiitaMod.Systems.BossSystems;
using WiitaMod.World;

namespace WiitaMod.Systems
{
    public static class WiitaModConditions
    {
        public static Condition InTropicalOcean = new Condition("In The Tropical Ocean", () => Main.LocalPlayer.InModBiome<TropicalOceanBiome>());
        public static Condition InTropicalCaverns = new Condition("In The Tropical Caverns", () => Main.LocalPlayer.InModBiome<TropicalCavernsBiome>());
        public static Condition DownedTropicalCrabBoss = new("Killed Carrus Insura", () => DownedBossSystem.downedTropicalCrabBoss);
    }
}