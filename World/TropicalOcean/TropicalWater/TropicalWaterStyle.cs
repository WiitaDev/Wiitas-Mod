using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WiitaMod.World.TropicalOcean.TropicalWater
{
    public class TropicalWaterStyle : ModWaterStyle
    {
        private Asset<Texture2D> rainTexture;
        public override void Load()
        {
            rainTexture = Mod.Assets.Request<Texture2D>("World/TropicalOcean/TropicalWater/TropicalRain");
        }

        public override int ChooseWaterfallStyle()
        {
            return 0;
        }

        public override int GetSplashDust()
        {
            return DustID.Water;
        }

        public override int GetDropletGore()
        {
            return ModContent.GoreType<TropicalDroplet>();
        }

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 1f;
            b = 1f;
        }

        public override Color BiomeHairColor()
        {
            return Color.Aqua;
        }

        public override byte GetRainVariant()
        {
            return (byte)Main.rand.Next(3);
        }

        public override Asset<Texture2D> GetRainTexture() => rainTexture;
    }
}