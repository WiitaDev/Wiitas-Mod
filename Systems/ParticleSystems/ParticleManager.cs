using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;

namespace WiitaMod.Systems.ParticleSystems
{

    public class ParticleManager //This code is form the Spirit Mods ParticleHandler
    {
        private static readonly int MaxParticlesAllowed = 500;

        private static Particle[] particles;
        private static int nextVacantIndex;
        private static int activeParticles;
        private static Dictionary<Type, int> particleTypes;
        private static Dictionary<int, Texture2D> particleTextures;
        private static List<Particle> particleInstances;
        private static List<Particle> batchedAlphaBlendParticles;
        private static List<Particle> batchedAdditiveBlendParticles;

        internal static void RegisterParticles()
        {
            particles = new Particle[MaxParticlesAllowed];
            particleTypes = new Dictionary<Type, int>();
            particleTextures = new Dictionary<int, Texture2D>();
            particleInstances = new List<Particle>();
            batchedAlphaBlendParticles = new List<Particle>(MaxParticlesAllowed);
            batchedAdditiveBlendParticles = new List<Particle>(MaxParticlesAllowed);

            Type baseParticleType = typeof(Particle);
            WiitaMod wiitaMod = ModContent.GetInstance<WiitaMod>();

            foreach (Type type in wiitaMod.Code.GetTypes())
            {
                if (type.IsSubclassOf(baseParticleType) && !type.IsAbstract && type != baseParticleType)
                {
                    int assignedType = particleTypes.Count;
                    particleTypes[type] = assignedType;

                    Particle instance = (Particle)FormatterServices.GetUninitializedObject(type);
                    particleInstances.Add(instance);

                    string texturePath = type.Namespace.Replace('.', '/') + "/" + type.Name;
                    if (instance.Texture != "")
                        texturePath = instance.Texture;
                    particleTextures[assignedType] = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad).Value;
                }
            }
        }

        internal static void Unload()
        {
            particles = null;
            particleTypes = null;
            particleTextures = null;
            particleInstances = null;
            batchedAlphaBlendParticles = null;
            batchedAdditiveBlendParticles = null;
        }

        public static void SpawnParticle(Particle particle)
        {
            if (Main.netMode == Terraria.ID.NetmodeID.Server || activeParticles == MaxParticlesAllowed)
                return;

            particles[nextVacantIndex] = particle;
            particle.ID = nextVacantIndex;
            particle.Type = particleTypes[particle.GetType()];

            if (nextVacantIndex + 1 < particles.Length && particles[nextVacantIndex + 1] == null)
                nextVacantIndex++;
            else
                for (int i = 0; i < particles.Length; i++)
                    if (particles[i] == null)
                        nextVacantIndex = i;

            activeParticles++;
        }

        public static void SpawnParticle(int type, Vector2 position, Vector2 velocity)
        {
            Particle particle = new Particle();
            particle.Position = position;
            particle.Velocity = velocity;
            particle.Color = Color.White;
            particle.Origin = Vector2.Zero;
            particle.Rotation = 0f;
            particle.Scale = 1f;
            particle.Type = type;

            SpawnParticle(particle);
        }

        /// <summary>
        /// Deletes the particle at the given index. You typically do not have to use this; use Particle.Kill() instead.
        /// </summary>
        public static void DeleteParticleAtIndex(int index)
        {
            particles[index] = null;
            activeParticles--;
            nextVacantIndex = index;
        }

        /// <summary>
        /// Clears all the currently spawned particles.
        /// </summary>
        public static void ClearAllParticles()
        {
            for (int i = 0; i < particles.Length; i++)
                particles[i] = null;

            activeParticles = 0;
            nextVacantIndex = 0;
        }

        internal static void UpdateAllParticles()
        {
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;

                particle.TimeActive++;
                particle.Position += particle.Velocity;

                particle.Update();
            }
        }

        internal static void DrawAllParticles(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;

                if (particle.UseAdditiveBlend)
                    batchedAdditiveBlendParticles.Add(particle);
                else
                    batchedAlphaBlendParticles.Add(particle);
            }
            spriteBatch.End();

            if (batchedAlphaBlendParticles.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, null, null, Main.GameViewMatrix.ZoomMatrix);

                foreach (Particle particle in batchedAlphaBlendParticles)
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(spriteBatch);
                    else
                        spriteBatch.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);

                spriteBatch.End();
            }

            if (batchedAdditiveBlendParticles.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                foreach (Particle particle in batchedAdditiveBlendParticles)
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(spriteBatch);
                    else
                        spriteBatch.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);

                spriteBatch.End();
            }

            batchedAlphaBlendParticles.Clear();
            batchedAdditiveBlendParticles.Clear();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        /// <summary>
        /// Gets the texture of the given particle type.
        /// </summary>
        public static Texture2D GetTexture(int type) => particleTextures[type];

        /// <summary>
        /// Returns the numeric type of the given particle.
        /// </summary>
        public static int ParticleType<T>() => particleTypes[typeof(T)];
    }
}