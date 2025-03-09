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
    public class WiitaModServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;


        [Header("Content")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool OnlyHamis;

        [Header("Gameplay")]
        [DefaultValue(true)]
        public bool FastRespawn;
    }
}