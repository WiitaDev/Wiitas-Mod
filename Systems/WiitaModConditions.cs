using Terraria;
using Terraria.Localization;
using WiitaMod.Systems.BossSystems;
using WiitaMod.World.CrystalCaverns;

namespace WiitaMod.Systems
{
    public static class WiitaModConditions
    {
        public static Condition InCrystalCaverns = new Condition("Mods.WiitaMod.Conditions.InCrystalCaverns", () => Main.LocalPlayer.InModBiome<CrystalCavernsBiome>());
        public static Condition DownedKapitalismiBoss = new("Mods.WiitaMod.Conditions.DownedKapitalismiBoss", () => DownedBossSystem.downedKapitalismiBoss);
    }
}   