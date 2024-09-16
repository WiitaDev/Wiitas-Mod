using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;

namespace WiitaMod.Particles.ParticleSystems
{

    public class ParticleManager //This code is form the Spirit Mods ParticleHandler
    {
        private static readonly int MaxParticlesAllowed = 500;

        private static List<Particle> particles;
        //List containing the particles to delete
        private static List<Particle> particlesToKill;

        private static int nextVacantIndex;
        private static int activeParticles;
        private static Dictionary<Type, int> particleTypes;
        private static Dictionary<int, Texture2D> particleTextures;
        private static List<Particle> particleInstances;
        private static List<Particle> batchedAlphaBlendParticles;
        private static List<Particle> batchedAdditiveBlendParticles;

        internal static void RegisterParticles()
        {
            particles = new List<Particle>();
            particlesToKill = new List<Particle>();
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
            // Don't queue particles if the game is paused.
            // This precedent is established with how Dust instances are created.
            // Don't spawn particles if on the server side either, or if the particles dict is somehow null
            if (Main.gamePaused || Main.dedServ || particles == null)
                return;

            if (particles.Count >= MaxParticlesAllowed)
                return;

            particles.Add(particle);
            particle.Type = particleTypes[particle.GetType()];
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

        public static void RemoveParticle(Particle particle)
        {
            particlesToKill.Add(particle);
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

            //Clear out particles whose time is up
            particles.RemoveAll(particle => (particle.TimeActive >= particle.Lifetime && particle.SetLifetime) || particlesToKill.Contains(particle));
            particlesToKill.Clear();
        }

        internal static void DrawAllParticles(SpriteBatch sb)
        {
            if (particles.Count == 0)
                return;

            sb.End();
            var rasterizer = Main.Rasterizer;
            rasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            //Batch the particles to avoid constant restarting of the spritebatch
            foreach (Particle particle in particles)
            {
                if (particle == null)
                    continue;

                if (particle.UseAdditiveBlend)
                    batchedAdditiveBlendParticles.Add(particle);
                else
                    batchedAlphaBlendParticles.Add(particle);
            }
            if (batchedAlphaBlendParticles.Count > 0)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (Particle particle in batchedAlphaBlendParticles)
                {
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(sb);
                    else
                    {
                        Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                        sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f,
                            particle.Scale, SpriteEffects.None, 0f);
                    }
                }
                sb.End();
            }

            if (batchedAdditiveBlendParticles.Count > 0)
            {
                rasterizer = RasterizerState.CullNone;
                rasterizer.ScissorTestEnable = true;
                Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
                Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (Particle particle in batchedAdditiveBlendParticles)
                {
                    if (particle.UseCustomDraw)
                        particle.CustomDraw(sb);
                    else
                    {
                        Rectangle frame = particleTextures[particle.Type].Frame(1, particle.FrameVariants, 0, particle.Variant);
                        sb.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, frame, particle.Color, particle.Rotation, frame.Size() * 0.5f, particle.Scale, SpriteEffects.None, 0f);
                    }
                }
                sb.End();
            }

            batchedAlphaBlendParticles.Clear();
            batchedAdditiveBlendParticles.Clear();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
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