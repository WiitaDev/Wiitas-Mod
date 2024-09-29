using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WiitaMod.Projectiles;

namespace WiitaMod.Tiles
{
    public class CompressedSandstone : ModTile
    {
        public static readonly SoundStyle MineSound = new("WiitaMod/Assets/SFX/CompressedSandstoneHit", 3);

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = 36;
            AddMapEntry(new Color(26, 60, 83));
            HitSound = SoundID.Dig;
            MineResist = 3f;
            MinPick = 65;
            HitSound = MineSound;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanExplode(int i, int j) => true;
    }

}