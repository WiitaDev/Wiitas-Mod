using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using WiitaMod.World;

namespace WiitaMod.Systems
{
    public static class ExampleConditions
    {
        public static Condition InTropicalOcean = new Condition("Mods.WiitaMod.Conditions.InTropicalOcean", () => Main.LocalPlayer.InModBiome<TropicalOceanBiome>());
        public static Condition InTropicalCaverns = new Condition("Mods.WiitaMod.Conditions.InTropicalCaverns", () => Main.LocalPlayer.InModBiome<TropicalCavernsBiome>());
        public static Condition DownedTropicalCrabBoss = new("Mods.WiitaMod.Conditions.DownedTropicalCrabBoss", () => DownedBossSystem.downedTropicalCrabBoss);
    }
}