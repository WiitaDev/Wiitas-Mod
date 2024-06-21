using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WiitaMod.Systems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedTropicalCrabBoss = false;

        public override void ClearWorld()
        {
            downedTropicalCrabBoss = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedTropicalCrabBoss)
            {
                tag["downedTropicalCrabBoss"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedTropicalCrabBoss = tag.ContainsKey("downedTropicalCrabBoss");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedTropicalCrabBoss;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedTropicalCrabBoss = flags[0];
        }
    }
}