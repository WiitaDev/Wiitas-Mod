using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WiitaMod.Dusts
{
    public class TestDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            //dust.velocity *= 0.4f;
            dust.noGravity = true;
            dust.noLight = true;
            dust.scale *= 1f;
            dust.frame = new Rectangle(0, 0, 50, 50);
            dust.alpha = 0;
        }

        public override bool Update(Dust dust)
        {
            float light = 0.35f * dust.scale;
            Lighting.AddLight(dust.position, light, light, light);

            return false;
        }
    }
}