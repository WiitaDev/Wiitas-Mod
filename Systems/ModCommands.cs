using Microsoft.Xna.Framework;
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
            => CommandType.World;

        public override string Command
        => "cavestart";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText(Main.worldSurface + 25);
            Main.NewText(Main.rockLayer + 75);
            Main.NewText(caller.Player.position.ToTileCoordinates().Y);
        }
    }
}