using CsvHelper.TypeConversion;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace WiitaMod.Dusts
{
    public class Glow : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true; // Makes the dust have no gravity.
            dust.noLight = true; // Makes the dust emit no light.
            dust.scale *= 1f; // Multiplies the dust's initial scale by 1.5.
            dust.frame = new Rectangle(0, 0, 30, 30);
        }

    }
}
