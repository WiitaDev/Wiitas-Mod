using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Particles
{
    public class MagicCircleFadeParticle : Particle
    {
        public override bool UseCustomDraw => true;

        public override string Texture => "WiitaMod/Assets/Textures/MagicCircle";
        private float Opacity;

        public MagicCircleFadeParticle(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = 1f;
            Color = Color.DodgerBlue;
            Opacity = 200;
        }

        public override void Update()
        {
            Opacity -= 6;

            if (Opacity < 200) 
            {
                Scale -= 0.03f;
                Color = Color.DodgerBlue * 0.5f * (Opacity / 255f);          
            }

            if (Opacity < 0)
                Kill();
        }

        public override void CustomDraw(SpriteBatch spriteBatch) 
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Color color = Color;
            color.A = 0;

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, color, Rotation, texture.Size() * 0.5f, Scale * new Vector2(0.6f, 1f), 0, 0f);
        }

    }
}