using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WiitaMod.Systems.BossSystems
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedKapitalismiBoss = false;

        public override void ClearWorld()
        {
            downedKapitalismiBoss = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedKapitalismiBoss)
            {
                tag["downedKapitalismiBoss"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedKapitalismiBoss = tag.ContainsKey("downedKapitalismiBoss");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedKapitalismiBoss;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedKapitalismiBoss = flags[0];
        }
    }
}