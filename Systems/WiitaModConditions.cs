using Terraria;
using WiitaMod.Systems.BossSystems;
using WiitaMod.World;

namespace WiitaMod.Systems
{
    public static class WiitaModConditions
    {
        public static Condition InTropicalOcean = new Condition("Mods.WiitaMod.Conditions.InTropicalOcean", () => Main.LocalPlayer.InModBiome<TropicalOceanBiome>());
        public static Condition InTropicalCaverns = new Condition("Mods.WiitaMod.Conditions.InTropicalCaverns", () => Main.LocalPlayer.InModBiome<TropicalCavernsBiome>());
        public static Condition DownedTropicalCrabBoss = new("Mods.WiitaMod.Conditions.DownedTropicalCrabBoss", () => DownedBossSystem.downedTropicalCrabBoss);
    }
}