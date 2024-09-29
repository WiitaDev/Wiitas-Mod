using Microsoft.Xna.Framework;
using System.Configuration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using WiitaMod.World.TropicalOcean;

namespace WiitaMod.Systems
{
    public class ModCommands : ModCommand
    {
        public override CommandType Type
            => CommandType.Chat;

        public override string Command
        => "biome";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText(caller.Player.InModBiome(ModContent.GetInstance<TropicalCavernsBiome>()));
            Main.NewText(TropicalOceanGeneration.CaveStart);
            Main.NewText(TropicalOceanGeneration.BiomeWidth);
        }
    }
}