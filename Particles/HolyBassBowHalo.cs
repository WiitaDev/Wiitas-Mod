using System;
using Microsoft.Xna.Framework;
using Terraria;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Particles
{
    public class BassBowHaloParticle : Particle
    {
        public override string Texture => "WiitaMod/Particles/HolyBassBowHalo";
        public override int FrameVariants => 2;

        private float Opacity;

        public BassBowHaloParticle(Vector2 position, float rotation, bool bigHalo)
        {
            Position = position;
            Rotation = rotation;
            Variant = bigHalo ? 0 : 1;
            Scale = 1f;
            Color = Color.White;
            Opacity = 255;
        }

        public override void Update()
        {
            Opacity -= 10;

            if (Opacity < 200) 
            {
                Scale += 0.04f;
                Color = Color.White * (Opacity / 255f);          
            }

            if (Opacity < 0)
                Kill();
        }
    }
}