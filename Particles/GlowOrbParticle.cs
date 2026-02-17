using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using WiitaMod.Particles.ParticleSystems;

namespace WiitaMod.Particles
{
    public class GlowOrbParticle : Particle
    {
        public Color InitialColor;
        public bool Gravity;
        public float fadeOut = 1;
        public bool glowCenter;
        public Vector2 _squish;
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;
        public override bool UseAdditiveBlend => true;

        public override string Texture => "WiitaMod/Particles/GlowOrbParticle";

        public GlowOrbParticle(Vector2 position, Vector2 velocity, bool gravity, int lifetime, float scale, Vector2 squish, Color color, bool GlowCenter = true)
        {
            Position = position;
            Velocity = velocity;
            Gravity = gravity;
            Scale = scale;
            Lifetime = lifetime;
            Color = InitialColor = color;
            glowCenter = GlowCenter;
            _squish = squish;
        }

        public override void Update()
        {
            fadeOut -= 0.1f;
            Scale *= 0.93f;
            Color = Color.Lerp(InitialColor, InitialColor * 0.2f, (float)Math.Pow(LifetimeCompletion, 3D));
            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && Gravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(1f, 1f) * Scale;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation, texture.Size() * 0.5f, scale * _squish, 0, 0f);
            if (glowCenter)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * fadeOut, Rotation, texture.Size() * 0.5f, scale * _squish * new Vector2(0.5f, 0.5f), 0, 0f);
        }
    }
}
