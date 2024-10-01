using Terraria.ModLoader;

namespace WiitaMod.World.TropicalOcean.Background
{
    public class TropicalCavernsBackgroundStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            for (int i = 0; i <= 3; i++)
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/TropicalCavernsBG" + i.ToString());
        }
    }
}