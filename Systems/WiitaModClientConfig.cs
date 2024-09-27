using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WiitaMod.Systems
{
    public class WiitaModClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;


        [Header("Visuals")]
        [DefaultValue(true)]
        public bool Screenshake;
    }
}