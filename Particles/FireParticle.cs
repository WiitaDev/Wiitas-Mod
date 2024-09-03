using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
using WiitaMod.Systems.ParticleSystems;

namespace WiitaMod.Particles
{
    public class FireParticle : Particle //this is lifted form the public calamity mod repository
    {
        public float RelativePower;
        public override bool SetLifetime => true;
        public override int FrameVariants => 3;

        public Color BrightColor;
        public Color DarkColor;


        public override string Texture => "WiitaMod/Particles/Fire";

        public FireParticle(Vector2 relativePosition, int lifetime, float scale, float relativePower, Color brightColor, Color darkColor)
        {
            Position = relativePosition;
            Velocity = Vector2.Zero;
            Scale = scale;
            Variant = Main.rand.Next(3);
            Lifetime = lifetime;
            RelativePower = relativePower;
            BrightColor = brightColor;
            DarkColor = darkColor;
        }

        public override void Update()
        {
            Scale += RelativePower * 0.01f;
            Position.Y -= RelativePower * 3f;

            Color = Color.Lerp(BrightColor, DarkColor, LifetimeCompletion);
            Color = Color.Lerp(Color, Color.SaddleBrown, Utils.GetLerpValue(0.95f, 0.7f, LifetimeCompletion, true));
            Color = Color.Lerp(Color, Color.White, Utils.GetLerpValue(0.1f, 0.25f, LifetimeCompletion, true) * Utils.GetLerpValue(0.4f, 0.25f, LifetimeCompletion, true) * 0.7f);
            Color *= Utils.GetLerpValue(0f, 0.15f, LifetimeCompletion, true) * Utils.GetLerpValue(1f, 0.8f, LifetimeCompletion, true) * 0.6f;
            Color.A = 50;
        }
    }
}