using Microsoft.Xna.Framework;
using System.Configuration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using WiitaMod.World;

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
            if(caller == Main.LocalPlayer)
            Main.NewText(caller.Player.InModBiome(ModContent.GetInstance<TropicalOceanBiome>()));
            Main.NewText(caller.Player.InModBiome(ModContent.GetInstance<TropicalCavernsBiome>()));
            Main.NewText(TropicalOceanGeneration.CaveStart);
        }
    }
}