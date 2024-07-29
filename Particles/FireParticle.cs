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
        private Color _startColor;
        private Color _endColor;
        public int MaxTime;

        private const float FADETIME = 0.3f;

        public delegate void UpdateAction(Particle particle);

        private readonly UpdateAction _action;

        public override int FrameVariants => 3;

        public override string Texture => "WiitaMod/Particles/Fire";

        public FireParticle(Vector2 position, Vector2 velocity, Color startColor, Color endColor, float scale, int lifetime, UpdateAction action = null)
        {
            Position = position;
            Velocity = velocity;
            _startColor = startColor;
            _endColor = endColor;
            Scale = scale;
            MaxTime = lifetime;
            _action = action;
        }

        public override void Update()
        {
            Scale += 0.05f;

            Color = Color.Lerp(_startColor, _endColor, LifetimeCompletion);
            Color = Color.Lerp(Color, Color.SaddleBrown, Utils.GetLerpValue(0.95f, 0.7f, LifetimeCompletion, true));
            Color = Color.Lerp(Color, Color.White, Utils.GetLerpValue(0.1f, 0.25f, LifetimeCompletion, true) * Utils.GetLerpValue(0.4f, 0.25f, LifetimeCompletion, true) * 0.7f);
            Color *= Utils.GetLerpValue(0f, 0.15f, LifetimeCompletion, true) * Utils.GetLerpValue(1f, 0.8f, LifetimeCompletion, true) * 0.6f;
            Color.A = 50;
        }
    }
}